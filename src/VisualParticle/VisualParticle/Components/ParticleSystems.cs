using SoundCollector.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundCollector.Utils;
using Microsoft.Xna.Framework.Input;
using DPSF;
using SoundCollector.HelpObjects;

namespace SoundCollector.Components
{
    class ParticleSystems : Base.BaseGameComponent, IExtendedGameComponent
    {
        private DefaultTexturedQuadParticleSystemTemplate _mcParticleSystem;
        private DefaultTexturedQuadParticleSystemTemplate _mcParticleSystemOterColor;

        public Vector3 EmitterRealPos1 { get; private set; }
        public Vector3 EmitterRealPos2OterColor { get; private set; }

        public Texture2D OutsideCircle { get; private set; }

        public Boolean BonusFieldActive { get; set; }

        public Boolean BonusFieldPrepar { get; set; }

        public Int32 BonusPower { get; private set;}

        public event EventHandler<EventArgs> BonusFieldDisabled;
        public event EventHandler<EventArgs> ParticleHitMouse;
        public event EventHandler<EventArgs> ParticleHitPlayer;
        public event EventHandler<EventArgs> ParticleOutOfScreen;

        private Int32 _bannedOutsideCircleArea;

        private Vector3 _mouse3d;

        private Vector2 _activeMousePosition;
        private Vector2 _oldActiveMousePosition;
        private TimeSpan _previouslyGameTime;
        private Boolean _bonusLuxActive;
        private float _zRotationOfEmitter;
        private Int32 _particleDeflection;
        private const float PARTICLE_SPACE = 2.4f;
        private const float PARTICLE_DEAD_TIME = 0.0000001f;
        private const Int32 PARTICLE_OUTSIDE_AREA = 20;

        private const Int32 EMITTER_DISTANCE = 55;

        public ParticleSystems(MainGame mainGame)
            :base(mainGame)
        {
            this._bonusLuxActive = false;
        }

        public void Draw(GameTime gameTime)
        {
            MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
            
            if (this.OutsideCircle != null)
            {
                if (mp.AvarageFrequency < 0.5f)
                {
                    this._mainGame.MainSpriteBatch.Draw(this.OutsideCircle, new Vector2(this.EmitterRealPos1.X - this._bannedOutsideCircleArea, this.EmitterRealPos1.Y - this._bannedOutsideCircleArea), Color.Black);
                    this._mainGame.MainSpriteBatch.Draw(this.OutsideCircle, new Vector2(this.EmitterRealPos2OterColor.X - this._bannedOutsideCircleArea, this.EmitterRealPos2OterColor.Y - this._bannedOutsideCircleArea), Color.Black);
                }
                else
                {
                    this._mainGame.MainSpriteBatch.Draw(this.OutsideCircle, new Vector2(this.EmitterRealPos1.X - this._bannedOutsideCircleArea, this.EmitterRealPos1.Y - this._bannedOutsideCircleArea), Color.Purple);
                    this._mainGame.MainSpriteBatch.Draw(this.OutsideCircle, new Vector2(this.EmitterRealPos2OterColor.X - this._bannedOutsideCircleArea, this.EmitterRealPos2OterColor.Y - this._bannedOutsideCircleArea), Color.Purple);
                }
            }

            this._mcParticleSystem.Draw();
            this._mcParticleSystemOterColor.Draw();
        }

        public void Initialize()
        {
            this._particleDeflection = 1;
        }

        public void ReInit()
        {
            this.BonusPower = 0;

            this.BonusFieldActive = false;
            this.BonusFieldPrepar = false;
            this._mcParticleSystem.SetTexture(this.RandomParticleTextureName());
            this._mcParticleSystemOterColor.SetTexture(this.RandomParticleTextureName());

            this._mcParticleSystem.ActiveParticles.Clear();
            this._mcParticleSystemOterColor.ActiveParticles.Clear();
        }

        public void LoadContent()
        {
            this._mcParticleSystem = new DefaultTexturedQuadParticleSystemTemplate(this._mainGame, new Vector3(1f, 1.7f, 0), new Vector3(1f, 1.7f, 1f), true, false);
            this._mcParticleSystem.AutoInitialize(this._mainGame.GraphicsDevice, this._mainGame.Content, null);

            this._mcParticleSystemOterColor = new DefaultTexturedQuadParticleSystemTemplate(this._mainGame, new Vector3(0, 0, 1.7f), new Vector3(1, 1, 1.7f), false, true);
            this._mcParticleSystemOterColor.AutoInitialize(this._mainGame.GraphicsDevice, this._mainGame.Content, null);
        }

