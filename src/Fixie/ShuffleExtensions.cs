﻿namespace Fixie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ShuffleExtensions
    {
        /// <summary>
        /// Randomizes the order of the given tests.
        /// </summary>
        public static IReadOnlyList<T> Shuffle<T>(this IEnumerable<T> tests)
        {
            return tests.Shuffle(new Random());
        }

        /// <summary>
        /// Randomizes the order of the given tests, using the given pseudo-random number generator.
        /// </summary>
        public static IReadOnlyList<T> Shuffle<T>(this IEnumerable<T> tests, Random random)
        {
            var array = tests.ToList();

            FisherYatesShuffle(array, random);

            return array;
        }

        static void FisherYatesShuffle<T>(IList<T> array, Random random)
        {
            int n = array.Count;
            while (n > 1)
            {
                int k = random.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}