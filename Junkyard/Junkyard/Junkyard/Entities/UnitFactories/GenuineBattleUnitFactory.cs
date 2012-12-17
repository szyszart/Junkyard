using System;
using LuaInterface;

namespace Junkyard.Entities.UnitFactories
{
    public abstract class GenuineBattleUnitFactory : BattleUnitFactory
    {
        #region Properties

        protected float AttackRange { get; set; }
        protected int InitialHP { get; set; }
        protected float Speed { get; set; }

        #endregion
        #region Overrides

        protected override void ValidateAndStore(LuaTable data)
        {
            base.ValidateAndStore(data);

            try
            {
                Speed = (float) (double) data["speed"];
                AttackRange = (float) (double) data["attack_range"];
                InitialHP = (int) (double) data["initial_hp"];
            }
            catch
            {
                throw new ArgumentException("Invalid unit description.");
            }
        }

        #endregion
    }
}