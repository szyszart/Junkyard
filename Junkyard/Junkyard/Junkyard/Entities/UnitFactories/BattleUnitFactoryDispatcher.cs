using System;
using System.Collections.Generic;
using Junkyard.Entities.Units;

namespace Junkyard.Entities.UnitFactories
{
    public class BattleUnitFactoryDispatcher
    {
        #region Private fields

        protected Dictionary<string, BattleUnitFactory> specificFactories = new Dictionary<string, BattleUnitFactory>();

        #endregion
        #region Public methods

        public BattleUnit Create(string name)
        {
            return specificFactories[name].Create();
        }

        public void RegisterFactory(string name, BattleUnitFactory factory)
        {
            if (specificFactories.ContainsKey(name))
                throw new ArgumentException("Name is already bound with a factory.");
            specificFactories[name] = factory;
        }

        #endregion
    }
}