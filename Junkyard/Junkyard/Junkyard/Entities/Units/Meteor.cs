using System;
using Microsoft.Xna.Framework;

namespace Junkyard.Entities.Units
{
    internal class Meteor : AnimatedBattleUnit
    {
        #region Constants

        private const float MaxOffset = 2.0f;

        #endregion
        #region Private fields

        private static readonly Random random = new Random();
        protected float direction;
        protected bool dying = false;

        #endregion
        #region Properties

        public Vector3 Acceleration { get; set; }
        public float Range { get; set; }
        public Vector3 Velocity { get; set; }

        #endregion
        #region Ctors

        public Meteor()
        {
            Velocity = Vector3.Zero;

            // we don't want it to be treated as a battle unit
            Stealth = true;
        }

        public Meteor(Simulation simulation, Player player)
            : this()
        {
            Simulation = simulation;
            Player = player;
        }

        #endregion
        #region Overrides

        public override void OnSpawn()
        {
            AssertAnimation("fly");
            Avatar.Position += ((float) random.NextDouble() - 0.5f)*MaxOffset*Vector3.UnitX;
        }

        public override void OnTick(GameTime time)
        {
            AssertFacingTowardsFoe();

            float bottom = Avatar.Position.Y + Avatar.PixelScale.Y*Avatar.TexRect.Height;
            if (!dying && bottom <= Simulation.GroundLevel)
            {
                dying = true;
                BattleUnit foe = Simulation.GetNearestFoe(this);
                if (foe != null)
                {
                    float dist = Math.Abs(foe.Avatar.Position.X - Avatar.Position.X);
                    if (dist <= 2*Range)
                    {
                        Simulation.Attack(foe, this);
                        Simulation.Attack(foe, this);
                    }
                }
            }

            if (dying)
            {
                AssertAnimation("dying");
                if (AnimationEnded)
                {
                    reallyDead = true;
                }
            }
            else
            {
                AssertAnimation("fly");
                if (AnimationEnded)
                    ResetAnimation();

                float elapsedTime = time.ElapsedGameTime.Milliseconds/1000.0f;
                Velocity += elapsedTime*Acceleration;
                Avatar.Position += elapsedTime*new Vector3(direction*Velocity.X, Velocity.Y, Velocity.Z);
            }

            base.OnTick(time);
        }

        #endregion
        #region Protected methods

        protected void AssertFacingTowardsFoe()
        {
            direction = Player.Direction;
            Avatar.Flipped = (direction < 0);
        }

        #endregion
    }
}