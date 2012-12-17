using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard.Entities.Units
{
    internal class Ranged : AnimatedBattleUnit
    {
        #region Private fields

        protected float direction = 1.0f;
        protected bool dying = false;

        protected bool thrown = false;

        #endregion
        #region Properties

        public float AttackRange { get; set; }
        public float Speed { get; set; }

        #endregion
        #region Ctors

        public Ranged()
        {
        }

        public Ranged(Simulation simulation, Player player, Texture2D texture, Vector3 position)
            : base(simulation, player, texture, position)
        {
        }

        #endregion
        #region Event handlers

        protected void ProjectileHit(Projectile p)
        {
            if (p.Target != null)
                Simulation.Attack(p.Target, this);
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
                    reallyDead = true;
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

                Hp -= acumulatedDmg;
                acumulatedDmg = 0;

                if (Hp <= 0)
                {
                    dying = true;
                }

                if (currentAnimation == "attack" && currentFrame == 7 && !thrown)
                {
                    var projectile = (Projectile) Simulation.Spawn("menel_ranged_projectile");
                    projectile.Avatar.Position = Avatar.Position;
                    projectile.Gravity = 10.0f;
                    projectile.Player = Player;
                    projectile.Simulation = Simulation;
                    projectile.ProjectileHit += ProjectileHit;

                    if (foe != null)
                    {
                        projectile.SetTarget(foe);
                    }
                    else
                    {
                        projectile.Velocity = new Vector3(direction*5.5f, 0.5f, 0.0f);
                    }

                    Simulation.Add(projectile);
                    thrown = true;
                }

                if (AnimationEnded)
                {
                    ResetAnimation();
                    thrown = false;
                }
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