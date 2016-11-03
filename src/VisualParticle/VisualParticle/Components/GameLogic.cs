using SoundCollector.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using DPSF;
using Microsoft.Xna.Framework.Content;
using SoundCollector.Utils;

namespace SoundCollector.Components
{
    class GameLogic : Base.BaseGameComponent, IExtendedGameComponent
    {
        private TimeSpan _previouslyGameTime;
        private String _musicTimeText;
        private Int32 _musicMinutes;
        private Int32 _musicSeconds;
        private Int32 _shieldHit;
        private Int32 _particleCatch;
        private TimeSpan _prevMusicTime;
        private Int32 _timeToBonus;
        private Int32 _bonusTime;
        private Int32 _actualSongScore;
        private Int32 _actualSongHightScore;
        private Int32 _particlesPerGame;
        private String _actualSongStatus;
        private Boolean _isSongFinished;

        public GameLogic(MainGame mainGame)
            : base(mainGame)
        {

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize()
        {
            InputHandler ih = this._mainGame.GameManager.GetComponent<InputHandler>();
            ParticleSystems ps = this._mainGame.GameManager.GetComponent<ParticleSystems>();
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            Menu m = this._mainGame.GameManager.GetComponent<Menu>();

            ih.EscapePressed += Ih_EscapePressed;
            ih.SpacePressed += Ih_SpacePressed;
            ih.GamePadBackPressed += Ih_GamePadBackPressed;

            ps.BonusFieldDisabled += Ps_BonusFieldDisabled;
            ps.ParticleHitMouse += Ps_ParticleHitMouse;
            ps.ParticleOutOfScreen += Ps_ParticleOutOfScreen;
            ps.ParticleHitPlayer += Ps_ParticleHitPlayer;

            mp.BeforeSongArchiveClosed += Mp_BeforeSongArchiveClosed;
            mp.SongArchiveOpend += Mp_SongArchiveOpend;
            mp.SongChanged += Mp_SongChanged;

            this._isSongFinished = false;
        }

        private void Mp_SongChanged(object sender, EventArgs e)
        {
            if (sender != null && sender is MusicPlayer)
            {
                MusicPlayer mp = (MusicPlayer)sender;
                StatusMaker sm = this._mainGame.GameManager.GetComponent<StatusMaker>();
                //score metody
                sm.GenerateStatus(this._shieldHit, this._particleCatch, this._particlesPerGame, this._isSongFinished);
                Song prevSong = mp.PreviousSong;
                if (prevSong != null)
                {
                    Int32? searchForSong = mp.SongArchive.SearchByArtisName(prevSong.Artist.Name, prevSong.Name);
                    if (searchForSong.HasValue)
                    {
                        if (this._actualSongScore > mp.SongArchive.GetHightScoreById(searchForSong.Value))
                            mp.SongArchive.UpdateSongHightScore(searchForSong.Value, this._actualSongScore, sm.Status);
                    }
                    else
                    {
                        mp.SongArchive.AddSongHightScore(prevSong.Artist.Name, prevSong.Name, this._actualSongScore, sm.Status);
                    }
                }

                //reset score
                Int32? searchForSongActual = mp.SongArchive.SearchByArtisName(mp.ActiveSong.Artist.Name, mp.ActiveSong.Name);
                if (searchForSongActual.HasValue)
                {
                    this._actualSongHightScore = mp.SongArchive.GetHightScoreById(searchForSongActual.Value);
                    this._actualSongStatus = mp.SongArchive.GetHightStatusById(searchForSongActual.Value);
                }
                else
                {
                    this._actualSongHightScore = 0;
                }

                this._mainGame.Window.Title = mp.GetActiveSongName();//MediaPlayer.Queue.ActiveSong.Artist.Name + "-" + MediaPlayer.Queue.ActiveSong.Name;

                this.NewGame();
            }
        }

        private void Mp_SongArchiveOpend(object sender, EventArgs e)
        {
            if (sender != null && sender is MusicPlayer)
            {
                MusicPlayer mp = (MusicPlayer)sender;
                if (mp.ActualSongId.HasValue)
                {
                    this._actualSongHightScore = mp.SongArchive.GetHightScoreById(mp.ActualSongId.Value);
                    this._actualSongStatus = mp.SongArchive.GetHightStatusById(mp.ActualSongId.Value);
                }
            }
        }

        private void Mp_BeforeSongArchiveClosed(object sender, EventArgs e)
        {
            if (sender != null && sender is MusicPlayer)
            {
                MusicPlayer mp = (MusicPlayer)sender;
                StatusMaker sm = this._mainGame.GameManager.GetComponent<StatusMaker>();
                if (mp.ActualSongId.HasValue)
                {
                    if (this._actualSongScore > mp.SongArchive.GetHightScoreById(mp.ActualSongId.Value))
                        mp.SongArchive.UpdateSongHightScore(mp.ActualSongId.Value, this._actualSongScore, sm.Status);
                }
                //else
                //{
                //    mp.SongArchive.AddSongHightScore(mp.ActiveSong.Artist.Name, mp.ActiveSong.Name, score, sm.Status);
                //}
            }
        }

        private void Ps_ParticleHitPlayer(object sender, EventArgs e)
        {
            if (sender != null && sender is Player)
            {
                Player player = (Player)sender;
                MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
                player.Shield--;
                this._particlesPerGame++;
                this._shieldHit++;
                if (player.Shield <= 0)
                {
                    mp.ShuffledNextSong();
                    //MediaPlayer.IsShuffled = true;
                    //MediaPlayer.MoveNext();
                }
            }
        }

        private void Ps_ParticleOutOfScreen(object sender, EventArgs e)
        {
            this._particlesPerGame++;
        }

        private void Ps_ParticleHitMouse(object sender, EventArgs e)
        {
            if (sender != null && sender is Player)
            {
                Player player = (Player)sender;
                if (player.Shield < 100)
                {
                    this._particleCatch++;
                    player.Shield++;
                    this._actualSongScore += player.Multiplier;
                    this._particlesPerGame++;
                }
                else
                {
                    this._particleCatch++;
                    this._actualSongScore += player.Multiplier;
                    this._particlesPerGame++;
                    //score multiplicator
                }
            }
        }

        private void Ps_BonusFieldDisabled(object sender, EventArgs e)
        {
            this._bonusTime = 5;
            this._timeToBonus = 35;
        }

        private void Ih_SpacePressed(object sender, EventArgs e)
        {
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            mp.ShuffledNextSong();
            //MediaPlayer.IsShuffled = true;
            //MediaPlayer.MoveNext();
        }

        private void Ih_GamePadBackPressed(object sender, EventArgs e)
        {
            this._mainGame.Exit();
        }

        private void Ih_EscapePressed(object sender, EventArgs e)
        {
            this.PauseGame();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void LoadContent()
        {
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            StatusMaker sm = this._mainGame.GameManager.GetComponent<StatusMaker>();

            this._mainGame.Window.Title = mp.GetActiveSongName();

            //music minutes and seconds
            this._musicMinutes = mp.GetActiveSongDuration().Minutes;
            this._musicSeconds = mp.GetActiveSongDuration().Seconds;

            if (this._musicSeconds < 10)
                this._musicTimeText = this._musicMinutes + ":0" + this._musicSeconds;
            else
                this._musicTimeText = this._musicMinutes + ":" + this._musicSeconds;

            sm.GenerateStatus(-1, 0, 0, this._isSongFinished);

            this.NewGame();

            //game is paused when start
            this._mainGame.IsPaused = true;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            DynamicWorldCounter dwc = this._mainGame.GameManager.GetComponent<DynamicWorldCounter>();
            InputHandler ih = this._mainGame.GameManager.GetComponent<InputHandler>();

            if (!this._mainGame.IsActive)
            {
                this._mainGame.ResetElapsedTime();
                this._mainGame.IsPaused = false;
                this.PauseGame();
            }

            if (!this._mainGame.IsPaused)
            {
                if (gameTime.TotalGameTime - this._previouslyGameTime > this._mainGame.OneSecond)
                {
                    this._previouslyGameTime = gameTime.TotalGameTime;
                }

                if (gameTime.TotalGameTime - this._prevMusicTime > mp.MusicTick)
                {
                    ParticleSystems ps = this._mainGame.GameManager.GetComponent<ParticleSystems>();

                    if (this._timeToBonus > 0)
                        this._timeToBonus--;

                    if (this._timeToBonus == 0)
                        ps.BonusFieldPrepar = true;
                    if (ps.BonusFieldActive)
                        this._bonusTime--;

                    if (this._bonusTime == 0)
                    {
                        ps.BonusFieldPrepar = false;
                        ps.BonusFieldActive = false;
                        this._timeToBonus = 35;
                        this._bonusTime = 5;
                    }

                    //when music time is over and song didnt changed
                    if (this._musicMinutes == 0 && this._musicSeconds == 0)
                    {
                        mp.ShuffledNextSong();
                    }

                    this.CalculateTimeToEnd();

                    this._prevMusicTime = gameTime.TotalGameTime;
                }
            }
        }

        private void CalculateTimeToEnd()
        {
            this._musicSeconds--;

            if ((this._musicSeconds < 0) && (this._musicMinutes <= 0))
            {
                this._musicSeconds = 0;
            }
            if ((this._musicSeconds < 10) && (this._musicMinutes <= 0))
            {
                this._isSongFinished = true;
            }

            if (this._musicSeconds < 0)
            {
                this._musicSeconds = 59;
                this._musicMinutes--;
            }

            if (this._musicSeconds < 10)
                this._musicTimeText = this._musicMinutes + ":0" + this._musicSeconds;
            else
                this._musicTimeText = this._musicMinutes + ":" + this._musicSeconds;
        }

        public void PauseGame()
        {
            ParticleSystems ps = this._mainGame.GameManager.GetComponent<ParticleSystems>();
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();

            if (this._mainGame.IsPaused)
            {
                //unpause
                mp.UnPauseSong();
                this._mainGame.IsPaused = false;
                ps.PauseParticles(false);
            }
            else
            {
                //pause
                mp.PauseSong();
                this._mainGame.IsPaused = true;
                ps.PauseParticles(true);
            }
        }

        public void NewGame()
        {
            DynamicWorldCounter dwc = this._mainGame.GameManager.GetComponent<DynamicWorldCounter>();
            ParticleSystems ps = this._mainGame.GameManager.GetComponent<ParticleSystems>();
            Player player = this._mainGame.GameManager.GetComponent<Player>();
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();

            this._actualSongScore = 0;
            player.ReInit();

            this._particlesPerGame = 0;
            this._isSongFinished = false;
            this._shieldHit = 0;
            this._particleCatch = 0;
            dwc.ReInit();

            this._bonusTime = 5;
            this._timeToBonus = 35;

            this._musicMinutes = mp.GetActiveSongDuration().Minutes;
            this._musicSeconds = mp.GetActiveSongDuration().Seconds;
            ps.ReInit();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            ParticleSystems ps = this._mainGame.GameManager.GetComponent<ParticleSystems>();
            Player p = this._mainGame.GameManager.GetComponent<Player>();
            ResourcesComponent rc = this._mainGame.GameManager.GetComponent<ResourcesComponent>();

            this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Score: " + this._actualSongScore, new Vector2(2, 2), Color.Magenta * mp.AvarageFrequency * 1.6f);
            this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Score: " + this._actualSongScore, new Vector2(0, 0), Color.White);

            this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Multiply: " + p.Multiplier + "x", new Vector2(0, 35), Color.White);
            this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Time: " + this._musicTimeText, new Vector2(this._mainGame.MainViewport.Width - 150, 15), Color.White);
            this._mainGame.MainSpriteBatch.DrawString(rc.SmallerFont, "[esc]to menu", new Vector2(this._mainGame.MainViewport.Width - 150, 2), Color.White);

            if (this._actualSongHightScore > 0)
            {
                this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "High score: " + this._actualSongHightScore, new Vector2(0, 55), Color.White);
                this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, ", " + this._actualSongStatus, new Vector2(200, 55), Color.Purple);
            }
            else
            {
                this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "High score: First time playing", new Vector2(0, 55), Color.White);
            }

