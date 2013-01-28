using System;
using System.Collections.Generic;
using System.Linq;

namespace Junkyard.Helpers
{
    public static class RandomizationHelper
    {
        #region Private fields

        private static readonly Random _random = new Random();

        #endregion
        #region Properties

        public static Random Random
        {
            get { return _random; }
        }

        #endregion
        #region Public static methods

        public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            var list = enumerable as IList<T> ?? enumerable.ToList();
            return !list.Any() ? default(T) : list.ElementAt(_random.Next(list.Count()));
        }

        /// <summary>
        /// A handy little function that gives a random float between two
        /// values. This will be used in several places in the sample, in particilar in
        /// ParticleSystem.InitializeParticle.
        /// </summary>
        public static float RandomBetween(float min, float max)
        {
            return min + (float) _random.NextDouble()*(max - min);
        }

        #endregion
    }
}