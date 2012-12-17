using System;
using Junkyard.Entities.Units;
using LuaInterface;

namespace Junkyard.Entities.UnitFactories
{
    internal class ProjectileFactory : BattleUnitFactory
    {
        #region Private fields

        private float gravity;

        #endregion
        #region Overrides

        public override BattleUnit Create()
        {
            var unit = new Projectile();
            unit.Animations = Animations;
            return unit;
        }

        protected override void ValidateAndStore(LuaTable data)
        {
            try
            {
                gravity = (float) (double) data["gravity"];
            }
            catch
            {
                throw new ArgumentException("Invalid unit description.");
            }
        }

        #endregion
    }
}