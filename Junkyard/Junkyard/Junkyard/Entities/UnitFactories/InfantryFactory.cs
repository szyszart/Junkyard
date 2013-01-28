using Junkyard.Consts;
using Junkyard.Entities.Units;
using Junkyard.Helpers;

namespace Junkyard.Entities.UnitFactories
{
    internal class InfantryFactory : GenuineBattleUnitFactory
    {
        #region Overrides

        public override BattleUnit Create()
        {
            var unit = new Infantry
                           {
                               Speed =
                                   Speed + RandomizationHelper.RandomBetween(0, UnitConsts.RANDOM_SPEED_OFFSET),
                               AttackRange = AttackRange,
                               Hp = InitialHP,
                               Animations = Animations
                           };

            return unit;
        }

        #endregion
    }
}