using System;
using System.Collections.Generic;


namespace Checkers.Models.GameCore
{
    /// <summary>
    /// Методы расширения.
    /// </summary>
    internal static class Extensions 
    {
        /// <summary>
        /// Перемешать список.
        /// </summary>
        /// <typeparam name="T">Тип.</typeparam>
        /// <param name="list">Список.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            var rand = new Random();
            var n = list.Count;

            while (n > 1) {
                n--;
                var k = rand.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            } // while
        } // Shuffle
    } // Extensions
} // Checkers.Model