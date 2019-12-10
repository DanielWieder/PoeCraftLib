using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Data
{
    public class PoeRandom : IRandom
    {
        readonly Random _random = new Random();
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
