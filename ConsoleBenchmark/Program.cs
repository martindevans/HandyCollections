using System;
using BenchmarkDotNet.Running;

namespace ConsoleBenchmark
{
    public class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<Casts>();
            Console.ReadLine();
        }

        
    }
}
