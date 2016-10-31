using SoundCollector.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SoundCollector.Components
{
    class ResourcesComponent : Base.BaseGameComponent, IExtendedGameComponent
    {
        public Texture2D MenuBackground { get; private set; }

        public Texture2D PlayerTexture { get; private set; }

        public SpriteFont ScoreFont { get; private set; }

        public SpriteFont BiggerFont { get; private set; }

        public SpriteFont SmallerFont { get; private set; }

        public SpriteFont CzechFont { get; private set; }

        public ResourcesComponent(MainGame mainGame)
            : base(mainGame)
        {
        }

        public void Draw(GameTime gameTime)
        {

        }

        public void Initialize()
        {

        }

        public void LoadContent()
        {
            this.MenuBackground = this._mainGame.Content.Load<Texture2D>(@"blank");
            this.ScoreFont = this._mainGame.Content.Load<SpriteFont>(@"FontScore");
            this.BiggerFont = this._mainGame.Content.Load<SpriteFont>(@"Bigger");
            this.SmallerFont = this._mainGame.Content.Load<SpriteFont>(@"Smaller");
            this.CzechFont = this._mainGame.Content.Load<SpriteFont>(@"SongList");
            this.PlayerTexture = this._mainGame.Content.Load<Texture2D>(@"world");
        }

        public void UnloadContent()
        {
            this.MenuBackground.Dispose();
            this.PlayerTexture.Dispose();
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
