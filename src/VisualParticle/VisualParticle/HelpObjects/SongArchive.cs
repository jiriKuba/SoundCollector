using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System;
using SoundCollector.Utils;

namespace SoundCollector.HelpObjects
{
    class SongArchive
    {
        private Tracks _tracks;

        private const String HIGHSCORE_FILE_NAME = "highscore.dat";

        private readonly String HighscorePath;

        public SongArchive()
        {
            this._tracks = new Tracks();
            this.HighscorePath = System.IO.Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                MainGame.APPLICATION_ROAMING_FOLDER + "\\");
        }

        public void AddSongHightScore(String artis, String name, Int32 score, String stat)
        {
            this._tracks.Add(new Track(stat, artis, name, score));
        }


        public void UpdateSongHightScore(Int32 index, Int32 score, String stat)
        {
            this._tracks.TrackList[index].Score = score;
            this._tracks.TrackList[index].Stat = stat;
        }

        public Int32 GetHightScoreById(Int32 index)
        {
            return this._tracks.TrackList[index].Score;

        }

        public String GetHightStatusById(Int32 index)
        {
            return this._tracks.TrackList[index].Stat;
        }

        public String GetStateById(Int32 index)
        {
            return this._tracks.TrackList[index].Stat;
        }

        public IEnumerable<Track> GetTopNItem(Int32 n)
        {
            return this._tracks.TrackList.OrderByDescending(x => x.Score).Take(n);
        }

        public IEnumerable<Track> GetTopTen()
        {
            return this.GetTopNItem(10);
        }

        public Int32? SearchByArtisName(String artis, String name)
        {
            foreach (Track track in this._tracks.TrackList)
            {
                if ((track.Name == name) && (track.Artis == artis))
                    return this._tracks.TrackList.IndexOf(track);
            }
            return null;
        }

        public Boolean OpenArchive()
        {
            try
            {
                if (!System.IO.Directory.Exists(this.HighscorePath) || !System.IO.File.Exists(this.HighscorePath + HIGHSCORE_FILE_NAME))
                {
                    //default
                    this.SaveArchive();
                }
                else
                {
                    this._tracks = DataUtils.LoadObjectAsXmlFromFile<Tracks>(this.HighscorePath + HIGHSCORE_FILE_NAME);
                }

                return true;
            }

            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
        }

        public void SaveArchive()
        {
            if (!System.IO.Directory.Exists(this.HighscorePath))
            {
                System.IO.Directory.CreateDirectory(this.HighscorePath);
            }

            DataUtils.SaveObjectAsXmlToFile(this._tracks, this.HighscorePath + HIGHSCORE_FILE_NAME);
        }
    }
}
