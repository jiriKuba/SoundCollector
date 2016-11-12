using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SoundCollector.Components.Interfaces;

namespace SoundCollector.Components
{
    class StatusMaker : Base.BaseGameComponent, IExtendedGameComponent
    {
        public String Status { get; private set; }
        public Int32 StatusShowTime { get; private set; }

        private Color _statusColor;
        private TimeSpan _prevMusicTime;

        public const String WELCOME_STATUS_TEXT = "Welcome";
        public const String UNFINISHED_STATUS_TEXT = "Unfinished";
        public const String GOD_STATUS_TEXT = "GOD";
        public const String EXCELENT_STATUS_TEXT = "Excelent";
        public const String OK_STATUS_TEXT = "OK";
        public const String EHM_STATUS_TEXT = "Ehm...";
        public const String VERY_POOR_STATUS_TEXT = "Very poor!";
        public const String NOTHING_STATUS_TEXT = "Nothing?!";
        public const Int32 STATUS_SHOW_TIME_VALUE = 5;

        public StatusMaker(MainGame mainGame)
            : base(mainGame)
        {

        }

        public void GenerateStatus(Int32 shield, Int32 catchedParticles, Int32 allParticlesCount, Boolean isSongfinished)
        {
            this.StatusShowTime = STATUS_SHOW_TIME_VALUE;
            if (shield == -1)
            {
                this.Status = WELCOME_STATUS_TEXT;
                this._statusColor = Color.GreenYellow;
            }
            else
            {
                if (!isSongfinished)
                {
                    Status = UNFINISHED_STATUS_TEXT;
                    this._statusColor = Color.WhiteSmoke;
                }
                else
                {
                    if ((shield > 0) && (catchedParticles > 0) && (allParticlesCount > 0))
                    {
                        float statusGeneratedValue = (((float)(shield) + (float)(catchedParticles)) / (float)(allParticlesCount) * 100);
                        Int32 statusGeneratedValueInt = (Int32)statusGeneratedValue;

                        if (statusGeneratedValueInt >= 48)
                        {
                            this.Status = GOD_STATUS_TEXT;
                            this._statusColor = Color.Red;
                        }
                        else if (statusGeneratedValueInt >= 40)
                        {
                            this.Status = EXCELENT_STATUS_TEXT;
                            this._statusColor = Color.Gold;
                        }
                        else if (statusGeneratedValueInt >= 30)
                        {
                            this.Status = OK_STATUS_TEXT;
                            this._statusColor = Color.Silver;
                        }
                        else if (statusGeneratedValueInt >= 20)
                        {
                            this.Status = EHM_STATUS_TEXT;
                            this._statusColor = Color.Coral;
                        }
                        else if (statusGeneratedValueInt >= 10)
                        {
                            this.Status = VERY_POOR_STATUS_TEXT;
                            this._statusColor = Color.Brown;
                        }
                    }
                    else
                    {
                        this.Status = NOTHING_STATUS_TEXT;
                        this._statusColor = Color.White;
                    }
                }
            }
        }

        public void Initialize()
        {

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
                if (gameTime.TotalGameTime - this._prevMusicTime > mp.MusicTick)
                {
                    if (this.StatusShowTime > 0)
                        this.StatusShowTime--;

                    this._prevMusicTime = gameTime.TotalGameTime;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (this.StatusShowTime > 0)
            {
                ResourcesComponent rc = this._mainGame.GameManager.GetComponent<ResourcesComponent>();
                this._mainGame.MainSpriteBatch.DrawString(rc.BiggerFont, this.Status, new Vector2(this._mainGame.MainViewport.Width / 2 - this._mainGame.MainViewport.Width * 0.04f, 0), this._statusColor);
            }
        }
    }
}
