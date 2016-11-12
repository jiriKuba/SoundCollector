using SoundCollector.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using SoundCollector.HelpObjects;
using SoundCollector.Utils;

namespace SoundCollector.Components
{
    class MusicPlayer : Base.BaseGameComponent, IExtendedGameComponent
    {
        public VisualizationData VisualizationData { get; private set; }

        public float AvarageFrequency { get; private set; }

        public TimeSpan MusicTick { get; private set; }

        public MediaLibrary MediaLibrary { get; private set; }

        public SongArchive SongArchive { get; private set; }

        public SongCollection SongLibrary { get; private set; }

        public Int32? ActualSongId { get; private set; }

        public event EventHandler<EventArgs> SongArchiveOpend;
        public event EventHandler<EventArgs> BeforeSongArchiveClosed;
        public event EventHandler<EventArgs> SongChanged;

        public Song ActiveSong
        {
            get
            {
                return MediaPlayer.Queue.ActiveSong;
            }
        }

        public Song PreviousSong { get; private set; }

        //private readonly MainGame _mainGame;

        public MusicPlayer(MainGame mainGame)
            : base(mainGame)
        {
            this.VisualizationData = new VisualizationData();
            this.MediaLibrary = new MediaLibrary();
            this.SongArchive = new SongArchive();
            this.MusicTick = TimeSpan.FromSeconds(1);

            MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
        }

        public void Draw(GameTime gameTime)
        {

        }

        public void Initialize()
        {
            this.SongLibrary = this.MediaLibrary.Songs;
            this.PreviousSong = this.ActiveSong;
        }

        private void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                if (this.SongChanged != null)
                    this.SongChanged.Invoke(this, EventArgs.Empty);

                this.PreviousSong = this.ActiveSong;
            }
        }

        public void LoadContent()
        {
            MediaPlayer.IsShuffled = true;
            if (this.MediaLibrary != null && this.MediaLibrary.Songs.Count > 0)
                MediaPlayer.Play(this.MediaLibrary.Songs);

            MediaPlayer.Pause();
            MediaPlayer.IsVisualizationEnabled = true;

            if (!this.SongArchive.OpenArchive())
            {
                this.SongArchive.AddSongHightScore(this.ActiveSong.Artist.Name, this.ActiveSong.Name, 0, StatusMaker.UNFINISHED_STATUS_TEXT);
            }

            this.ActualSongId = this.ActiveSong == null ? null : this.SongArchive.SearchByArtisName(this.ActiveSong.Artist.Name, this.ActiveSong.Name);

            if (this.SongArchiveOpend != null)
                this.SongArchiveOpend.Invoke(this, EventArgs.Empty);
        }

        public void UnloadContent()
        {
            MediaPlayer.Pause();
            this.ActualSongId = this.ActiveSong == null ? null : this.SongArchive.SearchByArtisName(this.ActiveSong.Artist.Name, this.ActiveSong.Name);

            if (this.BeforeSongArchiveClosed != null)
                this.BeforeSongArchiveClosed.Invoke(this, EventArgs.Empty);
            this.SongArchive.SaveArchive();
        }

        public void Update(GameTime gameTime)
        {
            if (!this._mainGame.IsPaused)
            {
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.GetVisualizationData(this.VisualizationData);
                }

                this.AvarageFrequency = 0f;
                if (this.VisualizationData != null && this.VisualizationData.Frequencies != null && this.VisualizationData.Frequencies.Count > 0)
                {
                    for (int i = 0; i < this.VisualizationData.Frequencies.Count; i++)
                    {
                        this.AvarageFrequency += this.VisualizationData.Frequencies[i];
                    }
                    this.AvarageFrequency = this.AvarageFrequency / this.VisualizationData.Frequencies.Count;
                }
            }
        }

        public void NextShuffledSong()
        {
            MediaPlayer.MoveNext();
            MediaPlayer.IsShuffled = true;
        }

        public void ShuffledNextSong()
        {
            MediaPlayer.IsShuffled = true;
            MediaPlayer.MoveNext();
        }

        public void PauseSong()
        {
            MediaPlayer.Pause();
        }

        public void UnPauseSong()
        {
            MediaPlayer.Resume();
        }

        public void SetMediaPlayerShuffled(Boolean isShuffled)
        {
            MediaPlayer.IsShuffled = isShuffled;
        }

        public void PlaySongByIndex(Int32 index)
        {
            if (index >= 0 && this.MediaLibrary.Songs != null && this.MediaLibrary.Songs.Count > 0 && index < this.MediaLibrary.Songs.Count)
                MediaPlayer.Play(this.SongLibrary, index);
        }

        public String GetActiveSongName()
        {
            if (this.ActiveSong == null)
                return MainGame.APPLICATION_ROAMING_FOLDER;
            else
                return TextUtils.GetSongName(this.ActiveSong);
        }

        public TimeSpan GetActiveSongDuration()
        {
            if (this.ActiveSong == null)
                return default(TimeSpan);
            else
                return this.ActiveSong.Duration;
        }
    }
}