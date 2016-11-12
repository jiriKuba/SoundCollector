using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DPSF;
//using DPSF.ParticleSystems;
using SoundCollector.Components;
using SoundCollector.Utils;
using SoundCollector.Components.Interfaces;

namespace SoundCollector.Components
{
    /// <summary>
    /// Simple game manager
    /// </summary>
    class GameManager : Base.BaseGameComponent, IExtendedGameComponent
    {
        private readonly List<IExtendedGameComponent> _components;

        public Boolean AreAnyComponents
        {
            get
            {
                return this._components != null && this._components.Count > 0;
            }
        }

        public GameManager(MainGame mainGame)
            :base(mainGame)
        {
            this._components = new List<IExtendedGameComponent>();            
        }

        public void AddComponent(IExtendedGameComponent c)
        {
            this._components.Add(c);
        }

        public void Initialize()
        {
            if (this.AreAnyComponents)
            {
                foreach (IExtendedGameComponent c in this._components)
                {
                    c.Initialize();
                }
            }
        }

        public void LoadContent()
        {
            if (this.AreAnyComponents)
            {
                foreach (IExtendedGameComponent c in this._components)
                {
                    c.LoadContent();
                }
            }
        }

        public void UnloadContent()
        {
            if (this.AreAnyComponents)
            {
                foreach (IExtendedGameComponent c in this._components)
                {
                    c.UnloadContent();
                }
                this._components.Clear();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (this.AreAnyComponents)
            {
                foreach (IExtendedGameComponent c in this._components)
                {
                    c.Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (this.AreAnyComponents)
            {
                foreach (IExtendedGameComponent c in this._components)
                {
                    c.Draw(gameTime);
                }
            }
        }

        public T GetComponent<T>() where T : IExtendedGameComponent
        {
            if (this.AreAnyComponents)
            {
                foreach (IExtendedGameComponent c in this._components)
                {
                    if (c is T)
                        return (T)c;
                }
            }

            return default(T);
        }
    }
}