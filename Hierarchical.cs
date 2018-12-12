using System;
using System.Collections.Generic;
using System.Linq;

namespace Clustering
{
    public class Hierarchical<T>
    {
        public T[] Points { get; set; }
        public Func<T, T, byte> Comparer { get; set; }

        public Hierarchical(T[] points, Func<T, T, byte> comparer)
        {
            Points = points;
            Comparer = comparer;
        }

        /// <summary>
        /// SLINK alogorithm implemented with https://grid.cs.gsu.edu/~wkim/index_files/papers/sibson.pdf.
        /// This produces the same result as a naive single linkage clustering algorithm.
        /// </summary>
        public void SLINK(out int[] PI, out byte[] LA)
        {
            int length = Points.Length;
            PI = new int[length];
            LA = new byte[length];
            var MU = new byte[length];

            for (int i = 0; i < length; i++)
            {
                PI[i] = i;
                LA[i] = Byte.MaxValue;

                for (int j = 0; j < i; j++)
                    MU[j] = Comparer(Points[j], Points[i]);

                for (int j = 0; j < i; j++)
                {
                    if (LA[j] < MU[j])
                    {
                        if (MU[j] < MU[PI[j]])
                            MU[PI[j]] = MU[j];
                    }
                    else
                    {
                        if (LA[j] < MU[PI[j]])
                            MU[PI[j]] = LA[j];
                        LA[j] = MU[j];
                        PI[j] = i;
                    }
                }

                for (int j = 0; j < i; j++)
                    if (LA[j] >= LA[PI[j]])
                        PI[j] = i;
            }
        }

        /// <summary>
        /// CLINK alogorithm implemented with http://comjnl.oxfordjournals.org/content/20/4/364.full.pdf.
        /// This APPROXIMATES the result from complete clustering,
        /// and DOES NOT produce the same result as a naive complete linkage clustering algorithm.
        /// </summary>
        public void CLINK(out int[] PI, out byte[] LA)
        {
            int length = Points.Length;
            PI = new int[length];
            LA = new byte[length];
            var MU = new byte[length];

            int a;
            int b;
            byte c;
            int d;
            byte e;

            LA[0] = Byte.MaxValue;

            for (int i = 1; i < length; i++)
            {
                PI[i] = i;
                LA[i] = Byte.MaxValue;

                for (int j = 0; j < i; j++)
                    MU[j] = Comparer(Points[j], Points[i]);

                for (int j = 0; j < i; j++)
                {
                    if (LA[j] < MU[j])
                    {
                        if (MU[j] > MU[PI[j]])
                            MU[PI[j]] = MU[j];
                        MU[j] = Byte.MaxValue;
                    }
                }

                a = i - 1;

                for (int j = 0; j < i; j++)
                {
                    if (LA[i - j - 1] < MU[PI[i - j - 1]])
                        MU[i - j - 1] = Byte.MaxValue;
                    else if (MU[i - j - 1] < MU[a])
                        a = i - j - 1;
                }

                b = PI[a];
                c = LA[a];
                PI[a] = i;
                LA[a] = MU[a];

                while (a < i - 1)
                {
                    if (b < i - 1)
                    {
                        d = PI[b];
                        e = LA[b];
                        PI[b] = i;
                        LA[b] = c;
                        b = d;
                        c = e;
                    }
                    else
                    {
                        if (b == i - 1)
                        {
                            PI[b] = i;
                            LA[b] = c;
                        }

                        break;
                    }
                }
                
                for (int j = 0; j < i; j++)
                    if (PI[PI[j]] == i)
                        if (LA[j] >= LA[PI[j]])
                            PI[j] = i;
            }
        }

        public T[][] Consol(T[] points, int[] PI, byte[] LA, byte threshold)
        {
            int length = PI.Length;
            (int index, int group) [] retval = new (int, int)[length];
            for (int i = 0; i < length; i++)
                retval[i] = (i, i);
            for (int i = length - 2; i >= 0; i--)
            {
                if (LA[i] >= threshold)
                    continue;
                retval[i].group = retval[PI[i]].group;
            }
            return retval.GroupBy((i => i.group), (i => points[i.index])).Select(g => g.ToArray()).ToArray();
        }
    }
}
