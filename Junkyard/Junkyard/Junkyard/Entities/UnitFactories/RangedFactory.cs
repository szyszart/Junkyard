using Junkyard.Entities.Units;

namespace Junkyard.Entities.UnitFactories
{
    internal class RangedFactory : GenuineBattleUnitFactory
    {
        #region Overrides

        public override BattleUnit Create()
        {
            var unit = new Ranged {Animations = Animations, AttackRange = AttackRange, Speed = Speed, Hp = InitialHP};
            return unit;
        }

        #endregion
    }
}