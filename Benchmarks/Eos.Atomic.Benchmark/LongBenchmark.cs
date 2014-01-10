using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Eos.Atomic.Benchmark
{
    public static class LongBenchmark
    {
        private static readonly object LockObj = new object();
        private static long _interlocked;
        private static AtomicLong _atomic;
        private static AtomicLongPadded _atomicPadded;
        private static long _shared;

        public static void Execute(int totalOfTasks, int expectedTotalIterations)
        {
            Console.WriteLine("Long benchmark\n");
            Console.WriteLine(ProfileResult.GetHeader());

            // 1. Run Shared 1 Task
            var result = Profiler.Profile(
                        () => Profiler.UseTasks(1, expectedTotalIterations, AddNoThreadSharing),
                        "Shared 1 Task",
                        () =>
                        {
                            var tmp = _shared;
                            _shared = 0;
                            return tmp.ToString("N0");
                        });

            Console.WriteLine(result);

            // 2. Atomic not padded N Task
            Profiler.Profiles(
                expectedTotalIterations,
                totalOfTasks,
                AddAtomic,
                () =>
                {
                    var tmp = _atomic.ReadAcquireFence();
                    _atomic = new AtomicLong();

                    return tmp.ToString("N0");
                },
                "Atomic {0:N0} Task (not padded)");

            // 3. Atomic padded N Task
            Profiler.Profiles(
                expectedTotalIterations,
                totalOfTasks,
                AddAtomicPadded,
                () =>
                {
                    var tmp = _atomicPadded.ReadAcquireFence();
                    _atomicPadded = new AtomicLongPadded();

                    return tmp.ToString("N0");
                },
                "Atomic padded {0:N0} Task");

            // 4. Simple interlock not wrapped N Task
            Profiler.Profiles(
                expectedTotalIterations,
                totalOfTasks,
                AddSimpleInterlocked,
                () =>
                {
                    var tmp = _interlocked;
                    _interlocked = 0;

                    return tmp.ToString("N0");
                },
                "Simple interlocked {0:N0} Task");

            // 5. Monitor N Task 
            Profiler.Profiles(
                expectedTotalIterations,
                totalOfTasks,
                AddWithLock,
                () =>
                {
                    var tmp = _shared;
                    _shared = 0;

                    return tmp.ToString("N0");
                },
                "Monitor/Lock {0:N0} Task");

            Console.WriteLine();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAtomicPadded(int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                _atomicPadded.IncrementAndGet();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAtomic(int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                _atomic.IncrementAndGet();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddWithLock(int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                lock (LockObj)
                {
                    _shared++;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddSimpleInterlocked(int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                Interlocked.Increment(ref _interlocked);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddNoThreadSharing(int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                _shared++;
            }
        }
    }
}
