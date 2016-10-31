#region File Description
//===================================================================
// DefaultTexturedQuadParticleSystemTemplate.cs
// 
// This file provides the template for creating a new Textued Quad Particle
// System that inherits from the Default Textured Quad Particle System.
//
// The spots that should be modified are marked with TODO statements.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using DPSF;
#endregion

namespace SoundCollector.HelpObjects
{
    //-----------------------------------------------------------
    // TODO: Rename/Refactor the Particle System class
    //-----------------------------------------------------------
    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class DefaultTexturedQuadParticleSystemTemplate : DefaultTexturedQuadParticleSystem
    {
        //public float av;

        public float AvarageFrequency { get; set; }

        private readonly Boolean _mulitplyFogColorYWithAvarageFrequency;
        private readonly Boolean _mulitplyFogColorZWithAvarageFrequency;

        //public bool isSqare;
        public float CollectingPower;
        //private Viewport viewport;

        private MagnetPoint mcEmitterPointMagnet;
        public Boolean mbMagnetsAffectPosition;

        private readonly Vector3 _fogColorNormal;
        private readonly Vector3 _fogColorDrive; //when music louder


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public DefaultTexturedQuadParticleSystemTemplate(Game cGame, Vector3 fogColorNormal, Vector3 fogColorDrive, 
            Boolean mulitplyFogColorYWithAvarageFrequency, Boolean mulitplyFogColorZWithAvarageFrequency)
            : base(cGame)
        {
            this.mcEmitterPointMagnet = null;
            this.mbMagnetsAffectPosition = true;

            this._fogColorNormal = fogColorNormal;
            this._fogColorDrive = fogColorDrive;

            this._mulitplyFogColorYWithAvarageFrequency = mulitplyFogColorYWithAvarageFrequency;
            this._mulitplyFogColorZWithAvarageFrequency = mulitplyFogColorZWithAvarageFrequency;
    }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any Particle System properties here
        //-----------------------------------------------------------

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any overridden Particle System functions here
        //-----------------------------------------------------------

        //===========================================================
        // Initialization Functions
        //===========================================================

        /// <summary>
        /// Function to Initialize the Particle System with default values.
        /// Particle system properties should not be set until after this is called, as 
        /// they are likely to be reset to their default values.
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device the Particle System should use</param>
        /// <param name="cContentManager">The Content Manager the Particle System should use to load resources</param>
        /// <param name="cSpriteBatch">The Sprite Batch that the Sprite Particle System should use to draw its particles.
        /// If this is not initializing a Sprite particle system, or you want the particle system to use its own Sprite Batch,
        /// pass in null.</param>
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            //-----------------------------------------------------------
            // TODO: Change any Initialization parameters desired and the Name
            //-----------------------------------------------------------
            // Initialize the Particle System before doing anything else
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 500, 1000,
                                                UpdateVertexProperties, @"star");

            // Set the Name of the Particle System
            this.Name = "My DPSF particle system";
            //viewport = cGraphicsDevice.Viewport;

            // Finish loading the Particle System in a separate function call, so if
            // we want to reset the Particle System later we don't need to completely 
            // re-initialize it, we can just call this function to reset it.

