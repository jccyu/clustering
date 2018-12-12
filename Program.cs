using System;
using System.Linq;

namespace Clustering
{
    class Program
    {
        static void Main(string[] args)
        {
            int length = 30000;
            Random rand = new Random();
            var currentTime = DateTime.Now;
            (byte, byte)[] points = new (byte, byte)[length];
            for (int i = 0; i < length; i++)
            {
                points[i].Item1 = (byte)rand.Next(Byte.MaxValue);
                points[i].Item2 = (byte)rand.Next(Byte.MaxValue);
            }

            Hierarchical<(byte, byte)> hierarchical = new Hierarchical<(byte, byte)>(points, ((x, y) => (byte)Math.Min(byte.MaxValue, Math.Sqrt((y.Item1 - x.Item1) * (y.Item1 - x.Item1) + (y.Item2 - x.Item2) * (y.Item2 - x.Item2)))));
            hierarchical.CLINK(out int[] PI, out byte[] LA);

            //int missedDistance = 0;
            //for (int i = 0; i < PI.Length - 1; i++)
            //{
            //    Console.WriteLine($"{i} - {PI[i]}: {LA[i]}");
            //    //missedDistance += LA[i];
            //}
            //Console.WriteLine("Missed distance: " + missedDistance);

            var retval = hierarchical.Consol(points, PI, LA, 20);
            foreach (var item in retval)
            {
                Console.WriteLine($" - [{String.Join(", ", item.ToArray())}]");
                Console.WriteLine();
            }
            
            Console.WriteLine($"Completed in: {DateTime.Now - currentTime}");
            Console.ReadLine();
        }
    }
}