        public void UnloadContent()
        {
            this._mcParticleSystem.Destroy();
            this._mcParticleSystemOterColor.Destroy();

            if (this.OutsideCircle != null)
                this.OutsideCircle.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            if (!this._mainGame.IsActive)
            {
                this._mcParticleSystemOterColor.Game.ResetElapsedTime();
                this._mcParticleSystem.Game.ResetElapsedTime();
            }

            if (!this._mainGame.IsPaused)
            {
                MusicPlayer mp = this._mainGame.GameManager.GetComponent<MusicPlayer>();
                InputHandler ih = this._mainGame.GameManager.GetComponent<InputHandler>();

                this._bannedOutsideCircleArea = (Int32)(Math.Exp(mp.AvarageFrequency * 8.4f));

                if (this.OutsideCircle != null)
                    this.OutsideCircle.Dispose();

                this.OutsideCircle = DrawUtils.CreateCircle(this._bannedOutsideCircleArea, this._mainGame.GraphicsDevice);

                DynamicWorldCounter dwc = this._mainGame.GameManager.GetComponent<DynamicWorldCounter>();

                Matrix rot = Matrix.CreateRotationZ(dwc.ZRotationAngle);
                Vector3 vec3 = Vector3.Transform(Vector3.Up, rot);
                vec3.Normalize();

                Matrix sViewMatrix = Matrix.CreateLookAt(this._mainGame.MainCameraPosition, Vector3.Zero, vec3);

                // Setup the Camera's Projection matrix by specifying the field of view (1/4 pi), aspect ratio, and the near and far clipping planes
                //Matrix sProjectionMatrix = Matrix.CreateOrthographic(this._mainGame.GraphicsDevice.Viewport.Width, this._mainGame.GraphicsDevice.Viewport.Height, 1, 10000);//
                Matrix sProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this._mainGame.GraphicsDevice.Viewport.Width / (float)this._mainGame.GraphicsDevice.Viewport.Height, 1, 10000);

                // Set the Particle System's World, View, and Projection matrices so that it knows how to draw the particles properly.
                this._mcParticleSystem.SetWorldViewProjectionMatrices(Matrix.Identity, sViewMatrix, sProjectionMatrix);

                // Update the Particle System for craft
                this._mcParticleSystem.SetCameraPosition(this._mainGame.MainCameraPosition);
                this._mcParticleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                // Set the Particle System's World, View, and Projection matrices so that it knows how to draw the particles properly.
                this._mcParticleSystemOterColor.SetWorldViewProjectionMatrices(Matrix.Identity, sViewMatrix, sProjectionMatrix);


                // Update the Particle System
                this._mcParticleSystemOterColor.SetCameraPosition(this._mainGame.MainCameraPosition);
                this._mcParticleSystemOterColor.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                if ((!new Rectangle((Int32)(this.EmitterRealPos1.X - this._bannedOutsideCircleArea), (Int32)(this.EmitterRealPos1.Y - this._bannedOutsideCircleArea),
                    this.OutsideCircle.Width, this.OutsideCircle.Height).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1)))
                && (!new Rectangle((Int32)(this.EmitterRealPos2OterColor.X - this._bannedOutsideCircleArea), (Int32)(this.EmitterRealPos2OterColor.Y - this._bannedOutsideCircleArea),
                this.OutsideCircle.Width, this.OutsideCircle.Height).Contains(new Rectangle(ih.ActualMouseState.X, ih.ActualMouseState.Y, 1, 1))))
                {
                    this._activeMousePosition = new Vector2(ih.ActualMouseState.X, ih.ActualMouseState.Y);
                }

                this._mouse3d = this._mainGame.MainViewport.Unproject(new Vector3(this._activeMousePosition.X, this._activeMousePosition.Y, 0),  this._mcParticleSystem.Projection, this._mcParticleSystem.View, this._mcParticleSystem.World);
                this._mouse3d = new Vector3(this._mouse3d.X, this._mouse3d.Y, 0) * this._mouse3d.Z * (-1);//don't know why but it works!?

                this._mcParticleSystem.MouseMagnet(this._mouse3d);
                this._mcParticleSystemOterColor.MouseMagnet(this._mouse3d);
                
