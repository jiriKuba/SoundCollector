using SoundCollector.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SoundCollector.Components
{
    class BackColorer : Base.BaseGameComponent, IExtendedGameComponent
    {
        private Color _backColor;
        
        public BackColorer(MainGame mainGame)
            :base(mainGame)
        {

        }

        public void Draw(GameTime gameTime)
        {
            this._mainGame.GraphicsDevice.Clear(this._backColor);
        }

        public void Initialize()
        {
            this._backColor = Color.Black;
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
                float colorParameter = 0f;
                if (mp.AvarageFrequency > 0.5f)
                {
                    colorParameter = mp.AvarageFrequency * 0.15f;
                    this._backColor = new Color(mp.AvarageFrequency * 0.15f, 0, 0.5f * mp.AvarageFrequency, 0);
                }
                else
                {
                    if (colorParameter > 0)
                        colorParameter -= 0.01f;
                    this._backColor = new Color(colorParameter, 0, 0.5f * mp.AvarageFrequency, 0);
                }
            }
        }
    }
}
