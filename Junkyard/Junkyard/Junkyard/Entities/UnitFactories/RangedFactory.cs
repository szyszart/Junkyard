using Junkyard.Consts;
using Junkyard.Entities.Units;
using Junkyard.Helpers;

namespace Junkyard.Entities.UnitFactories
{
    internal class RangedFactory : GenuineBattleUnitFactory
    {
        #region Overrides

        public override BattleUnit Create()
        {
            var unit = new Ranged
                           {
                               Animations = Animations,
                               AttackRange = AttackRange,
                               Speed =
                                   Speed + RandomizationHelper.RandomBetween(0, UnitConsts.RANDOM_SPEED_OFFSET),
                               Hp = InitialHP
                           };
            return unit;
        }

        #endregion
    }
}