using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SoundCollector.Components.Interfaces;
using SoundCollector.Utils;
using SoundCollector.HelpObjects;

namespace SoundCollector.Components
{
    class Menu : Base.BaseGameComponent, IExtendedGameComponent
    {
        private List<Int32> _randomNineSongs;
        private Boolean _topTenCheck;
        private Boolean _randomNineCheck;
        private Boolean _indexChange;
        private Int32 _selectedItem;

        private Int32 _width;
        private Int32 _nowWidth;

        private Int32 _nowHeight;
        private Int32 _height;

        private Int32 _positionY;
        private Int32 _nowPositionY;
        private Boolean _mouseMenuPressed;
        private readonly Dictionary<Int32, String> _menuItems;
        private readonly Color _menuBackgroundColor;

        public Menu(MainGame mainGame)
            : base(mainGame)
        {
            this._menuBackgroundColor = new Color(200, 198, 0, 200);
            this._menuItems = new Dictionary<Int32, String>();
        }

        private List<Int32> GetRandomNineSongs()
        {
            Random random = new Random();
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            return mp.SongLibrary.OrderBy(x => random.NextDouble()).Select(s => mp.SongLibrary.ToList().IndexOf(s)).Take(9).ToList();
        }

        public void Initialize()
        {
            this._menuItems.Add(0, "resume");
            this._menuItems.Add(1, "nextsong");
            this._menuItems.Add(2, "scoreboard");
            this._menuItems.Add(3, "random playlist");
            this._menuItems.Add(4, "exit");

            this._indexChange = false;

            this._topTenCheck = false;
            this._randomNineCheck = false;

            this._height = 350;
            this._width = 500;
            this._nowPositionY = 0;

            this._mouseMenuPressed = false;
        }

        public void LoadContent()
        {

        }

        public void UnloadContent()
        {
            this._menuItems.Clear();

            if (this._randomNineSongs != null)
                this._randomNineSongs.Clear();
        }

