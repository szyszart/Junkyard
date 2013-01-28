#region

using System;
using System.Diagnostics;
using Junkyard.Consts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATweener;

#endregion

namespace Junkyard.Entities.Units
{
    public class BattleUnit : IDrawable
    {
        #region Private fields

        protected int AcumulatedDmg = 0;
        private Tweener _disposeTweener;

        #endregion
        #region Properties

        public ScaledSprite3D Avatar { get; protected set; }
        public int Hp { get; set; }
        public Player Player { get; set; }
        public bool ReallyDead { get; set; }
        public Simulation Simulation { get; set; }

        /// <summary>
        ///     Determines whether enemy units will see this unit as a hostile entity.
        ///     Set to false to represent an entity that is not a real battle unit (e.g. a projectile).
        /// </summary>
        public virtual bool Stealth { get; set; }

        #endregion
        #region Ctors

        public BattleUnit()
        {
        }

        public BattleUnit(Simulation simulation, Player player)
        {
            Simulation = simulation;
            Player = player;
        }

        #endregion
        #region Event triggers

        public virtual void OnDamage(int dmg)
        {
            AcumulatedDmg += dmg;
        }

        public virtual void OnDeath()
        {
        }

        /// <summary>
        ///     Perform a step of (visually)disposing the unit from the game.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="duration">Duration of disposing animation in seconds.</param>
        /// <param name="disposeOffset">How match </param>
        /// <returns>Is unit already disposed?</returns>
        /// TODO: calculate proper offset based of world coordinates
        public virtual bool OnDispose(GameTime gameTime, float duration = UnitConsts.DEFAULT_DISPOSE_DURATION,
                                      float disposeOffset = 5f)
        {
            Debug.Assert(Avatar != null);
            if (Avatar == null)
            {
                throw new InvalidOperationException();
            }
            Debug.Assert(duration > 0);

            if (_disposeTweener == null)
            {
                _disposeTweener = new Tweener(Avatar.Position.Y,
                                              (Avatar.Position.Y + disposeOffset),
                                              duration,
                                              Cubic.EaseIn);
                _disposeTweener.Play();
                return false;
            }

            _disposeTweener.Update(gameTime);
            Avatar.Position = new Vector3(Avatar.Position.X, _disposeTweener.Position, Avatar.Position.Z);
            return !_disposeTweener.Playing;
        }

        /// <summary>
        ///     Perform a step of (visually)disposing the unit from the game.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="duration">Duration of disposing animation.</param>
        /// <param name="disposeOffset">How match </param>
        /// <returns>Is unit already disposed?</returns>
        /// TODO: calculate proper offset based of world coordinates
        public virtual bool OnDispose(GameTime gameTime, TimeSpan duration, float disposeOffset = 5f)
        {
            Debug.Assert(duration.TotalSeconds < float.MaxValue);
            return OnDispose(gameTime, (float) duration.TotalSeconds, disposeOffset);
        }

        public virtual void OnRemove()
        {
        }

        public virtual void OnSpawn()
        {
        }

        public virtual void OnTick(GameTime time)
        {
        }

        #endregion
        #region IDrawable Members

        public virtual void Draw(Effect effect)
        {
        }

        #endregion
    }
}