            LoadParticleSystem();
        }

        /// <summary>
        /// Load the Particle System Events and any other settings
        /// </summary>
        public void LoadParticleSystem()
        {

            //-----------------------------------------------------------
            // TODO: Setup the Particle System to achieve the desired result.
            // You may change all of the code in this function. It is just
            // provided to show you how to setup a simple particle system.
            //-----------------------------------------------------------

            // Set the Function to use to Initialize new Particles.
            // The Default Templates include a Particle Initialization Function called
            // InitializeParticleUsingInitialProperties, which initializes new Particles
            // according to the settings in the InitialProperties object (see further below).
            // You can also create your own Particle Initialization Functions as well, as shown with
            // the InitializeParticleProperties function below.
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;
            //ParticleInitializationFunction = InitializeParticleProperties;

            // Setup the Initial Properties of the Particles.
            // These are only applied if using InitializeParticleUsingInitialProperties 
            // as the Particle Initialization Function.
            InitialProperties.LifetimeMin = 5.0f;
            InitialProperties.LifetimeMax = 7.0f;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.VelocityMin = new Vector3(-10, 10, -10);
            InitialProperties.VelocityMax = new Vector3(10, 75, 10);

            InitialProperties.RotationMin = new Vector3(0, 0, 0);
            InitialProperties.RotationMax = new Vector3(0, 0, MathHelper.Pi);
            InitialProperties.RotationalVelocityMin = new Vector3(0, 0, -MathHelper.Pi);
            InitialProperties.RotationalVelocityMax = new Vector3(0, 0, MathHelper.Pi);
            InitialProperties.StartWidthMin = 5;
            InitialProperties.StartWidthMax = 8;
            InitialProperties.StartHeightMin = 5;
            InitialProperties.StartHeightMax = 8;
            InitialProperties.EndWidthMin = 3;
            InitialProperties.EndWidthMax = 3;
            InitialProperties.EndHeightMin = 3;
            InitialProperties.EndHeightMax = 3;
            InitialProperties.StartColorMin = Color.Black;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.Black;
            InitialProperties.EndColorMax = Color.White;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Allow the Particle's Position, Rotation, Width and Height, Color, Transparency, and Orientation to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            //ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            //ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

            // Clear all of the Magnets so they can be re-added
            MagnetList.Clear();


            // This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
            // Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);

            // Set the Particle System's Emitter to toggle on and off every 0.5 seconds
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = 1.0f;
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
            //ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);

            // Add the Magnet Event to the Particle Events
            this.AddMagnetParticleEvent();

            // Setup the Emitter
            //Emitter.ParticlesPerSecond = 50;
            // Emitter.OrientationData.Orientation = new Quaternion(new Vector3(0, 0, 0), 1);


        }

        protected override void SetEffectParameters()
        {
            //-----------------------------------------------------------
            // TODO: Set any global Shader variables required before drawing
            //-----------------------------------------------------------



            AlphaTestEffect effect = this.Effect as AlphaTestEffect;
            if (effect == null) return;

            // Specify the World, View, and Projection Matrices to use, as well as the Texture to use

            effect.World = this.World;
            effect.View = this.View;
            effect.Projection = this.Projection;
            effect.Texture = this.Texture;

            // effect.FogColor = DPSFHelper.RandomColor().ToVector3();
            //if (!isSqare)
            //{
            //    if (av > 0.5)
            //        effect.FogColor = new Vector3(1, av * 1.7f, 1);
            //    else
            //        effect.FogColor = new Vector3(1, av * 1.7f, 0);


            //}
            //else 
            //{
            //    if (av > 0.5)
            //        effect.FogColor = new Vector3(1, 1, av * 1.7f);
            //    else
            //        effect.FogColor = new Vector3(0,0,av * 1.7f);


            //}

            if (this.AvarageFrequency > 0.5)
            {
                effect.FogColor = new Vector3(this._fogColorDrive.X, this._mulitplyFogColorYWithAvarageFrequency ? this._fogColorDrive.Y * this.AvarageFrequency : this._fogColorDrive.Y,
                    this._mulitplyFogColorZWithAvarageFrequency ? this._fogColorDrive.Z * this.AvarageFrequency : this._fogColorDrive.Z);
            }
            else
            { 
                effect.FogColor = new Vector3(this._fogColorNormal.X, this._mulitplyFogColorYWithAvarageFrequency ? this._fogColorNormal.Y * this.AvarageFrequency : this._fogColorNormal.Y,
                    this._mulitplyFogColorZWithAvarageFrequency ? this._fogColorNormal.Z * this.AvarageFrequency : this._fogColorNormal.Z); 
            }

            effect.FogEnabled = true;
            effect.VertexColorEnabled = true;// Enable tinting the texture's color.
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(DefaultTexturedQuadParticle cParticle)
        {
            //-----------------------------------------------------------
            // TODO: Initialize all of the Particle's properties here.
            // If you plan on simply using the default InitializeParticleUsingInitialProperties
            // Particle Initialization Function (see the LoadParticleSystem() function above), 
            // then you may delete this function all together.
            //-----------------------------------------------------------

            // Set the Particle's Lifetime (how long it should exist for)
            cParticle.Lifetime = 2.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;


            // Set the Particle's Velocity
            // Vector3 sVelocityMin = new Vector3(-100, 100, -100);
            //  Vector3 sVelocityMax = new Vector3(250, 300, 250);
            Vector3 sVelocityMin = new Vector3(-100, 100, -100);
            Vector3 sVelocityMax = new Vector3(150, 200, 150);
            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(sVelocityMin, sVelocityMax);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);


            // Give the Particle a random Size
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Width = cParticle.StartWidth = cParticle.EndWidth =
                cParticle.Height = cParticle.StartHeight = cParticle.EndHeight = RandomNumber.Next(10, 50);

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            cParticle.Color = cParticle.StartColor = cParticle.EndColor = DPSFHelper.RandomColor();
            // cParticle.Color = cParticle.StartColor = cParticle.EndColor = Color.Green;
        }


        //===========================================================
        // Particle Update Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place your Particle Update functions here, using the 
        // same function prototype as below (i.e. public void FunctionName(DPSFParticle, float))
        //-----------------------------------------------------------

        /// <summary>
        /// Example of how to create a Particle Event Function
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateParticleFunctionExample(DefaultTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle here
            // Example: cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
            //cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
            // Effects expect a premultiplied color, so get the actual color to use.

        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place your Particle System Update functions here, using 
        // the same function prototype as below (i.e. public void FunctionName(float))
        //-----------------------------------------------------------

        /// <summary>
        /// Example of how to create a Particle System Event Function
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateParticleSystemFunctionExample(float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle System here
            // Example: Emitter.EmitParticles = true;
            // Example: SetTexture("TextureAssetName");

        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any other functions here
        //-----------------------------------------------------------

        /// <summary>
        /// Update the Emitter Magnet's Position to the Emitter's Position
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateEmitterMagnetToTheEmittersPosition(float fElapsedTimeInSeconds)
        {
            if (mcEmitterPointMagnet != null)
            {
                mcEmitterPointMagnet.PositionData.Position = Emitter.PositionData.Position;
            }
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================
        public void LoadSeparateEmitterMagnetsParticleSystem()
        {
            LoadParticleSystem();

            // Remove the unnecessary event that was added in the LoadEmitterMagnetParticleSytem() function
            ParticleSystemEvents.RemoveEveryTimeEvent(UpdateEmitterMagnetToTheEmittersPosition, 0, 0);

            // Clear the Magnets List
            MagnetList.Clear();

            // Add two Point Magnets
            MagnetList.Add(new MagnetPoint(new Vector3(100, 50, 0),
                                    DefaultParticleSystemMagnet.MagnetModes.Attract,
                                    DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse,
                                    0, 100, 20, 0));
            MagnetList.Add(new MagnetPoint(new Vector3(-100, 50, 0),
                                    DefaultParticleSystemMagnet.MagnetModes.Repel,
                                    DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse,
                                    0, 100, 20, 0));
        }

        public void MouseMagnet(Vector3 mys)
        {
            //LoadParticleSystem();
            MagnetList.Clear();
            // Remove the unnecessary event that was added in the LoadEmitterMagnetParticleSytem() function
            ParticleSystemEvents.RemoveEveryTimeEvent(UpdateEmitterMagnetToTheEmittersPosition, 0, 0);

            // Clear the Magnets List
            //MagnetList.Clear();

            // Add two Point Magnets

            MagnetList.Add(new MagnetPoint(mys,
                                    DefaultParticleSystemMagnet.MagnetModes.Attract,
                                    DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse,
                                    0, 25 + this.CollectingPower, 2 + this.CollectingPower, 0));




        }

        public void BonusLux(Vector3 mys)
        {
            LoadParticleSystem();

            // Remove the unnecessary event that was added in the LoadEmitterMagnetParticleSytem() function
            ParticleSystemEvents.RemoveEveryTimeEvent(UpdateEmitterMagnetToTheEmittersPosition, 0, 0);

            // Clear the Magnets List
            MagnetList.Clear();

            // Add two Point Magnets

            MagnetList.Add(new MagnetPoint(mys,
                                    DefaultParticleSystemMagnet.MagnetModes.Attract,
                                    DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse,
                                    0, 100, 20, 0));




        }
        public void BonusShield(Vector3 mys)
        {
            LoadParticleSystem();

            // Remove the unnecessary event that was added in the LoadEmitterMagnetParticleSytem() function
            ParticleSystemEvents.RemoveEveryTimeEvent(UpdateEmitterMagnetToTheEmittersPosition, 0, 0);

            // Clear the Magnets List
            MagnetList.Clear();

            // Add two Point Magnets
            MagnetList.Add(new MagnetPoint(mys,
                                    DefaultParticleSystemMagnet.MagnetModes.Attract,
                                    DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse,
                                    0, 25 + this.CollectingPower, 2 + this.CollectingPower, 0));

            MagnetList.Add(new MagnetPoint(new Vector3(0, 0, 0),
                                                    DefaultParticleSystemMagnet.MagnetModes.Repel,
                                                    DefaultParticleSystemMagnet.DistanceFunctions.Cubed,
                                                    0, 50, 15, 0));




        }


        public void ToggleMagnetsAffectingPositionVsVelocity()
        {
            // Toggle what should be affected by the Magnets
            mbMagnetsAffectPosition = !mbMagnetsAffectPosition;

            // Use the proper Particle Events
            AddMagnetParticleEvent();
        }

        public void AddMagnetParticleEvent()
        {
            // Remove the Magnet Particle Events
            ParticleEvents.RemoveAllEventsInGroup(1);

            // If the Magnets should affect Position
            if (mbMagnetsAffectPosition)
            {
                ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAccordingToMagnets, 500, 1);
            }
            // Else the Magnets should affect Velocity
            else
            {
                ParticleEvents.AddEveryTimeEvent(UpdateParticleVelocityAccordingToMagnets, 500, 1);
            }
        }
    }
}