                //mouse movement to particle lux power
                if (gameTime.TotalGameTime - this._previouslyGameTime > this._mainGame.OneSecond)
                {                    
                    if ((this._oldActiveMousePosition.X != -10) && (this._activeMousePosition.X != -10))
                    {
                        this._mcParticleSystem.CollectingPower = Vector2.Distance(this._activeMousePosition, this._oldActiveMousePosition) * 0.1f;
                        this._mcParticleSystemOterColor.CollectingPower = this._mcParticleSystem.CollectingPower;//Vector2.Distance(this._activeMousePosition, this._oldActiveMousePosition) * 0.1f;
                    }

                    if (this._mcParticleSystem.CollectingPower > 13)
                    {
                        this._mcParticleSystem.CollectingPower = 13;
                        this._mcParticleSystemOterColor.CollectingPower = 13;
                        this.BonusPower++;
                        if (this.BonusPower >= 100)
                        {
                            this.BonusPower = 100;
                        }
                    }

                    this._oldActiveMousePosition = this._activeMousePosition;
                    this._previouslyGameTime = gameTime.TotalGameTime;
                }
                
                if ((ih.ActualMouseState.LeftButton == ButtonState.Pressed) && (this.BonusPower >= 100))
                {
                    this._bonusLuxActive = true;
                }

                if ((ih.ActualMouseState.RightButton == ButtonState.Pressed) && (this.BonusFieldPrepar))
                {
                    this._mcParticleSystem.BonusShield(this._mouse3d);
                    this._mcParticleSystemOterColor.BonusShield(this._mouse3d);
                    this.BonusFieldActive = true;
                }

                if ((ih.ActualMouseState.RightButton == ButtonState.Released) && (this.BonusFieldActive))
                {
                    //bonusTime = 5;
                    //timeToBonus = 35;
                    this.BonusFieldPrepar = false;
                    this.BonusFieldActive = false;
                    if (this.BonusFieldDisabled != null)
                        this.BonusFieldDisabled.Invoke(null, EventArgs.Empty);
                }

                if (this._bonusLuxActive)
                {
                    this._mcParticleSystem.BonusLux(this._mouse3d);
                    this._mcParticleSystemOterColor.BonusLux(this._mouse3d);
                    this.BonusPower--;
                }

                if (this.BonusPower <= 0)
                {
                    this._bonusLuxActive = false;
                }

                //emitters position
                this._mcParticleSystem.Emitter.PositionData.Position = new Vector3((-1) * EMITTER_DISTANCE, EMITTER_DISTANCE, 0);
                this.EmitterRealPos1 = this._mainGame.MainViewport.Project(this._mcParticleSystem.Emitter.PositionData.Position, this._mcParticleSystem.Projection, this._mcParticleSystem.View, this._mcParticleSystem.World);

                this._mcParticleSystemOterColor.Emitter.PositionData.Position = new Vector3(EMITTER_DISTANCE, (-1) * EMITTER_DISTANCE, 0);
                this.EmitterRealPos2OterColor = this._mainGame.MainViewport.Project(this._mcParticleSystemOterColor.Emitter.PositionData.Position, this._mcParticleSystem.Projection, this._mcParticleSystem.View, this._mcParticleSystem.World);

                if (this._zRotationOfEmitter >= 360)
                    this._zRotationOfEmitter = 0f;

                this._zRotationOfEmitter += 0.1f;
                //emitters rotation
                this._mcParticleSystem.Emitter.OrientationData.Rotate(Matrix.CreateRotationZ(MathHelper.ToRadians(this._zRotationOfEmitter)));
                this._mcParticleSystemOterColor.Emitter.OrientationData.Rotate(Matrix.CreateRotationZ(MathHelper.ToRadians(-this._zRotationOfEmitter)));

                //particles per second
                this._mcParticleSystem.Emitter.ParticlesPerSecond = mp.AvarageFrequency * 60;
                this._mcParticleSystem.SimulationSpeed = mp.AvarageFrequency * 2;
                this._mcParticleSystemOterColor.SimulationSpeed = mp.AvarageFrequency * 2;
                this._mcParticleSystemOterColor.Emitter.ParticlesPerSecond = mp.AvarageFrequency * 60;


                //if sound avarage bigger than 0.5 it makes boom effect
                if (mp.AvarageFrequency > 0.5f)
                {
                    this._particleDeflection = this._particleDeflection * (-1);
                    this._mcParticleSystem.AddParticles((Int32)(8 * mp.AvarageFrequency));

                    this._mcParticleSystemOterColor.AddParticles((Int32)(8 * mp.AvarageFrequency));
                }

