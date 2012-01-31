using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard.Particles
{
    class DustParticleSystem : ParticleSystem
    {
        public DustParticleSystem(Game game, ParticleManager particleManager, int howManyEffects) : base(game, particleManager, howManyEffects)
        {
        }

        protected override void InitializeConstants()
        {
            textureFilename = @"Particles\dust";
           
            minInitialSpeed = 20;
            maxInitialSpeed = 200;
            
            minAcceleration = -10;
            maxAcceleration = -50;

            minLifetime = 1.0f;
            maxLifetime = 2.5f;

            minScale = 1.0f;
            maxScale = 2.0f;

            minNumParticles = 10;
            maxNumParticles = 20;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            blendState = BlendState.AlphaBlend;

            DrawOrder = AlphaBlendDrawOrder;
        }
    }
}
