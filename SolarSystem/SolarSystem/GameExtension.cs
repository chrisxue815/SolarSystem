using System;
using System.Linq;

namespace SolarSystem
{
    public static class GameExtension
    {
        public static bool Contains<T>(this T[] array, T obj)
        {
            return Enumerable.Contains(array, obj);
        }
    }
}
