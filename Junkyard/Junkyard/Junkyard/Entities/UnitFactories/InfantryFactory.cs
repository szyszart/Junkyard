using Junkyard.Entities.Units;

namespace Junkyard.Entities.UnitFactories
{
    internal class InfantryFactory : GenuineBattleUnitFactory
    {
        #region Overrides

        public override BattleUnit Create()
        {
            var unit = new Infantry {Speed = Speed, AttackRange = AttackRange, Hp = InitialHP, Animations = Animations};

            return unit;
        }

        #endregion
    }
}