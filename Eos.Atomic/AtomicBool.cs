using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Eos.Atomic
{
    /// <summary>
    /// Allows an bool to be updated atomically using the memory barrier semantics.
    /// <para>Note: The bool is converted into a 32-bit integer.</para>
    /// </summary>
    [DebuggerTypeProxy(typeof(AtomicBoolDebugView))]
    public struct AtomicBool : IComparable<AtomicBool>, IEquatable<AtomicBool>
    {
        private int _value;

        /// <summary>
        /// Create a new <see cref="AtomicBool" /> with the given initial value.
        /// </summary>
        /// <param name="value">Initial value.</param>
        public AtomicBool(bool value)
        {
            _value = ToInt(value);
        }

        public static bool operator ==(AtomicBool left, AtomicBool right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AtomicBool left, AtomicBool right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Read the value without applying any fence.
        /// </summary>
        /// <returns>The current value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public bool ReadUnfenced()
        {
            return ToBool(_value);
        }

        /// <summary>
        /// Read the value applying acquire fence semantic.
        /// </summary>
        /// <returns>The most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public bool ReadAcquireFence()
        {
            var value = _value;
            Interlocked.MemoryBarrier();

            return ToBool(value);
        }

        /// <summary>
        /// Read the value applying full fence semantic.
        /// </summary>
        /// <returns>The most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public bool ReadFullFence()
        {
            Interlocked.MemoryBarrier();
            var value = _value;
            Interlocked.MemoryBarrier();

            return ToBool(value);
        }

        /// <summary>
        /// Read the value applying a compiler only fence, no CPU fence is applied.
        /// </summary>
        /// <returns>The current value.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public bool ReadCompilerOnlyFence()
        {
            return ToBool(_value);
        }

        /// <summary>
        /// Write the value applying release fence semantic.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteReleaseFence(bool newValue)
        {
            Interlocked.MemoryBarrier();
            _value = ToInt(newValue);
        }

        /// <summary>
        /// Write the value applying full fence semantic.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteFullFence(bool newValue)
        {
            Interlocked.MemoryBarrier();
            _value = ToInt(newValue);
            Interlocked.MemoryBarrier();
        }

        /// <summary>
        /// Write the value applying a compiler fence only, no CPU fence is applied.
        /// </summary>
        /// <param name="newValue">The new value</param>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void WriteCompilerOnlyFence(bool newValue)
        {
            _value = ToInt(newValue);
        }

        /// <summary>
        /// Write without applying any fence.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteUnfenced(bool newValue)
        {
            _value = ToInt(newValue);
        }

        /// <summary>
        /// Atomically set the value to the given updated value if the current value equals the comparand.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="comparand">The comparand (expected current value).</param>
        /// <returns>The original value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public bool CompareExchange(bool newValue, bool comparand)
        {
            var originalValue = Interlocked.CompareExchange(ref _value, ToInt(newValue), ToInt(comparand));

            return ToBool(originalValue);
        }

        /// <summary>
        /// Atomically set the value to the given updated value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>The original value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public bool Exchange(bool newValue)
        {
            var originalValue = Interlocked.Exchange(ref _value, ToInt(newValue));

            return ToBool(originalValue);
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
        public bool Equals(AtomicBool other)
        {
            return ReadFullFence() == other.ReadFullFence();
        }

        /// <summary>
        /// Return if the instances are equal using full fence semantic.
        /// </summary>
        /// <param name="obj">The comparand.</param>
        /// <returns>
        /// <para>True if equals.</para>
        /// <para>False if distinct.</para>
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is AtomicBool && Equals((AtomicBool)obj);
        }

        /// <summary>
        /// Compares two AtomicBool values using the full fence semantic.
        /// </summary>
        /// <param name="other">The comparand.</param>
        /// <returns>
        /// <para>Less than zero: This instance precedes obj in the sort order.</para>
        /// <para>Zero: This instance occurs in the same position in the sort order as obj.</para>
        /// <para>Greater than zero: This instance follows obj in the sort order.</para>
        /// </returns>
        public int CompareTo(AtomicBool other)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ToBool(int value)
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException("value", "Must be 0 or 1");
            }

            return value != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ToInt(bool value)
        {
            return value ? 1 : 0;
        }

        internal class AtomicBoolDebugView
        {
            internal AtomicBoolDebugView(AtomicBool value)
            {
                value = value;
            }

            public bool Value { get; set; }
        }
    }
}