            this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Shield: " + p.Shield + "%", new Vector2(0, 75), Color.White);

            if ((ps.BonusPower >= 0) && (ps.BonusPower < 25))
            {
                this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Bonus Lux", new Vector2(0, 175), Color.Red);
            }
            else if ((ps.BonusPower >= 25) && (ps.BonusPower < 50))
            {
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 160), Color.Red);
                this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Bonus Lux", new Vector2(0, 175), Color.Red);
            }
            else if ((ps.BonusPower >= 50) && (ps.BonusPower < 75))
            {
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 145), Color.Red);
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 160), Color.Red);
                this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Bonus Lux", new Vector2(0, 175), Color.Red);
            }
            else if ((ps.BonusPower >= 75) && (ps.BonusPower < 100))
            {
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 130), Color.Orange);
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 145), Color.Orange);
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 160), Color.Orange);
                this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Bonus Lux", new Vector2(0, 175), Color.Orange);
            }
            else if (ps.BonusPower >= 100)
            {
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 115), Color.Green);
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 130), Color.Green);
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 145), Color.Green);
                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Vector2(10, 160), Color.Green);
                this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Bonus Lux", new Vector2(0, 175), Color.Green);
            }

            if (this._timeToBonus > 0)
            {
                if (this._timeToBonus > 9)
                    this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "bonus shield: 0:" + this._timeToBonus, new Vector2(0, 195), Color.Red);
                else
                    this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "bonus shield: 0:0" + this._timeToBonus, new Vector2(0, 195), Color.Red);
            }
            else
            {
                if ((this._timeToBonus == 0) && (!ps.BonusFieldActive))
                {
                    this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "bonus shield: 0:00", new Vector2(0, 195), Color.Green);
                }
                if (ps.BonusFieldActive)
                {
                    this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "bonus shield: 0:0" + this._bonusTime, new Vector2(0, 195), Color.Purple);
                }
            }
        }
    }
}
