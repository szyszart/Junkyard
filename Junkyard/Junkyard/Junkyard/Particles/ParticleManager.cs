using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Junkyard.Particles
{
    public class ParticleManager
    {
        #region Constants

        private const string BAD_KEY_ARG_EXC_MSG = "There is ParticleSystem assiociated with this key already.";
        private const string NO_PS_ARG_EXC_MSG = "There is no ParticleSystem assiociated with this key.";

        #endregion
        #region Private fields

        private readonly Game _game;
        private readonly Dictionary<int, ParticleSystem> _particleSystems = new Dictionary<int, ParticleSystem>(4);
        private readonly SpriteBatch _spriteBatch;

        #endregion
        #region Properties

        /// <summary>
        /// Gets the ParticleSystem associated with given key
        /// </summary>
        /// <param name="key">The key of the ParticleSystem to get</param>
        /// <returns></returns>
        public ParticleSystem this[int key]
        {
            get
            {
                if (!_particleSystems.ContainsKey(key))
                {
                    throw new ArgumentException(NO_PS_ARG_EXC_MSG);
                }
                return _particleSystems[key];
            }
        }

        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        #endregion
        #region Ctors

        public ParticleManager(Game game)
        {
            _game = game;
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        #endregion
        #region Public methods

        public void AddSystem<T>(int key, int howManyEffects, float z)
            where T : ParticleSystem
        {
            if (_particleSystems.ContainsKey(key))
            {
                throw new ArgumentException(NO_PS_ARG_EXC_MSG);
            }
            Type type = typeof (T);
            var pSys = (ParticleSystem) Activator.CreateInstance(type, _game, this, howManyEffects);

            _particleSystems.Add(key, pSys);
            _game.Components.Add(pSys);
        }

        //public void Initialize(Game game)
        //{
        //    _game = game;
        //    _spriteBatch = new SpriteBatch(game.GraphicsDevice);
        //}

        public void RemoveSystem(int key)
        {
            if (!_particleSystems.ContainsKey(key))
            {
                throw new ArgumentException(BAD_KEY_ARG_EXC_MSG);
            }
            _particleSystems.Remove(key);
        }

        #endregion
    }
}