                //sound avarage to particle system
                this._mcParticleSystem.AvarageFrequency = mp.AvarageFrequency;
                this._mcParticleSystemOterColor.AvarageFrequency = mp.AvarageFrequency;

                this.ParticleForEach(this._mcParticleSystem, mp.AvarageFrequency);
                this.ParticleForEach(this._mcParticleSystemOterColor, mp.AvarageFrequency);
            }
        }

        public void PauseParticles(Boolean isPaused)
        {
            this._mcParticleSystem.Enabled = !isPaused;
            this._mcParticleSystemOterColor.Enabled = !isPaused;
        }

        private String RandomParticleTextureName()
        {
            Random random = new Random();
            Int32 num = random.Next(9);
            switch (num)
            {
                case 0:
                    return "cir";
                case 1:
                    return "x";
                case 2:
                    return "star";
                case 3:
                    return "sqe";
                case 4:
                    return "ball";
                case 5:
                    return "kolecko";
                case 6:
                    return "tangle";
                case 7:
                    return "flow";
                case 8:
                    return "ctverec";
                default:
                    return "star";
            }
        }

        private void ParticleForEach(DefaultTexturedQuadParticleSystemTemplate localParticleSystem, float avarage)
        {
            Player player = this._mainGame.GameManager.GetComponent<Player>();
            if(player != null && localParticleSystem != null && localParticleSystem.ActiveParticles != null && localParticleSystem.ActiveParticles.Count > 0)
            {
                foreach (DefaultQuadParticle particle in localParticleSystem.ActiveParticles)
                {
                    particle.Size = avarage * 15f;

                    Vector3 paticlePosition = this._mainGame.MainViewport.Project(particle.Position, localParticleSystem.Projection, localParticleSystem.View, localParticleSystem.World);

                    //shield
                    if ((new Rectangle((int)(this._mainGame.MainViewport.Width / 2 - player.Shield), (int)(this._mainGame.MainViewport.Height / 2 - player.Shield), player.GetCircleWidth(), player.GetCircleHeight()).Contains(new Rectangle((int)(paticlePosition.X - 12), (int)(paticlePosition.Y - 12), (int)(12), (int)(12)))) ||
                    (new Rectangle((int)(this._mainGame.MainViewport.Width / 2 - player.Shield), (int)(this._mainGame.MainViewport.Height / 2 - player.Shield), player.GetCircleWidth(), player.GetCircleHeight()).Contains(new Rectangle((int)(paticlePosition.X + 12), (int)(paticlePosition.Y + 12), (int)(12), (int)(12)))))
                    {
                        particle.Lifetime = PARTICLE_DEAD_TIME;

                        if (this.ParticleHitPlayer != null)
                            this.ParticleHitPlayer.Invoke(player, EventArgs.Empty);
                    }

                    if ((paticlePosition.X > this._mainGame.MainViewport.Width + PARTICLE_OUTSIDE_AREA) || (paticlePosition.X < (-1) * PARTICLE_OUTSIDE_AREA))
                    {
                        particle.Lifetime = PARTICLE_DEAD_TIME;
                        //particlesPerGame++;
                        if (this.ParticleOutOfScreen != null)
                            this.ParticleOutOfScreen.Invoke(null, EventArgs.Empty);

                    }

                    if ((paticlePosition.Y > this._mainGame.MainViewport.Height + PARTICLE_OUTSIDE_AREA) || (paticlePosition.Y < (-1) * PARTICLE_OUTSIDE_AREA))
                    {
                        particle.Lifetime = PARTICLE_DEAD_TIME;
                        //particlesPerGame++;
                        if (this.ParticleOutOfScreen != null)
                            this.ParticleOutOfScreen.Invoke(null, EventArgs.Empty);
                    }

                    //projection paticle on canvas
                    if (particle.Visible)
                    {
                        if ((this._activeMousePosition.X >= paticlePosition.X - particle.Width * PARTICLE_SPACE) && (this._activeMousePosition.X <= paticlePosition.X + particle.Width * PARTICLE_SPACE))
                        {
                            if ((this._activeMousePosition.Y <= paticlePosition.Y + particle.Height * PARTICLE_SPACE) && (this._activeMousePosition.Y >= paticlePosition.Y - particle.Height * PARTICLE_SPACE))
                            {
                                particle.Visible = false;
                                particle.Lifetime = PARTICLE_DEAD_TIME;

                                if (this.ParticleHitMouse != null)
                                    this.ParticleHitMouse.Invoke(player, EventArgs.Empty);
                            }
                        }
                    }
                }
            }
        }
    }
}