        public void Update(GameTime gameTime)
        {
            if (this._mainGame.IsPaused && this._mainGame.IsActive)
            {
                InputHandler ih = this._mainGame.GameManager.GetComponent<InputHandler>();

                this._positionY = this._mainGame.MainViewport.Height / 2 - this._height / 2;
                if (this._positionY > this._nowPositionY)
                    this._nowPositionY += 10;
                if (this._positionY < this._nowPositionY)
                    this._nowPositionY -= 10;

                if (this._nowHeight < this._height)
                    this._nowHeight += 10;
                else
                {
                    if (this._nowHeight > this._height)
                        this._nowHeight -= 10;
                }

                this._nowWidth = this._width;

                if ((!this._topTenCheck) && (!this._randomNineCheck))
                {
                    this._height = 350;
                    this._selectedItem = -1;
                    foreach (KeyValuePair<Int32, String> mi in this._menuItems)
                    {
                        if (new Rectangle(this.GetMenuX(), this._nowPositionY + 60 + 40 * mi.Key, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                        {
                            this._selectedItem = mi.Key;
                        }
                    }
                }
                else
                {
                    if (this._randomNineCheck)
                    {
                        this._height = 500;
                        if (!this._indexChange)
                        {
                            this._randomNineSongs = this.GetRandomNineSongs();
                            this._indexChange = true;
                        }

                        for (int j = 0; j < 9; j++)
                        {
                            if (new Rectangle(this.GetMenuX(), this._nowPositionY + 60 + 40 * j, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                                this._selectedItem = j;
                        }

                        //back
                        if (new Rectangle(this.GetMenuX(), this._nowPositionY + 60 + 40 * 9, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                            this._selectedItem = 9;

                        //next
                        if (new Rectangle(this._mainGame.MainViewport.Width / 2, this._nowPositionY + 60 + 40 * 9, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                            this._selectedItem = 10;
                    }
                    else if (this._topTenCheck)
                    {
                        this._height = 500;
                        if (new Rectangle(this.GetMenuX(), this._nowPositionY + 60 + 40 * 9, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                            this._selectedItem = 0;
                        else
                            this._selectedItem = -1;
                    }
                }

                if (ih.ActualMouseState.LeftButton == ButtonState.Pressed && !this._mouseMenuPressed)
                {
                    this.MenuClick(this._selectedItem);
                    this._mouseMenuPressed = true;

                }
                if (ih.ActualMouseState.LeftButton == ButtonState.Released)
                    this._mouseMenuPressed = false;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (this._mainGame.IsPaused && this._mainGame.IsActive)
            {
                ResourcesComponent rc = this._mainGame.GameManager.GetComponent<ResourcesComponent>();
                MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
                IEnumerable<Track> tracks = mp.SongArchive.GetTopTen();

                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Rectangle(this.GetMenuX(), this._nowPositionY, this._nowWidth, this._nowHeight), this._menuBackgroundColor);
                if (this._nowHeight == this._height && this._nowPositionY <= this._positionY)
                {
                    if (mp.SongLibrary != null && mp.SongLibrary.Count > 0)
                    {
                        if ((!this._topTenCheck) && (!this._randomNineCheck))
                        {
                            if (this._menuItems != null && this._menuItems.Count > 0)
                            {
                                foreach (KeyValuePair<Int32, String> mi in this._menuItems)
                                {
                                    if (this._selectedItem == mi.Key)
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, mi.Value, new Vector2(this.GetMenuX() + 145, this._nowPositionY + 60 + 40 * mi.Key), Color.Red);
                                    else
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, mi.Value, new Vector2(this.GetMenuX() + 145, this._nowPositionY + 60 + 40 * mi.Key), Color.Black);

                                    //i++;
                                }
                            }
                            //this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "now playing: " + title, new Vector2(this._mainGame.MainViewport.Width / 2 - Width / 2, this._mainGame.MainViewport.Height / 2 + Height / 2 - 50), Color.Purple);
                        }
                        else
                        {
                            if (this._topTenCheck)
                            {
                                Int32 trackIterator = 0;
                                if (tracks != null && tracks.Any())
                                {
                                    foreach (Track t in tracks)
                                    {
                                        try
                                        {
                                            this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, TextUtils.RemoveDiacritics(t.ToString()), new Vector2(this.GetMenuX() + 15, this._nowPositionY + 60 + 40 * trackIterator), Color.Black);
                                            trackIterator++;
                                        }
                                        catch (ArgumentException)
                                        {
                                            trackIterator++;
                                        }
                                    }
                                }

                                if (this._selectedItem == 0)
                                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Back", new Vector2(this.GetMenuX() + 15, this._nowPositionY + 60 + 41 * 9), Color.Red);
                                else
                                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Back", new Vector2(this.GetMenuX() + 15, this._nowPositionY + 60 + 41 * 9), Color.Black);
                            }
                            else
                            {
                                if (this._randomNineCheck)
                                {
                                    if (this._randomNineSongs != null && this._randomNineSongs.Count > 0)
                                    {
                                        foreach (Int32 s in this._randomNineSongs)
                                        {                                            
                                            try
                                            {
                                                Int32 indexOfSong = this._randomNineSongs.IndexOf(s);
                                                String songName = TextUtils.GetSongName(mp.SongLibrary[s]);
                                                songName = (String.IsNullOrEmpty(songName) || songName.Length <= Track.MAX_FULL_TEXT_LENGTH) ? songName : songName.Substring(0, Track.MAX_FULL_TEXT_LENGTH) + Track.DOTS;
                                                songName = TextUtils.RemoveDiacritics(songName);
                                                if (this._selectedItem == indexOfSong)
                                                    this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, songName, new Vector2(this.GetMenuX() + 15, this._nowPositionY + 60 + 40 * indexOfSong), Color.Red);
                                                else
                                                    this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, songName, new Vector2(this.GetMenuX() + 15, this._nowPositionY + 60 + 40 * indexOfSong), Color.Black);
                                            }
                                            catch 
                                            {
                                                //nothing
                                                //this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Wrong character", new Vector2(this._mainGame.MainViewport.Width / 3f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * indexOfSong), Color.Black);
                                            }
                                        }
                                    }

                                    if (this._selectedItem == 9)
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Back", new Vector2(this.GetMenuX() + 15, this._nowPositionY + 60 + 40 * 9), Color.Red);
                                    else
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Back", new Vector2(this.GetMenuX() + 15, this._nowPositionY + 60 + 40 * 9), Color.Black);

                                    if (this._selectedItem == 10)
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Next list", new Vector2(this.GetMenuX() + this._nowWidth - 165, this._nowPositionY + 60 + 40 * 9), Color.Red);
                                    else
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Next list", new Vector2(this.GetMenuX() + this._nowWidth - 165, this._nowPositionY + 60 + 40 * 9), Color.Black);
                                }

                            }
                        }
                    }
                    else
                    {
                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Please insert some songs", new Vector2(this.GetMenuX() + 20, this._nowPositionY + 80), Color.Yellow);
                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "into MS MediaPlayer", new Vector2(this.GetMenuX() + 120, this._nowPositionY + 180), Color.Yellow);
                    }
                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "sound collector", new Vector2(this.GetMenuX() + 118, this._nowPositionY + 12), Color.Black);
                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "sound collector", new Vector2(this.GetMenuX() + 115, this._nowPositionY + 10), Color.MediumSlateBlue);

                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "runs on dpsf particle engine", new Vector2(this.GetMenuX(), this._nowPositionY + this._height - 41), Color.Peru);
                }
            }
        }

        private void MenuClick(Int32 clickedIndex)
        {
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            GameLogic gl = this._mainGame.GameManager.GetComponent<GameLogic>();
            IEnumerable<Track> tracks = mp.SongArchive.GetTopTen();

            if ((!this._randomNineCheck) && (!this._topTenCheck))
            {
                this._height = 500;
                switch (clickedIndex)
                {

                    case 0:
                        gl.PauseGame();
                        break;
                    case 1:
                        gl.PauseGame();
                        mp.NextShuffledSong();
                        break;
                    case 2:
                        this._topTenCheck = true;
                        break;
                    case 3:
                        this._randomNineCheck = true;
                        break;
                    case 4:
                        this._mainGame.Exit();
                        break;
                }
            }
            else if (this._randomNineCheck)
            {
                mp.SetMediaPlayerShuffled(false);
                if (clickedIndex < 9)
                {
                    mp.PlaySongByIndex(this._randomNineSongs[clickedIndex]);
                }

                if (clickedIndex == 10)
                {
                    this._indexChange = false;
                    mp.PauseSong();
                }
                else if (clickedIndex == 9)
                {
                    this._height = 350;
                    this._randomNineCheck = false;
                }
                else
                {
                    gl.PauseGame();
                }
            }
            else
            {
                if (this._topTenCheck)
                {
                    this._height = 350;
                    this._topTenCheck = false;
                }
            }
        }

        private Int32 GetMenuX()
        {
            return this._mainGame.MainViewport.Width / 2 - this._width / 2;
        }
    }
}