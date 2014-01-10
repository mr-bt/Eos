using System;

using Eos.Utils;

namespace Eos.Atomic.Benchmark
{
    public class Program
    {
        private const int TotalOfTasks = 16;
        private const int MinExpectedTotalIterations = 400000;

        public static void Main(string[] args)
        {
            var expectedTotalIterations = MinExpectedTotalIterations.NextPowerOfTwo();
            Console.WriteLine("Total iterations {0:N0}", expectedTotalIterations);

            LongBenchmark.Execute(TotalOfTasks, expectedTotalIterations);
            IntBenchmark.Execute(TotalOfTasks, expectedTotalIterations);

            Console.WriteLine("End");
        }
    }
}
