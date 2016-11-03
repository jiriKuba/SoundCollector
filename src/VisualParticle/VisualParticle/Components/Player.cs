using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DPSF;
using Microsoft.Xna.Framework.Graphics;
using SoundCollector.Components.Interfaces;
using SoundCollector.Utils;

namespace SoundCollector.Components
{
    class Player : Base.BaseGameComponent, IExtendedGameComponent
    {
        //public attributes
        public Int32 Size { get; set; }
        public Int32 Shield { get; set; }
        public Int32 MinSize { get; set; }
        public Texture2D Circle { get; set; }
        public Int32 Multiplier { get; set; }        

        public Player(MainGame mainGame)
            :base(mainGame)
        {
            this.MinSize = 25;
        }

        public Int32 GetCircleWidth()
        {
            return this.Circle.Width;
        }

        public Int32 GetCircleHeight()
        {
            return this.Circle.Height;
        }

        public void Initialize()
        {
            this.Circle = DrawUtils.CreateCircle(Size + MinSize, this._mainGame.GraphicsDevice);
            this.ReInit();
        }

        public void ReInit()
        {
            this.Size = 50;
            this.Shield = 50;
        }

        public void LoadContent()
        {
            
        }

        public void UnloadContent()
        {
            this.Circle.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            this.Circle.Dispose();
            this.Circle = DrawUtils.CreateCircle(Size + MinSize, this._mainGame.GraphicsDevice);
            this.Size = Shield;

            if (!this._mainGame.IsPaused)
            {
                if ((this.Shield < 100) && (this.Shield > 0))
                {
                    Int32 tempMulti = 15;
                    if (Int32.TryParse(this.Shield.ToString().ElementAt(0).ToString(), out tempMulti))
                    {
                        this.Multiplier = tempMulti;
                    }                    
                }
                else
                    this.Multiplier = 15;
            }
        }

        public void Draw(GameTime gameTime)
        {
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            ResourcesComponent rc = this._mainGame.GameManager.GetComponent<ResourcesComponent>();
            this._mainGame.MainSpriteBatch.Draw(rc.PlayerTexture, new Rectangle(this._mainGame.MainViewport.Width / 2 + 2, this._mainGame.MainViewport.Height / 2, 50, 50), Color.White * mp.AvarageFrequency * 1.9f);
            this._mainGame.MainSpriteBatch.Draw(this.Circle, new Vector2(this._mainGame.MainViewport.Width / 2  - this.Shield, this._mainGame.MainViewport.Height / 2 - this.Shield ), Color.DarkMagenta * mp.AvarageFrequency);
        }
    }
}
