using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using HandyCollections.Heap;

namespace ConsoleBenchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Program>();
            Console.ReadLine();
        }

        private void Check(uint u, int i)
        {
            var union = new Union32 { A = i };

            if (union.B != u)
                throw new NotImplementedException();
        }

        [Benchmark]
        public void Ptr()
        {
            for (int i = -1000; i < 1000; i++)
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
        public void Cast()
        {
            for (int i = -1000; i < 1000; i++)
            {
                uint x = unchecked((uint)i);

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
        public void Union()
        {
            Union32 u = new Union32();

            for (int i = -1000; i < 1000; i++)
            {
                u.A = i;

                uint x = u.B;

                Check(x, i);
            }
        }
    }
}
