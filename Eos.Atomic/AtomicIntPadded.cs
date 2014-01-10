using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Eos.Atomic
{
    /// <summary>
    /// Allows an interger (32-bit) to be updated atomically using the memory barrier semantics. 
    /// <para>It prevents false sharing by ensuring that an instance will live on it's own cache line.</para>
    /// <para><seealso cref="http://www.drdobbs.com/parallel/eliminate-false-sharing/217500206?pgno=4"/></para>
    /// <para>For complext operations (e.g. multiplication) it's used non-blocking approach with 
    /// SpinWait and Interlocked operations.</para>
    /// </summary>
    [DebuggerDisplay("{_value}")]
    [StructLayout(LayoutKind.Explicit, Size = AtomicConstants.CacheLineSize * 2)]
    public struct AtomicIntPadded : IComparable<AtomicIntPadded>, IEquatable<AtomicIntPadded>
    {
        [FieldOffset(AtomicConstants.CacheLineSize)]
        private int _value;

        /// <summary>
        /// Create a new <see cref="AtomicIntPadded"/> with the given initial value.
        /// </summary>
        /// <param name="value">Initial value.</param>
        public AtomicIntPadded(int value)
        {
            _value = value;
        }

        public static bool operator ==(AtomicIntPadded left, AtomicIntPadded right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AtomicIntPadded left, AtomicIntPadded right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Read the value without applying any fence.
        /// </summary>
        /// <returns>The current value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int ReadUnfenced()
        {
            return _value;
        }

        /// <summary>
        /// Read the value applying acquire fence semantic.
        /// </summary>
        /// <returns>The most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int ReadAcquireFence()
        {
            var value = _value;
            Interlocked.MemoryBarrier();

            return value;
        }

        /// <summary>
        /// Read the value applying full fence semantic.
        /// </summary>
        /// <returns>The most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int ReadFullFence()
        {
            Interlocked.MemoryBarrier();
            var value = _value;
            Interlocked.MemoryBarrier();

            return value;
        }

        /// <summary>
        /// Read the value applying a compiler only fence, no CPU fence is applied.
        /// </summary>
        /// <returns>The current value.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public int ReadCompilerOnlyFence()
        {
            return _value;
        }

        /// <summary>
        /// Write the value applying release fence semantic.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteReleaseFence(int newValue)
        {
            Interlocked.MemoryBarrier();
            _value = newValue;
        }

        /// <summary>
        /// Write the value applying full fence semantic.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteFullFence(int newValue)
        {
            Interlocked.MemoryBarrier();
            _value = newValue;
            Interlocked.MemoryBarrier();
        }

        /// <summary>
        /// Write the value applying a compiler fence only, no CPU fence is applied.
        /// </summary>
        /// <param name="newValue">The new value</param>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void WriteCompilerOnlyFence(int newValue)
        {
            _value = newValue;
        }

        /// <summary>
        /// Write without applying any fence.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteUnfenced(int newValue)
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
        public int CompareExchange(int newValue, int comparand)
        {
            return Interlocked.CompareExchange(ref _value, newValue, comparand);
        }

        /// <summary>
        /// Atomically set the value to the given updated value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>The original value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int Exchange(int newValue)
        {
            return Interlocked.Exchange(ref _value, newValue);
        }

        /// <summary>
        /// Atomically add the given value to the current value and return the sum.
        /// </summary>
        /// <param name="delta">The value to be added.</param>
        /// <returns>The sum of the current value and the given value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int AddAndGet(int delta)
        {
            return Interlocked.Add(ref _value, delta);
        }

        /// <summary>
        /// Atomically increment the current value and return the new value.
        /// </summary>
        /// <returns>The incremented value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int IncrementAndGet()
        {
            return Interlocked.Increment(ref _value);
        }

        /// <summary>
        /// Atomically decrements the current value and return the new value.
        /// </summary>
        /// <returns>The decremented value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int DecrementAndGet()
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
        public int ModuleAndGet(int secondOperand, out int originalValue)
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
        /// Atomically calculates the multiplication of the current value by the secondOperand.
        /// <para>To ensure the atomicity it relies on a SpinWait and a CompareExchange.</para>
        /// </summary>
        /// <param name="secondOperand">The second operand of a multiplication operation.</param>
        /// <param name="originalValue">The original value.</param>
        /// <returns>The multiplication of the current value by the the secondOperand.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int MultiplyAndGet(int secondOperand, out int originalValue)
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
        public bool Equals(AtomicIntPadded other)
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

            return obj is AtomicIntPadded && Equals((AtomicIntPadded)obj);
        }

        /// <summary>
        /// Compares two AtomicIntPadded values using the full fence semantic.
        /// </summary>
        /// <param name="other">The comparand.</param>
        /// <returns>
        /// <para>Less than zero: This instance precedes obj in the sort order.</para>
        /// <para>Zero: This instance occurs in the same position in the sort order as obj.</para>
        /// <para>Greater than zero: This instance follows obj in the sort order.</para>
        /// </returns>
        public int CompareTo(AtomicIntPadded other)
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
