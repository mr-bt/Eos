using System;

namespace Eos.Atomic.Benchmark
{
    public class ProfileResult
    {
        public string Name { get; set; }

        public TimeSpan Elapsed { get; set; }

        public int Iteractions { get; set; }

        public string Result { get; set; }

        public static string GetHeader()
        {
            return string.Format("{0,-30} | {1,-14} | {2,-8} | {3,-12} | {4,-15}{5}-------------------------------------------------------------------------------------------------------",
                "Name",
                "Milliseconds",
                "Seconds",
                "Iteractions",
                "Result",
                Environment.NewLine);
        }

        public override string ToString()
        {
            return string.Format("{0,-30}   {1,-14:N4}   {2,-8:N4}   {3,-12:N0}   {4,-15}", Name, Elapsed.TotalMilliseconds, Elapsed.TotalSeconds, Iteractions, Result);
        }
    }
}
