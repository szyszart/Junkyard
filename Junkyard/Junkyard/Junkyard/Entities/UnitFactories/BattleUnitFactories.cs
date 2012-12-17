using System.Collections.Generic;
using Junkyard.Animations;
using Junkyard.Entities.Units;
using LuaInterface;

namespace Junkyard.Entities.UnitFactories
{
    public abstract class BattleUnitFactory
    {
        #region Private fields

        protected LuaTable _params;

        #endregion
        #region Properties

        public Dictionary<string, Animation> Animations { get; set; }

        public virtual LuaTable Params
        {
            get { return _params; }
            set
            {
                ValidateAndStore(value);
                _params = value;
            }
        }

        #endregion
        #region Ctors

        public BattleUnitFactory()
        {
            Animations = new Dictionary<string, Animation>();
        }

        #endregion
        #region Public methods

        public abstract BattleUnit Create();

        #endregion
        #region Protected methods

        protected virtual void ValidateAndStore(LuaTable data)
        {
        }

        #endregion
    }
}