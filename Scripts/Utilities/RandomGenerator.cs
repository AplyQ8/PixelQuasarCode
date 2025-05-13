using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

namespace Utilities
{
    public class RandomGenerator
    {
        private static RandomGenerator _instance;

        private readonly Random _rand;
        public RandomGenerator()
        {
            _rand = new Random();
        }
        
        public static RandomGenerator Instance => _instance ??= new RandomGenerator();
        
        /// <summary>
        /// Evaluates random value between 0 and 1
        /// </summary>
        /// <param name="threshold">Chance value</param>
        /// <returns>true if an input chance value is in range. Otherwise false</returns>
        public bool IsInRange(float threshold) => (float)_rand.NextDouble() <= threshold;
        /// <summary>
        /// Generates and returns random value between to points.
        /// </summary>
        /// <param name="from">start value</param>
        /// <param name="to">end value</param>
        /// <returns>Value between From and To</returns>
        public int RandomValueInRange(int from, int to) => _rand.Next(from, to);
        /// <summary>
        /// Generates and returns random float value between two points.
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <returns>Float value between From and To</returns>
        public float RandomValueInRange(float from, float to) => (float)(_rand.NextDouble() * (to - from) + from);
    }
}

