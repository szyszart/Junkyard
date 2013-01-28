using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard.Entities.Units
{
    public class Infantry : AnimatedBattleUnit
    {
        #region Private fields

        protected float direction = 1.0f;
        protected bool dying = false;

        #endregion
        #region Properties

        public float AttackRange { get; set; }
        public float Speed { get; set; }

        #endregion
        #region Ctors

        public Infantry()
        {
        }

        public Infantry(Simulation simulation, Player player, Texture2D texture, Vector3 position)
            : base(simulation, player, texture, position)
        {
        }

        #endregion
        #region Overrides

        public override void OnDeath()
        {
            AssertAnimation("death");
            dying = true;
        }

        public override void OnSpawn()
        {
            AssertAnimation("walk");
        }

        public override void OnTick(GameTime time)
        {
            AssertFacingTowardsFoe();

            if (dying)
            {
                AssertAnimation("dying");
                if (AnimationEnded)
                {
                    ReallyDead = true;
                }
            }
            else
            {
                BattleUnit foe = Simulation.GetNearestFoe(this);
                float dist = (foe != null)
                                 ? Vector3.Distance(foe.Avatar.Position, Avatar.Position)
                                 : float.PositiveInfinity;
                if (foe != null && dist <= AttackRange &&
                    Math.Sign(direction)*(foe.Avatar.Position.X - Avatar.Position.X) >= 0.25*AttackRange)
                {
                    AssertAnimation("attack");
                }
                else if (currentAnimation != "attack" || AnimationEnded)
                {
                    AssertAnimation("walk");
                    Move();
                }

                Hp -= AcumulatedDmg;
                AcumulatedDmg = 0;

                if (Hp <= 0)
                {
                    dying = true;
                }

                if (currentAnimation == "attack" && AnimationEnded)
                {
                    if (foe != null)
                    {
                        Simulation.Attack(foe, this);
                    }
                }

                if (AnimationEnded)
                    ResetAnimation();
            }
            base.OnTick(time);
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

        protected void Move()
        {
            Avatar.Position += direction*Speed*Vector3.UnitX;
        }

        #endregion
    }
}