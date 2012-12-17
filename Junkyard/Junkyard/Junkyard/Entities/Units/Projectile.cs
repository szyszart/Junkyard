using System;
using Microsoft.Xna.Framework;

namespace Junkyard.Entities.Units
{
    internal class Projectile : AnimatedBattleUnit
    {
        #region Delegates

        internal delegate void ProjectileHitHandler(Projectile sender);

        #endregion
        #region Private fields

        protected bool active = true;
        protected float direction = 1.0f;

        #endregion
        #region Properties

        public float Gravity { get; set; }
        public BattleUnit Target { get; set; }
        public Vector3 Velocity { get; set; }

        #endregion
        #region Events

        public event ProjectileHitHandler ProjectileHit;

        #endregion
        #region Ctors

        public Projectile()
        {
            Gravity = 10.0f;
            Velocity = Vector3.Zero;

            // we don't want it to be treated as a battle unit
            Stealth = true;
        }

        public Projectile(Simulation simulation, Player player)
            : this()
        {
            Simulation = simulation;
            Player = player;
        }

        #endregion
        #region Event triggers

        protected virtual void OnHit()
        {
            if (ProjectileHit != null)
                ProjectileHit(this);
        }

        #endregion
        #region Overrides

        public override void OnSpawn()
        {
            AssertAnimation("fly");
        }

        public override void OnTick(GameTime time)
        {
            AssertFacingTowardsFoe();
            AssertAnimation("fly");
            if (AnimationEnded)
                ResetAnimation();

            float elapsedTime = time.ElapsedGameTime.Milliseconds/1000.0f;

            Velocity += elapsedTime*new Vector3(0, -Gravity, 0);
            Avatar.Position += elapsedTime*Velocity;

            if (active && Avatar.Position.Y < Simulation.GroundLevel)
            {
                OnHit();
                active = false;
            }

            if (Avatar.Position.Y < -5.0f)
                Simulation.Remove(this);

            base.OnTick(time);
        }

        #endregion
        #region Public methods

        public void SetTarget(BattleUnit u)
        {
            Target = u;
            float uWidth = u.Avatar.PixelScale.X*u.Avatar.TexRect.Width;

            if (u.Avatar.Flipped)
                uWidth = -uWidth;

            float xDistance = u.Avatar.Position.X - Avatar.Position.X + 2*uWidth;
            SetTarget(xDistance);
        }

        public void SetTarget(float xDistance)
        {
            double angle = Math.PI*0.25;
            var v = (float) Math.Sqrt((Math.Abs(xDistance)*Gravity)/Math.Sin(2*angle));
            direction = Math.Sign(xDistance);
            Velocity = new Vector3(direction*v*(float) Math.Cos(angle), v*(float) Math.Sin(angle), 0);
        }

        #endregion
        #region Protected methods

        protected void AssertFacingTowardsFoe()
        {
            direction = Player.Direction;
            if (Player != null)
            {
                Avatar.Flipped = (direction < 0);
            }
        }

        #endregion
    }
}