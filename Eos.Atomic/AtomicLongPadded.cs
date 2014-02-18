using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Eos.Atomic
{
    /// <summary>
    /// Allows an long (64-bit) to be updated atomically using the memory barrier semantics.
    /// <para>For complext operations (e.g. multiplication) it's used non-blocking approach with 
    /// SpinWait and Interlocked operations.</para>
    /// <para>It prevents false sharing by ensuring that an instance will live on it's own cache line.</para>
    /// <para><seealso cref="http://www.drdobbs.com/parallel/eliminate-false-sharing/217500206?pgno=4"/></para>
    /// <para>NOTE: Running on a 32-bits CPU and using CPU fences always use full fence to read/write 
    /// as the CPU will perform two operations instead of 1 and then not atomic.</para>
    /// </summary>
    [DebuggerDisplay("{_value}")]
    [StructLayout(LayoutKind.Explicit, Size = AtomicConstants.CacheLineSize * 2)]
    public struct AtomicLongPadded : IComparable<AtomicLongPadded>, IEquatable<AtomicLongPadded>
    {
        [FieldOffset(AtomicConstants.CacheLineSize)]
        private long _value;

        /// <summary>
        /// Create a new <see cref="AtomicLongPadded"/> with the given initial value.
        /// </summary>
        /// <param name="value">Initial value.</param>
        public AtomicLongPadded(long value)
        {
            _value = value;
        }

        public static bool operator ==(AtomicLongPadded left, AtomicLongPadded right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AtomicLongPadded left, AtomicLongPadded right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Read the value without applying any fence.
        /// </summary>
        /// <returns>The current value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long ReadUnfenced()
        {
            return _value;
        }

        /// <summary>
        /// Read the value applying acquire fence semantic.
        /// <para>NOTE: Only suitable for 64-bits CPUs as on 32-bits CPU it will be perform two operations.</para>
        /// </summary>
        /// <returns>The most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long ReadAcquireFence()
        {
            var value = _value;
            Thread.MemoryBarrier();

            return value;
        }

        /// <summary>
        /// Read the value applying full fence semantic.
        /// </summary>
        /// <returns>The most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long ReadFullFence()
        {
            // using the Interlocked.Read() uses the same approach (Interlocked.CompareExchange)
            // however the comparand is 0L, meaning that it's more common to the value being 0L than 
            // long.MinValue and then not doing the value exchange
            return Interlocked.CompareExchange(ref _value, long.MinValue, long.MinValue);
        }

        /// <summary>
        /// Read the value applying a compiler only fence, no CPU fence is applied.
        /// </summary>
        /// <returns>The current value.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public long ReadCompilerOnlyFence()
        {
            return _value;
        }

        /// <summary>
        /// Write the value applying release fence semantic.
        /// <para>NOTE: Only suitable for 64-bits CPUs as on 32-bits CPU it will be perform two operations.</para>
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteReleaseFence(long newValue)
        {
            Thread.MemoryBarrier();
            _value = newValue;
        }

        /// <summary>
        /// Write the value applying full fence semantic.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteFullFence(long newValue)
        {
            Interlocked.Exchange(ref _value, newValue);
        }

        /// <summary>
        /// Write the value applying a compiler fence only, no CPU fence is applied.
        /// </summary>
        /// <param name="newValue">The new value</param>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void WriteCompilerOnlyFence(long newValue)
        {
            _value = newValue;
        }

        /// <summary>
        /// Write without applying any fence.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteUnfenced(long newValue)
        {
            _value = newValue;
        }

        /// <summary>
        /// Atomically set the value to the given updated value if the current value equals the comparand.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="comparand">The comparand (expected current value).</param>
        /// <returns>The original value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long CompareExchange(long newValue, long comparand)
        {
            return Interlocked.CompareExchange(ref _value, newValue, comparand);
        }

        /// <summary>
        /// Atomically set the value to the given updated value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>The original value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long Exchange(long newValue)
        {
            return Interlocked.Exchange(ref _value, newValue);
        }

        /// <summary>
        /// Atomically add the given value to the current value and return the sum.
        /// </summary>
        /// <param name="delta">The value to be added.</param>
        /// <returns>The sum of the current value and the given value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long AddAndGet(long delta)
        {
            return Interlocked.Add(ref _value, delta);
        }

        /// <summary>
        /// Atomically increment the current value and return the new value.
        /// </summary>
        /// <returns>The incremented value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long IncrementAndGet()
        {
            return Interlocked.Increment(ref _value);
        }

        /// <summary>
        /// Atomically decrements the current value and return the new value.
        /// </summary>
        /// <returns>The decremented value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long DecrementAndGet()
        {
            return Interlocked.Decrement(ref _value);
        }

        /// <summary>
        /// Atomically calculates the module of the current value by the secondOperand.
        /// <para>To ensure the atomicity it relies on a SpinWait and a CompareExchange.</para>
        /// </summary>
        /// <param name="secondOperand">The second operand of a module operation.</param>
        /// <param name="originalValue">The original value.</param>
        /// <returns>The module of the current value by the the secondOperand.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long ModuleAndGet(long secondOperand, out long originalValue)
        {
            var spinWait = new SpinWait();

            do
            {
                var snapshot = ReadAcquireFence();
                var nextPosition = snapshot % secondOperand;

                originalValue = CompareExchange(nextPosition, snapshot);
                if (snapshot == originalValue)
                {
                    return nextPosition;
                }

                spinWait.SpinOnce();
            }
            while (true);
        }

        /// <summary>
        /// Atomically increments the current value and calculates the module with the secondOperand.
        /// <para>To ensure the atomicity it relies on a SpinWait and a CompareExchange.</para>
        /// </summary>
        /// <param name="secondOperand">The second operand of a module operation.</param>
        /// <param name="originalValue">The original value.</param>
        /// <returns>The module of the current value by the the secondOperand.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long IncrementModuleAndGet(long secondOperand, out long originalValue)
        {
            return AddModuleAndGet(1, secondOperand, out originalValue);
        }

        /// <summary>
        /// Atomically add delta to the current value and calculates the module with the secondOperand.
        /// <para>To ensure the atomicity it relies on a SpinWait and a CompareExchange.</para>
        /// </summary>
        /// <param name="delta">The value to be added.</param>
        /// <param name="secondOperand">The second operand of a module operation.</param>
        /// <param name="originalValue">The original value.</param>
        /// <returns>The module of the current value by the the secondOperand.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long AddModuleAndGet(long delta, long secondOperand, out long originalValue)
        {
            var spinWait = new SpinWait();

            do
            {
                var snapshot = ReadAcquireFence();
                var nextPosition = (snapshot + delta) % secondOperand;

                originalValue = CompareExchange(nextPosition, snapshot);
                if (snapshot == originalValue)
                {
                    return nextPosition;
                }

                spinWait.SpinOnce();
            }
            while (true);
        }

        /// <summary>
        /// Atomically calculates the multiplication of the current value by the secondOperand.
        /// <para>To ensure the atomicity it relies on a SpinWait and a CompareExchange.</para>
        /// </summary>
        /// <param name="secondOperand">The second operand of a multiplication operation.</param>
        /// <param name="originalValue">The original value.</param>
        /// <returns>The multiplication of the current value by the the secondOperand.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public long MultiplyAndGet(long secondOperand, out long originalValue)
        {
            var spinWait = new SpinWait();

            do
            {
                var snapshot = ReadAcquireFence();

                var nextPosition = snapshot * secondOperand;

                originalValue = CompareExchange(nextPosition, snapshot);
                if (snapshot == originalValue)
                {
                    return nextPosition;
                }

                spinWait.SpinOnce();
            }
            while (true);
        }

        public override int GetHashCode()
        {
            return ReadFullFence().GetHashCode();
        }

        /// <summary>
        /// Return if the instances are equal using full fence semantic.
        /// </summary>
        /// <param name="other">The comparand.</param>
        /// <returns>
        /// <para>True if equals.</para>
        /// <para>False if distinct.</para>
        /// </returns>
        public bool Equals(AtomicLongPadded other)
        {
            return ReadFullFence() == other.ReadFullFence();
        }

        /// <summary>
        /// Return if the instances are equal using full fence semantic.
        /// </summary>
        /// <param name="obj">The comparand.</param>
        /// <returns>
        /// <para>True if equals.</para>
        /// <para>False if distinct or null.</para>
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is AtomicLongPadded && Equals((AtomicLongPadded)obj);
        }

        /// <summary>
        /// Compares two AtomicLongPadded values using the full fence semantic.
        /// </summary>
        /// <param name="other">The comparand.</param>
        /// <returns>
        /// <para>Less than zero: This instance precedes obj in the sort order.</para>
        /// <para>Zero: This instance occurs in the same position in the sort order as obj.</para>
        /// <para>Greater than zero: This instance follows obj in the sort order.</para>
        /// </returns>
        public int CompareTo(AtomicLongPadded other)
        {
            return ReadFullFence().CompareTo(other.ReadAcquireFence());
        }

        /// <summary>
        /// Returns the string representation of the current value.
        /// </summary>
        /// <returns>The string representation of the most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public override string ToString()
        {
            var value = ReadFullFence();

            return value.ToString();
        }

        /// <summary>
        /// Returns the string representation of the current value.
        /// </summary>
        /// <param name="provider">The string formater provider.</param>
        /// <returns>The string representation of the most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public string ToString(IFormatProvider provider)
        {
            var value = ReadFullFence();

            return value.ToString(provider);
        }

        /// <summary>
        /// Returns the string representation of the current value.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <returns>The string representation of the most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public string ToString(string format)
        {
            var value = ReadFullFence();

            return value.ToString(format);
        }

        /// <summary>
        /// Returns the string representation of the current value.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="provider">The string formater provider.</param>
        /// <returns>The string representation of the most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public string ToString(string format, IFormatProvider provider)
        {
            var value = ReadFullFence();

            return value.ToString(format, provider);
        }
    }
}
