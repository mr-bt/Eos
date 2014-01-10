using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Eos.Atomic
{
    /// <summary>
    /// Allows an reference to be updated atomically using the memory barrier semantics.
    /// <para>It prevents false sharing by ensuring that an instance will live on it's own cache line.</para>
    /// <para><seealso cref="http://www.drdobbs.com/parallel/eliminate-false-sharing/217500206?pgno=4"/></para>
    /// </summary>
    [DebuggerDisplay("{_value}")]
    [StructLayout(LayoutKind.Explicit, Size = AtomicConstants.CacheLineSize * 2)]
    public struct AtomicReferencePadded<T> : IEquatable<AtomicReferencePadded<T>>
        where T : class
    {
        [FieldOffset(AtomicConstants.CacheLineSize)]
        private T _value;

        /// <summary>
        /// Create a new <see cref="AtomicReferencePadded{T}" /> with the given initial value.
        /// </summary>
        /// <param name="value">Initial value.</param>
        public AtomicReferencePadded(T value)
        {
            _value = value;
        }

        public static bool operator ==(AtomicReferencePadded<T> left, AtomicReferencePadded<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AtomicReferencePadded<T> left, AtomicReferencePadded<T> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Read the value without applying any fence.
        /// </summary>
        /// <returns>The current value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public T ReadUnfenced()
        {
            return _value;
        }

        /// <summary>
        /// Read the value applying acquire fence semantic.
        /// </summary>
        /// <returns>The most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public T ReadAcquireFence()
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
        public T ReadFullFence()
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
        public T ReadCompilerOnlyFence()
        {
            return _value;
        }

        /// <summary>
        /// Write the value applying release fence semantic.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteReleaseFence(T newValue)
        {
            Interlocked.MemoryBarrier();
            _value = newValue;
        }

        /// <summary>
        /// Write the value applying full fence semantic.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteFullFence(T newValue)
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
        public void WriteCompilerOnlyFence(T newValue)
        {
            _value = newValue;
        }

        /// <summary>
        /// Write without applying any fence.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteUnfenced(T newValue)
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
        public T CompareExchange(T newValue, T comparand)
        {
            return Interlocked.CompareExchange(ref _value, newValue, comparand);
        }

        /// <summary>
        /// Atomically set the value to the given updated value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>The original value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public T Exchange(T newValue)
        {
            return Interlocked.Exchange(ref _value, newValue);
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
        public bool Equals(AtomicReferencePadded<T> other)
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

            return obj is AtomicReferencePadded<T> && Equals((AtomicReferencePadded<T>)obj);
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
    }
}
