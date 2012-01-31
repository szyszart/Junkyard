using System;
using System.Collections.Generic;
using System.Linq;

namespace Junkyard.Helpers
{
    public static class GeneralHelper
    {
        #region Private fields

        private static readonly Random _random = new Random();
        public static Random Random { get { return _random; } }

        #endregion
        #region Public static methods

        public static T GetRandomElement<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            return list.Count() == 0 ? default(T) : list.ElementAt(_random.Next(list.Count()));
        }

        //  a handy little function that gives a random float between two
        // values. This will be used in several places in the sample, in particilar in
        // ParticleSystem.InitializeParticle.
        public static float RandomBetween(float min, float max)
        {
            return min + (float) _random.NextDouble()*(max - min);
        }

        #endregion
    }
}