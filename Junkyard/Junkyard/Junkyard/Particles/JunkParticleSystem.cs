using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Junkyard.Particles
{
    class JunkParticleSystem : ParticleSystem
    {
        public JunkParticleSystem(Game game, ParticleManager particleManager, int howManyEffects) : base(game, particleManager, howManyEffects)
        {
        }

        protected override void InitializeConstants()
        {
            
        }
    }
}
