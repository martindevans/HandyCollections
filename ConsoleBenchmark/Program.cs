using System;
using BenchmarkDotNet.Running;

namespace ConsoleBenchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Program>();
            Console.ReadLine();
        }

        public Program()
        {
        }
    }
}
