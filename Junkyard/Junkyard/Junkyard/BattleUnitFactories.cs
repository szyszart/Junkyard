using System;
using System.Collections.Generic;
using LuaInterface;

namespace Junkyard
{
    abstract class BattleUnitFactory
    {        
        public BattleUnitFactory()
        {
            Animations = new Dictionary<string, Animation>();
        }

        public Dictionary<string, Animation> Animations { get; set; }
        public virtual LuaTable Params { get; set; }

        public abstract BattleUnit Create();
    }

    class BattleUnitFactoryDispatcher {
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

    class InfantryFactory : BattleUnitFactory {
        private float speed;
        private float attackRange;
        private int initialHP;

        public override LuaTable Params
        {
            get
            {
                return base.Params;
            }
            set
            {
                ValidateAndStore(value);
                base.Params = value;
            }
        }

        private void ValidateAndStore(LuaTable data)
        {
            try
            {
                speed = (float)(double)data["speed"];
                attackRange = (float)(double)data["attack_range"];
                initialHP = (int)(double)data["initial_hp"];
            }
            catch
            {
                throw new ArgumentException("Invalid unit description.");
            }
        }

        public override BattleUnit Create()
        {
            Infantry unit = new Infantry();
            unit.Speed = speed;
            unit.AttackRange = attackRange;
            unit.Hp = initialHP;
            unit.Animations = Animations;       

            return unit;
        }
    }
}