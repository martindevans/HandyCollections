using System;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace ConsoleBenchmark
{
    public class Casts
    {
        private static void Check(uint u, int i)
        {
            var union = new Union32 { A = i };

            if (union.B != u)
                throw new NotImplementedException();
        }

        [Benchmark]
        public static void Ptr()
        {
            for (var i = -1000; i < 1000; i++)
            {
                uint x;
                unsafe
                {
                    x = *(uint*)&i;
                }

                Check(x, i);
            }
        }

        [Benchmark]
        public static void Cast()
        {
            for (var i = -1000; i < 1000; i++)
            {
                var x = unchecked((uint)i);

                Check(x, i);
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct Union32
        {
            [FieldOffset(0)]
            public int A;

            [FieldOffset(0)]
            public uint B;
        }

        [Benchmark]
        public static void Union()
        {
            var u = new Union32();

            for (var i = -1000; i < 1000; i++)
            {
                u.A = i;

                var x = u.B;

                Check(x, i);
            }
        }
    }
}
