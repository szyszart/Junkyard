using System;
using System.Collections.Generic;
using LuaInterface;
using Microsoft.Xna.Framework;

namespace Junkyard
{
    public abstract class BattleUnitFactory
    {        
        public BattleUnitFactory()
        {
            Animations = new Dictionary<string, Animation>();
        }

        public Dictionary<string, Animation> Animations { get; set; }

        protected LuaTable _params;
        public virtual LuaTable Params
        {
            get
            {
                return _params;
            }
            set
            {                
                ValidateAndStore(value);
                _params = value;
            }
        }

        protected virtual void ValidateAndStore(LuaTable data) { }
        public abstract BattleUnit Create();
    }

    public abstract class GenuineBattleUnitFactory : BattleUnitFactory
    {
        protected float Speed { get; set; }
        protected float AttackRange { get; set; }
        protected int InitialHP { get; set; }

        protected override void ValidateAndStore(LuaTable data)
        {
            base.ValidateAndStore(data);

            try
            {
                Speed = (float)(double)data["speed"];
                AttackRange = (float)(double)data["attack_range"];
                InitialHP = (int)(double)data["initial_hp"];
            }
            catch
            {
                throw new ArgumentException("Invalid unit description.");
            }
        }
    }

    public class BattleUnitFactoryDispatcher {
        protected Dictionary<string, BattleUnitFactory> specificFactories = new Dictionary<string, BattleUnitFactory>();

        public void RegisterFactory(string name, BattleUnitFactory factory)
        {
            if (specificFactories.ContainsKey(name))
                throw new ArgumentException("Name is already bound with a factory."); 
            specificFactories[name] = factory;
        }

        public BattleUnit Create(string name)
        {
            return specificFactories[name].Create();
        }
    }

    class InfantryFactory : GenuineBattleUnitFactory
    {
        public override BattleUnit Create()
        {
            Infantry unit = new Infantry();
            unit.Speed = Speed;
            unit.AttackRange = AttackRange;
            unit.Hp = InitialHP;
            unit.Animations = Animations;       

            return unit;
        }
    }

    class ProjectileFactory : BattleUnitFactory
    {
        private float gravity;

        protected override void ValidateAndStore(LuaTable data)
        {
            try
            {
                gravity = (float)(double)data["gravity"];
            }
            catch
            {
                throw new ArgumentException("Invalid unit description.");
            }
        }

        public override BattleUnit Create()
        {
            Projectile unit = new Projectile();
            unit.Animations = Animations;
            return unit;
        }
    }

    class RangedFactory : GenuineBattleUnitFactory
    {
        public override BattleUnit Create()
        {
            Ranged unit = new Ranged();
            unit.Animations = Animations;
            unit.AttackRange = AttackRange;
            unit.Speed = Speed;
            unit.Hp = InitialHP;
            return unit;
        }
    }

    class MeteorFactory : BattleUnitFactory
    {
        private Vector3 velocity;
        private Vector3 acc;
        private float height;
        private float range;

        protected override void ValidateAndStore(LuaTable data)
        {
            base.ValidateAndStore(data);
            try
            {
                var v = (LuaTable)data["velocity"];                
                velocity = new Vector3((float)(double)v[1], (float)(double)v[2], (float)(double)v[3]);

                var a = (LuaTable)data["acceleration"];
                acc = new Vector3((float)(double)v[1], (float)(double)v[2], (float)(double)v[3]);

                height = (float)(double)data["height"];
                range = (float)(double)data["range"];
            }
            catch
            {
                throw new ArgumentException("Invalid unit description.");
            }
        }

        public override BattleUnit Create()
        {
            Meteor unit = new Meteor();
            unit.Animations = Animations;
            unit.Velocity = velocity;
            unit.Acceleration = acc;
            unit.Range = range;
            unit.Avatar.Position = unit.Avatar.Position + height * Vector3.UnitY;
            return unit;
        }
    }
}