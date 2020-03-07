using System;
using PoeCraftLib.Entities;

namespace PoeCraftLib.Data
{
    public class PoeRandom : IRandom
    {
        private readonly Random _random = new Random();
        public int Next()
        {
            return _random.Next();
        }

        public int Next(int max)
        {
            return _random.Next(max);
        }

        public int Next(int min, int max)
        {
            return _random.Next(min, max);
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }
}
