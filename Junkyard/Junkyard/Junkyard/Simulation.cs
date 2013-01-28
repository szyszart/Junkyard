using System;
using System.Collections.Generic;
using Junkyard.Entities;
using Junkyard.Entities.UnitFactories;
using Junkyard.Entities.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard
{
    public class Simulation : IDrawable
    {
        #region Private fields

        private readonly List<BattleUnit> _toAdd;
        private readonly List<BattleUnit> _toRemove;
        private readonly Random _random = new Random();

        private readonly List<BattleUnit> _units;

        #endregion
        #region Properties

        public BattleUnitFactoryDispatcher FactoryDispatcher { get; protected set; }
        public float GroundLevel { get; set; }

        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        public List<BattleUnit> Units
        {
            get { return _units; }
        }

        #endregion
        #region Ctors

        public Simulation()
        {
            _units = new List<BattleUnit>();
            _toRemove = new List<BattleUnit>();
            _toAdd = new List<BattleUnit>();
            FactoryDispatcher = new BattleUnitFactoryDispatcher();
            GroundLevel = 0;
        }

        #endregion
        #region Public methods

        public void Add(BattleUnit unit)
        {
            _toAdd.Add(unit);
        }

        public void Attack(BattleUnit who, BattleUnit attacker)
        {
            if (attacker.Stealth || attacker.Hp > 0)
                who.Hp -= 60 + _random.Next(48);
        }

        public BattleUnit GetNearestFoe(BattleUnit requester)
        {
            BattleUnit nearest = null;
            float minDist = float.PositiveInfinity;
            foreach (BattleUnit unit in _units)
            {
                if (unit.Player == requester.Player || unit.ReallyDead || unit.Stealth)
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
            _toRemove.Add(unit);
        }

        public BattleUnit Spawn(string kind)
        {
            return FactoryDispatcher.Create(kind);
        }

        public void Tick(GameTime time)
        {
            foreach (BattleUnit unit in _units)
            {
                if (!unit.ReallyDead)
                {
                    unit.OnTick(time);
                }
                else
                {
                    //if disposing ended - remove
                    if (unit.OnDispose(time))
                    {
                        Remove(unit);
                        continue;
                    }
                }

                if (unit.Stealth) continue;

                // TODO: add a better way of determining whether a unit has reached its destination
                Player enemyPlayer = (unit.Player == PlayerOne) ? PlayerTwo : PlayerOne;
                Ship enemyShip = enemyPlayer.Ship;

                // TODO: add a Y coordinate check
                if (MathHelper.Distance(enemyShip.Position.X, unit.Avatar.Position.X) < 0.6f)
                {
                    // TODO: custom effects of unit atack on the enemy ship
                    enemyPlayer.Hp -= 10;
                    Remove(unit);
                }
            }

            foreach (BattleUnit unit in _toAdd)
            {
                _units.Add(unit);
                unit.OnSpawn();
            }

            _toAdd.Clear();

            foreach (BattleUnit unit in _toRemove)
                _units.Remove(unit);

            _toRemove.Clear();
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
}