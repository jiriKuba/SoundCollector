using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundCollector.Components;
using SoundCollector.Components.Interfaces;
using SoundCollector.HelpObjects;
using SoundCollector.HelpObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundCollector
{
    class MainGame : Microsoft.Xna.Framework.Game
    {
        public GameManager GameManager { get; private set; }

        public Boolean IsPaused { get; set; }

        public Vector3 MainCameraPosition { get; set; }

        public GraphicsDeviceManager Graphics { get; private set; }

        public SpriteBatch MainSpriteBatch { get; private set; }

        public TimeSpan OneSecond { get; private set; }

        public ISettingService SettingService { get; private set; }

        //has to be without setter and getter => it is struct
        public Viewport MainViewport;

        public const String APPLICATION_ROAMING_FOLDER = "SoundCollector";

        public MainGame()
        {
            //main variables
            this.Graphics = new GraphicsDeviceManager(this);
            this.SettingService = new SettingService();
            //this.MainViewport = this.Graphics.GraphicsDevice.Viewport;

            //is this nessacery?
            this.MainViewport.MaxDepth = 200;
            this.MainViewport.MinDepth = -200;

            //window setting
            this.Content.RootDirectory = "Content";
            this.Window.Title = APPLICATION_ROAMING_FOLDER;
            this.Window.AllowUserResizing = true;
            //this.Graphics.PreferredBackBufferWidth = 1280;
            //this.Graphics.PreferredBackBufferHeight = 720;
            this.Graphics.IsFullScreen = false;
            this.Graphics.ApplyChanges();

            //game settings
            this.IsPaused = true;
            this.MainCameraPosition = new Vector3(0, 0, -200); //start position    
            this.IsMouseVisible = true;
            
            this.GameManager = new GameManager(this);
            this.GameManager.AddComponent(new BackColorer(this));
            this.GameManager.AddComponent(new InputHandler(this));
            this.GameManager.AddComponent(new ResourcesComponent(this));
            this.GameManager.AddComponent(new MusicPlayer(this));
            this.GameManager.AddComponent(new Player(this));
            this.GameManager.AddComponent(new ParticleSystems(this));
            this.GameManager.AddComponent(new Menu(this));            
            this.GameManager.AddComponent(new DynamicWorldCounter(this));            
            this.GameManager.AddComponent(new StatusMaker(this));
            this.GameManager.AddComponent(new GameLogic(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Settings s = this.SettingService.GetSettings();
            this.Graphics.PreferredBackBufferWidth = s.WindowWidth;
            this.Graphics.PreferredBackBufferHeight = s.WindowHeight;
            this.Graphics.IsFullScreen = false;
            this.Graphics.ApplyChanges();

            this.MainViewport = this.Graphics.GraphicsDevice.Viewport;
            this.OneSecond = TimeSpan.FromSeconds(0.1);            
            this.GameManager.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.MainSpriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.GameManager.LoadContent();
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Settings s = this.SettingService.GetSettings();
            s.WindowWidth = this.MainViewport.Width;
            s.WindowHeight = this.MainViewport.Height;
            this.SettingService.SaveSettings();

            this.GameManager.UnloadContent();
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.MainViewport = this.Graphics.GraphicsDevice.Viewport;
            this.GameManager.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.MainSpriteBatch.Begin();
            this.GameManager.Draw(gameTime);
            this.MainSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
