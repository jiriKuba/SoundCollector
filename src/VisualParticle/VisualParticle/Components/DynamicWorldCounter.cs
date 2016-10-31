using SoundCollector.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SoundCollector.Components
{
    class DynamicWorldCounter : Base.BaseGameComponent, IExtendedGameComponent
    {
        public float ZRotationAngle { get; private set; }

        public Int32 DirectionSign { get; private set; }

        /// <summary>
        /// minimal time to next camera change direction
        /// </summary>
        private Int32 _timeToRevers;

        private TimeSpan _prevMusicTime;
        
        public DynamicWorldCounter(MainGame mainGame)
            :base(mainGame)
        {

        }

        public void Draw(GameTime gameTime)
        {

        }

        public void Initialize()
        {
            this.ReInit();
            this.DirectionSign = 1;
        }

        public void ReInit()
        {
            this._timeToRevers = 10;
        }

        public void LoadContent()
        {

        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            if (!this._mainGame.IsPaused)
            {
                MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
                if (this._timeToRevers <= 0)
                {
                    if (Math.Round(mp.AvarageFrequency * 1000, 0) == 521)
                    {
                        this.DirectionSign = this.DirectionSign * (-1);
                        this._timeToRevers = 10;
                    }
                }

                if (mp.AvarageFrequency > 0.41f)
                    this.ZRotationAngle += this.DirectionSign * (mp.AvarageFrequency * 0.020f);

                else
                    this.ZRotationAngle += this.DirectionSign * (mp.AvarageFrequency * 0.008f);

                //this code runs every second
                if (gameTime.TotalGameTime - this._prevMusicTime > mp.MusicTick)
                {                    
                    if (this._timeToRevers > 0)
                        this._timeToRevers--;

                    this._prevMusicTime = gameTime.TotalGameTime;
                }
            }
        }
    }
}
