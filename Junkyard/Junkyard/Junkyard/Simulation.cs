using System;
using System.Collections.Generic;
using Junkyard.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class Simulation : IDrawable
    {
        #region Private Fields
        private readonly Random _random = new Random();
        #endregion

        #region Properties
        protected List<BattleUnit> Units;
        protected List<BattleUnit> ToRemove;

        public Player playerOne { get; set; }
        public Player playerTwo { get; set; }
        
        #endregion

        public Simulation()
        {
            Units = new List<BattleUnit>();
            ToRemove = new List<BattleUnit>();
        }

        public void Add(BattleUnit unit)
        {            
            Units.Add(unit);
            unit.OnSpawn();
        }

        public BattleUnit GetNearestFoe(BattleUnit requester)
        {
            BattleUnit nearest = null;
            float minDist = float.PositiveInfinity;
            foreach (BattleUnit unit in Units)
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
            if (attacker.Hp > 0)
                who.Hp -= 60 + _random.Next(48);
        }

        public void Remove(BattleUnit unit)
        {
            unit.OnRemove();
            ToRemove.Add(unit);
        }

        public void Tick(GameTime time)
        {
            foreach (BattleUnit unit in Units)
            {
                unit.OnTick(time);
                // TODO: add a better way of determining whether a unit has reached its destination
                Player EnemyPlayer = (unit.Player == playerOne) ? playerTwo : playerOne;
                var EnemyShip = EnemyPlayer.Ship;

                if (MathHelper.Distance(EnemyShip.Position.X, unit.Avatar.Position.X) < 0.5f )
                {
                    EnemyPlayer.Hp -= 10;
                    Remove(unit);
                }
            }

            foreach (BattleUnit unit in ToRemove)
                Units.Remove(unit);            

            ToRemove.Clear();
        }

        public void Draw(Effect effect)
        {
            foreach (BattleUnit unit in Units)
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
        public Ship Ship { get; set; }
        public int Hp { get; set; }
        public Player(string name)
        {
            Name = name;
            InitialPosition = Vector3.Zero;
            Direction = 1.0f;
            Hp = 50;
        }
    }
}