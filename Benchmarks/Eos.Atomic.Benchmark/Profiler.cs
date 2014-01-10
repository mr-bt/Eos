using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Eos.Utils;

namespace Eos.Atomic.Benchmark
{
    public static class Profiler
    {
        public static ProfileResult Profile(
            Action testFunc, string name, Func<string> captureAndCleanResult, int iterations = 1000)
        {
            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // warm up 
            testFunc();
            captureAndCleanResult();

            var watch = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                testFunc();
            }

            watch.Stop();

            var result = new ProfileResult
                             {
                                 Name = name,
                                 Elapsed = watch.Elapsed,
                                 Iteractions = iterations,
                                 Result = captureAndCleanResult()
                             };

            return result;
        }

        public static void Profiles(
            int expectedTotalIterations,
            int totalOfTasks,
            Action<int> testFunc,
            Func<string> captureAndCleanResult,
            string nameFormat,
            int iterations = 1000)
        {
            for (var runingTasks = 1;
                 runingTasks <= totalOfTasks;
                 runingTasks = runingTasks.NextPowerOfTwo())
            {
                var tasks = runingTasks;
                var iterationsPerThread = expectedTotalIterations / tasks;

                var result = Profile(
                    () => UseTasks(tasks, iterationsPerThread, testFunc),
                    string.Format(nameFormat, runingTasks),
                    captureAndCleanResult,
                    iterations);

                Console.WriteLine(result);

                runingTasks++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UseTasks(int tasksNumber, int iterations, Action<int> action)
        {
            var tasks = new Task[tasksNumber];
            for (var i = 0; i < tasksNumber; i++)
            {
                tasks[i] = new Task(
                    () => action(iterations),
                    TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);
                tasks[i].ConfigureAwait(false);
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
        }
    }
}
