using SoundCollector.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SoundCollector.Components
{
    class InputHandler : Base.BaseGameComponent, IExtendedGameComponent
    {    
        public KeyboardState ActualKeyboardState { get; private set; }
        public KeyboardState PreviouslyKeyboardState { get; private set; }
        public MouseState ActualMouseState { get; private set; }

        //public Vector2 NonBlockMousePosition { get; private set; }
        public Vector2 OldMousePosition { get; private set; }

        public event EventHandler<EventArgs> EscapePressed;
        public event EventHandler<EventArgs> SpacePressed;
        public event EventHandler<EventArgs> GamePadBackPressed;

        public InputHandler(MainGame mainGame)
            :base(mainGame)
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

        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            if (this._mainGame.IsActive)
            {
                this.ActualKeyboardState = Keyboard.GetState();
                this.ActualMouseState = Mouse.GetState();
            }

            if ((this.ActualKeyboardState.IsKeyDown(Keys.Escape)) && (this.PreviouslyKeyboardState != this.ActualKeyboardState))
            {
                if (this.EscapePressed != null)
                    this.EscapePressed.Invoke(null, EventArgs.Empty);

                this.PreviouslyKeyboardState = this.ActualKeyboardState;
            }
            if (this.ActualKeyboardState.GetPressedKeys().Count() == 0)
                this.PreviouslyKeyboardState = Keyboard.GetState();

            if (!this._mainGame.IsPaused)
            {

                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    if (this.GamePadBackPressed != null)
                        this.GamePadBackPressed.Invoke(null, EventArgs.Empty);
                }

                //next song
                if ((this.ActualKeyboardState.IsKeyDown(Keys.Space)) && (this.PreviouslyKeyboardState != this.ActualKeyboardState))
                {
                    if (this.SpacePressed != null)
                        this.SpacePressed.Invoke(null, EventArgs.Empty);

                    this.PreviouslyKeyboardState = this.ActualKeyboardState;
                }
                if (this.ActualKeyboardState.IsKeyUp(Keys.Space))
                {
                    this.PreviouslyKeyboardState = Keyboard.GetState();
                }
            }
        }
    }
}
