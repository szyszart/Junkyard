using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard.Entities.Units
{
    public class BattleUnit : IDrawable
    {
        #region Private fields

        public bool reallyDead = false;

        protected int acumulatedDmg = 0;

        #endregion
        #region Properties

        public ScaledSprite3D Avatar { get; protected set; }
        public int Hp { get; set; }
        public Player Player { get; set; }
        public Simulation Simulation { get; set; }

        /// <summary>
        /// Determines whether enemy units will see this unit as a hostile entity.
        /// Set to false to represent an entity that is not a real battle unit (e.g. a projectile).
        /// </summary>
        public virtual bool Stealth { get; set; }

        #endregion
        #region Ctors

        public BattleUnit()
        {
            Stealth = false;
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
            acumulatedDmg += dmg;
        }

        public virtual void OnDeath()
        {
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