using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Eos.Atomic
{
    /// <summary>
    /// Allows an reference to be updated atomically using the memory barrier semantics.
    /// </summary>
    public struct AtomicReference<T> : IEquatable<AtomicReference<T>>
        where T : class
    {
        private T _value;

        /// <summary>
        /// Create a new <see cref="AtomicReference{T}" /> with the given initial value.
        /// </summary>
        /// <param name="value">Initial value.</param>
        public AtomicReference(T value)
        {
            _value = value;
        }

        public static bool operator ==(AtomicReference<T> left, AtomicReference<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AtomicReference<T> left, AtomicReference<T> right)
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

        public bool Equals(AtomicReference<T> other)
        {
            return ReadFullFence() == other.ReadFullFence();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is AtomicReference<T> && Equals((AtomicReference<T>)obj);
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
