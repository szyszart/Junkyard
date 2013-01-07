using System;
using Junkyard.Entities.Units;
using LuaInterface;
using Microsoft.Xna.Framework;

namespace Junkyard.Entities.UnitFactories
{
    internal class MeteorFactory : BattleUnitFactory
    {
        #region Private fields

        private Vector3 _acc;
        private float _height;
        private float _range;
        private Vector3 _velocity;

        #endregion
        #region Overrides

        public override BattleUnit Create()
        {
            var unit = new Meteor
                           {
                               Animations = Animations,
                               Velocity = _velocity,
                               Acceleration = _acc,
                               Range = _range
                           };
            unit.Avatar.Position = unit.Avatar.Position + _height*Vector3.UnitY;
            return unit;
        }

        protected override void ValidateAndStore(LuaTable data)
        {
            base.ValidateAndStore(data);
            try
            {
                var v = (LuaTable) data["velocity"];
                _velocity = new Vector3((float) (double) v[1], (float) (double) v[2], (float) (double) v[3]);

                var a = (LuaTable) data["acceleration"];
                _acc = new Vector3((float) (double) a[1], (float) (double) a[2], (float) (double) a[3]);

                _height = (float) (double) data["height"];
                _range = (float) (double) data["range"];
            }
            catch
            {
                throw new ArgumentException("Invalid unit description.");
            }
        }

        #endregion
    }
}