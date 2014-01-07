using System;
using System.Runtime;

namespace Eos.Utils
{
    /// <summary>
    /// Math utils summary
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Round up to the next highest power of 2 of integer value.
        /// <para>Source 2014.01.02: <seealso cref="http://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2" /></para>
        /// </summary>
        /// <param name="value">Value to round up.</param>
        /// <returns>Next highest power of 2 of 32-bit or returns the same value if it's power of two.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static int NextPowerOfTwo(this int value)
        {
            // zero is not a power of 2 as any negative number
            if (value < 1)
            {
                return 0;
            }

            const int MaxIntPowerOfTwo = 1073741824;
            if (value > MaxIntPowerOfTwo)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;

            return value;
        }

        /// <summary>
        /// Round up to the next highest power of two of long.
        /// <para>Source 2014.01.02: <seealso cref="http://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2" /></para>
        /// </summary>
        /// <param name="value">Value to round up.</param>
        /// <returns>Next highest power of 2 of 32-bit or returns the same value if it's power of two.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static long NextPowerOfTwo(this long value)
        {
            // zero is not a power of 2 as any negative number
            if (value < 1)
            {
                return 0;
            }

            const long MaxLongPowerOfTwo = 4611686018427387904;
            if (value > MaxLongPowerOfTwo)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            value++;

            return value;
        }

        /// <summary>
        /// Determining if an positive integer is a power of 2
        /// <para>Source 2014.01.02: <seealso cref="http://bits.stephan-brumme.com/isPowerOfTwo.html" /></para>
        /// </summary>
        /// <param name="value">Input value to check if it's power of two</param>
        /// <returns><b>True</b> if value is power of two</returns>
        /// <remarks>
        /// "In mathematics, a power of two means a number of the form 2n where n is an integer [...]
        /// In a context where only integers are considered, n is restricted to non-negative values,[1] 
        /// so we have 1, 2, and 2 multiplied by itself a certain number of times"
        /// <para>Source2014.01.02 <seealso cref="http://en.wikipedia.org/wiki/Power_of_two"/></para>
        /// </remarks>
        public static bool IsPowerOfTwo(this int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Determining if an positive long is a power of 2
        /// <para>Source 2014.01.02: <seealso cref="http://bits.stephan-brumme.com/isPowerOfTwo.html" /></para>
        /// </summary>
        /// <param name="value">Input value to check if it's power of two</param>
        /// <returns><b>True</b> if value is power of two</returns>
        /// <remarks>
        /// "In mathematics, a power of two means a number of the form 2n where n is an integer [...]
        /// In a context where only integers are considered, n is restricted to non-negative values,[1] 
        /// so we have 1, 2, and 2 multiplied by itself a certain number of times"
        /// <para>Source2014.01.02 <seealso cref="http://en.wikipedia.org/wiki/Power_of_two"/></para>
        /// </remarks>
        public static bool IsPowerOfTwo(this long value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Detect if two integers have opposite signs.
        /// <para>Source 2014.01.02: <seealso cref="http://graphics.stanford.edu/~seander/bithacks.html#DetectOppositeSigns" /></para>
        /// </summary>
        /// <param name="value1">First input value to compare signs.</param>
        /// <param name="value2">First input value to compare signs.</param>
        /// <returns><c>True</c> if the have oposite signs.</returns>
        public static bool HaveOppositeSigns(this int value1, int value2)
        {
            return (value1 ^ value2) < 0;
        }

        /// <summary>
        /// Detect if two longs have opposite signs.
        /// <para>Source 2014.01.02: <seealso cref="http://graphics.stanford.edu/~seander/bithacks.html#DetectOppositeSigns" /></para>
        /// </summary>
        /// <param name="value1">First input value to compare signs.</param>
        /// <param name="value2">First input value to compare signs.</param>
        /// <returns><c>True</c> if the have oposite signs.</returns>
        public static bool HaveOppositeSigns(this long value1, long value2)
        {
            return (value1 ^ value2) < 0;
        }

        /// <summary>
        /// It changes the sign of a integrer.
        /// </summary>
        /// <param name="value">Input value to change sign.</param>
        /// <returns>Return the input value with the inverse sign.</returns>
        public static int ChangeSign(this int value)
        {
            return value * -1;
        }

        /// <summary>
        /// It changes the sign of a long.
        /// </summary>
        /// <param name="value">Input value to change sign.</param>
        /// <returns>Return the input value with the inverse sign.</returns>
        public static long ChangeSign(this long value)
        {
            return value * -1;
        }

        /// <summary>
        /// Detecte if an integer is an even number.
        /// </summary>
        /// <param name="value">The value to detect it's even.</param>
        /// <returns><c>True</c> if and only if the value is even.</returns>
        public static bool IsEven(this int value)
        {
            return (value & 0x1) == 0x0;
        }

        /// <summary>
        /// Detecte if an long is an even number.
        /// </summary>
        /// <param name="value">The value to detect it's even.</param>
        /// <returns><c>True</c> if and only if the value is even.</returns>
        public static bool IsEven(this long value)
        {
            return (value & 0x1) == 0x0;
        }
    }
}