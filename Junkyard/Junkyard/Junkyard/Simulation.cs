using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class Simulation : IDrawable
    {
        protected List<BattleUnit> units;
        protected List<BattleUnit> toRemove;
        protected Random random = new Random();

        public Simulation()
        {
            units = new List<BattleUnit>();
            toRemove = new List<BattleUnit>();
        }

        public void Add(BattleUnit unit)
        {            
            units.Add(unit);
            unit.OnSpawn();
        }

        public BattleUnit GetNearestFoe(BattleUnit requester)
        {
            BattleUnit nearest = null;
            float minDist = float.PositiveInfinity;
            foreach (BattleUnit unit in units)
            {
                if (unit.Player == requester.Player || unit.reallyDead)
                    continue;
                float curDist = Vector3.Distance(unit.Avatar.Position, requester.Avatar.Position);
                if (curDist < minDist)
                {
                    minDist = curDist;
                    nearest = unit;
                }
            }
            return nearest;
        }

        public void Attack(BattleUnit who, BattleUnit attacker)
        {
            if (attacker.HP > 0)
                who.HP -= 60 + random.Next(48);
        }

        public void Remove(BattleUnit unit)
        {
            unit.OnRemove();
            toRemove.Add(unit);
        }

        public void Tick(GameTime time)
        {
            foreach (BattleUnit unit in units)
            {
                unit.OnTick(time);
                // TODO: add a reasonable way of determining whether a unit has reached its destination
                if (Math.Abs(unit.Avatar.Position.X) > 5.0f)
                    Remove(unit);
            }

            foreach (BattleUnit unit in toRemove)
                units.Remove(unit);

            toRemove.Clear();
        }

        public void Draw(Effect effect)
        {
            foreach (BattleUnit unit in units)
            {
                unit.Draw(effect);
            }
        }
    }

    public class Player
    {
        public Vector3 InitialPosition { get; set; }
        public string Name { get; set; }
        public float Direction { get; set; }
        public Player(string name)
        {
            Name = name;
            InitialPosition = Vector3.Zero;
            Direction = 1.0f;
        }
    }
}