using System;
using System.Collections.Generic;
using Junkyard.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class Simulation : IDrawable
    {
        #region Private fields

        protected List<BattleUnit> ToRemove;
        private readonly Random _random = new Random();

        private readonly List<BattleUnit> _units;

        #endregion
        #region Properties

        public List<BattleUnit> Units
        {
            get { return _units; }
        }

        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        #endregion
        #region Ctors

        public Simulation()
        {
            _units = new List<BattleUnit>();
            ToRemove = new List<BattleUnit>();
        }

        #endregion
        #region Public methods

        public void Add(BattleUnit unit)
        {
            _units.Add(unit);
            unit.OnSpawn();
        }

        public void Attack(BattleUnit who, BattleUnit attacker)
        {
            if (attacker.Hp > 0)
                who.Hp -= 60 + _random.Next(48);
        }

        public BattleUnit GetNearestFoe(BattleUnit requester)
        {
            BattleUnit nearest = null;
            float minDist = float.PositiveInfinity;
            foreach (BattleUnit unit in _units)
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

        public void Remove(BattleUnit unit)
        {
            unit.OnRemove();
            ToRemove.Add(unit);
        }

        public void Tick(GameTime time)
        {
            foreach (BattleUnit unit in _units)
            {
                unit.OnTick(time);
                // TODO: add a better way of determining whether a unit has reached its destination
                Player enemyPlayer = (unit.Player == PlayerOne) ? PlayerTwo : PlayerOne;
                Ship enemyShip = enemyPlayer.Ship;

                if (MathHelper.Distance(enemyShip.Position.X, unit.Avatar.Position.X) < 0.6f)
                {
                    enemyPlayer.Hp -= 10;
                    Remove(unit);
                }
            }

            foreach (BattleUnit unit in ToRemove)
                _units.Remove(unit);

            ToRemove.Clear();
        }

        #endregion
        #region IDrawable Members

        public void Draw(Effect effect)
        {
            foreach (BattleUnit unit in _units)
            {
                unit.Draw(effect);
            }
        }

        #endregion
    }

    public class Player
    {
        #region Properties

        public float Direction { get; set; }
        public int Hp { get; set; }
        public Vector3 InitialPosition { get; set; }
        public string Name { get; set; }
        public Ship Ship { get; set; }

        #endregion
        #region Ctors

        public Player(string name)
        {
            Name = name;
            InitialPosition = Vector3.Zero;
            Direction = 1.0f;
            Hp = 50;
        }

        #endregion
    }
}