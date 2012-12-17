using System;
using Junkyard.Entities.Units;
using LuaInterface;
using Microsoft.Xna.Framework;

namespace Junkyard.Entities.UnitFactories
{
    internal class MeteorFactory : BattleUnitFactory
    {
        #region Private fields

        private Vector3 acc;
        private float height;
        private float range;
        private Vector3 velocity;

        #endregion
        #region Overrides

        public override BattleUnit Create()
        {
            var unit = new Meteor();
            unit.Animations = Animations;
            unit.Velocity = velocity;
            unit.Acceleration = acc;
            unit.Range = range;
            unit.Avatar.Position = unit.Avatar.Position + height*Vector3.UnitY;
            return unit;
        }

        protected override void ValidateAndStore(LuaTable data)
        {
            base.ValidateAndStore(data);
            try
            {
                var v = (LuaTable) data["velocity"];
                velocity = new Vector3((float) (double) v[1], (float) (double) v[2], (float) (double) v[3]);

                var a = (LuaTable) data["acceleration"];
                acc = new Vector3((float) (double) v[1], (float) (double) v[2], (float) (double) v[3]);

                height = (float) (double) data["height"];
                range = (float) (double) data["range"];
            }
            catch
            {
                throw new ArgumentException("Invalid unit description.");
            }
        }

        #endregion
    }
}