using System;

namespace CLFramework
{
    public class Rnd
    {
        private static Random PrivateRnd = new Random();

        public static int Next()
        {
            lock (PrivateRnd)
            {
                return PrivateRnd.Next();
            }
        }
        public static int Next(int MaxValue)
        {
            lock (PrivateRnd)
            {
                return PrivateRnd.Next(MaxValue);
            }
        }
        public static int Next(int MinValue, int MaxValue)
        {
            lock (PrivateRnd)
            {
                return PrivateRnd.Next(MinValue, MaxValue);
            }
        }
        public static double NextDouble()
        {
            lock (PrivateRnd)
            {
                return PrivateRnd.NextDouble();
            }
        }
    }

}
