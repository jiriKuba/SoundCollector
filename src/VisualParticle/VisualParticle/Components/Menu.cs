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
        private Boolean _songArchivCheck;
        private Boolean _indexChange;
        private Int32 _selectedItem;
       
        private Int32 _width;
        private Int32 _nowWidth;

        private Int32 _nowHeight;
        private Int32 _height;

        private Int32 _positionX;
        private Int32 _nowPositionX;
        private Boolean _mouseMenuPressed;
        private readonly Dictionary<Int32, String> _menuItems;   
        private readonly Color _menuBackgroundColor;

        public Menu(MainGame mainGame)
            :base(mainGame)
        {
            this._menuBackgroundColor = new Color(200, 198, 0, 200);
            this._menuItems = new Dictionary<Int32, String>();
        }

        private List<Int32> GetRandomNineSongs()
        {
            Random random = new Random();
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            return mp.SongLibrary.OrderBy(x => random.NextDouble()).Select(s=> mp.SongLibrary.ToList().IndexOf(s)).Take(9).ToList();
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
            this._songArchivCheck = false;

            this._height = 350;
            this._width = 500;
            this._nowPositionX = 0;

            this._mouseMenuPressed = false;
        }

        public void LoadContent()
        {
            
        }

        public void UnloadContent()
        {
            this._menuItems.Clear();

            if(this._randomNineSongs != null)
                this._randomNineSongs.Clear();
        }

        public void Update(GameTime gameTime)
        {
            if (this._mainGame.IsPaused && this._mainGame.IsActive)
            {
                InputHandler ih = this._mainGame.GameManager.GetComponent<InputHandler>();

                this._positionX = this._mainGame.MainViewport.Height / 2 - this._height / 2;
                if (this._positionX > this._nowPositionX)
                    this._nowPositionX += 10;
                if (this._positionX < this._nowPositionX)
                    this._nowPositionX -= 10;

                if (this._nowHeight < this._height)
                    this._nowHeight += 10;
                else
                {
                    if (this._nowHeight > this._height)
                        this._nowHeight -= 10;
                }
                
                this._nowWidth = this._width;
                
                if ((!this._topTenCheck) && (!this._songArchivCheck))
                {
                    this._height = 350;
                    this._selectedItem = -1;
                    foreach (KeyValuePair<Int32, String> mi in this._menuItems)
                    {
                        if (new Rectangle(this._mainGame.MainViewport.Width / 2 - this._width / 2, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * mi.Key, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                        {
                            this._selectedItem = mi.Key;
                        }                        
                    }

                }
                else
                {
                    if (this._songArchivCheck)
                    {
                        this._height = 500;
                        if (!this._indexChange)
                        {
                            this._randomNineSongs = this.GetRandomNineSongs();
                            this._indexChange = true;
                        }

                        for (int j = 0; j < 9; j++)
                        {
                            if (new Rectangle(this._mainGame.MainViewport.Width / 2 - this._width / 2, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * j, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                                this._selectedItem = j;
                        }

                        //back
                        if (new Rectangle(this._mainGame.MainViewport.Width / 2 - this._width / 2, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * 9, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                            this._selectedItem = 9;

                        if (new Rectangle(this._mainGame.MainViewport.Width / 2, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * 9, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                            this._selectedItem = 10;
                    }
                    else
                    {
                        if (this._topTenCheck)
                        {
                            this._height = 500;
                            if (new Rectangle(this._mainGame.MainViewport.Width / 2, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 41 * 9, this._width, 60).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                                this._selectedItem = 0;
                            else
                                this._selectedItem = -1;
                        }
                    }
                }

                //menu.update(ih.ActualMouseState, this._mainGame.MainViewport);
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

                this._mainGame.MainSpriteBatch.Draw(rc.MenuBackground, new Rectangle(this._mainGame.MainViewport.Width / 2 - this._width / 2, this._nowPositionX, this._nowWidth, this._nowHeight), this._menuBackgroundColor);
                if (this._nowHeight == this._height && this._nowPositionX <= this._positionX)
                {

                    if (mp.SongLibrary != null && mp.SongLibrary.Count > 0)
                    {
                        if ((!this._topTenCheck) && (!this._songArchivCheck))
                        {
                            foreach (KeyValuePair<Int32, String> mi in this._menuItems)
                            {
                                if (this._selectedItem == mi.Key)
                                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, mi.Value, new Vector2(this._mainGame.MainViewport.Width / 2.5f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * mi.Key), Color.Red);
                                else
                                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, mi.Value, new Vector2(this._mainGame.MainViewport.Width / 2.5f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * mi.Key), Color.Black);

                                //i++;
                            }
                            //this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "now playing: " + title, new Vector2(this._mainGame.MainViewport.Width / 2 - Width / 2, this._mainGame.MainViewport.Height / 2 + Height / 2 - 50), Color.Purple);
                        }
                        else
                        {
                            if (this._topTenCheck)
                            {
                                Int32 trackIterator = 0;
                                foreach (Track t in tracks)
                                {
                                    try
                                    {
                                        this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, TextUtils.RemoveDiacritics(t.ToString()), new Vector2(this._mainGame.MainViewport.Width / 3f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * trackIterator), Color.Black);
                                        trackIterator++;
                                    }
                                    catch (ArgumentException)
                                    {
                                        trackIterator++;
                                    }                                    
                                }

                                if (this._selectedItem == 0)
                                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Back", new Vector2(this._mainGame.MainViewport.Width / 1.7f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 41 * 9), Color.Red);
                                else
                                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Back", new Vector2(this._mainGame.MainViewport.Width / 1.7f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 41 * 9), Color.Black);
                            }
                            else
                            {
                                if (this._songArchivCheck)
                                {
                                    if (this._randomNineSongs != null && this._randomNineSongs.Count > 0)
                                    {
                                        foreach (Int32 s in this._randomNineSongs)
                                        {
                                            Int32 indexOfSong = this._randomNineSongs.IndexOf(s);
                                            try
                                            {                                                
                                                if (this._selectedItem == indexOfSong)
                                                    this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, TextUtils.RemoveDiacritics(mp.SongLibrary[s].Artist.Name) + "-" + TextUtils.RemoveDiacritics(mp.SongLibrary[s].Name), new Vector2(this._mainGame.MainViewport.Width / 3f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * indexOfSong), Color.Red);
                                                else
                                                    this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, TextUtils.RemoveDiacritics(mp.SongLibrary[s].Artist.Name) + "-" + TextUtils.RemoveDiacritics(mp.SongLibrary[s].Name), new Vector2(this._mainGame.MainViewport.Width / 3f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * indexOfSong), Color.Black);
                                            }
                                            catch (ArgumentException)
                                            {
                                                this._mainGame.MainSpriteBatch.DrawString(rc.ScoreFont, "Wrong characrter", new Vector2(this._mainGame.MainViewport.Width / 3f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * indexOfSong), Color.Black);
                                            }
                                        }
                                    }

                                    if (this._selectedItem == 9)
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Next list", new Vector2(this._mainGame.MainViewport.Width / 3f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * 9), Color.Red);
                                    else
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Next list", new Vector2(this._mainGame.MainViewport.Width / 3f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * 9), Color.Black);

                                    if (this._selectedItem == 10)
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Back", new Vector2(this._mainGame.MainViewport.Width / 1.7f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * 9), Color.Red);
                                    else
                                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Back", new Vector2(this._mainGame.MainViewport.Width / 1.7f, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 60 + 40 * 9), Color.Black);

                                }

                            }
                        }
                    }
                    else
                    {
                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "Please insert some songs", new Vector2(this._mainGame.MainViewport.Width / 2 - this._width / 2 + 20, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 80), Color.Yellow);
                        this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "into MS MediaPlayer", new Vector2(this._mainGame.MainViewport.Width / 2 - this._width / 2 + 120, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 180), Color.Yellow);
                    }
                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "sound collector", new Vector2(this._mainGame.MainViewport.Width / 2.5f + 8, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 12), Color.Black);
                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "sound collector", new Vector2(this._mainGame.MainViewport.Width / 2.5f + 5, this._mainGame.MainViewport.Height / 2 - this._height / 2 + 10), Color.MediumSlateBlue);

                    this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, "runs on dpsf particle engine", new Vector2(this._mainGame.MainViewport.Width / 2 - this._width / 2, this._mainGame.MainViewport.Height / 2 + this._height / 2 - 40), Color.Peru);
                }
            }
        }

        private void MenuClick(Int32 clickedIndex)
        {
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            GameLogic gl = this._mainGame.GameManager.GetComponent<GameLogic>();
            IEnumerable<Track> tracks = mp.SongArchive.GetTopTen();

            if ((!this._songArchivCheck) && (!this._topTenCheck))
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
                        this._songArchivCheck = true;
                        break;
                    case 4:
                        this._mainGame.Exit();
                        break;
                }
            }
            else
            {

                if (this._songArchivCheck)
                {
                    mp.SetMediaPlayerShuffled(false);
                    if (clickedIndex < 9)
                    {
                        mp.PlaySongByIndex(this._randomNineSongs[clickedIndex]);
                    }

                    if (clickedIndex == 9)
                    {
                        this._indexChange = false;
                        mp.PauseSong();
                    }
                    else
                    {
                        if (clickedIndex == 10)
                        {
                            this._height = 350;
                            this._songArchivCheck = false;
                        }
                        else
                        {
                            gl.PauseGame();
                        }
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
        }
    }
}
