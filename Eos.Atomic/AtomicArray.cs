﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Eos.Atomic
{
    /// <summary>
    /// Allows an reference array to be updated atomically using the memory barrier semantics.
    /// </summary>
    public struct AtomicArray<T> : IEnumerable<T>
        where T : class
    {
        private readonly T[] _array;

        /// <summary>
        /// Create a reference array <see cref="AtomicReferenceArray{T}" /> copying the values from the source array.
        /// </summary>
        /// <param name="array">Source array.</param>
        public AtomicArray(T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Length == 0)
            {
                throw new ArgumentOutOfRangeException("array", "Array length must be greater than zero");
            }

            _array = new T[array.Length];
            Array.Copy(array, _array, array.Length);
        }

        /// <summary>
        /// Create a new reference array <see cref="AtomicReferenceArray{T}" /> with the specified length.
        /// </summary>
        /// <param name="length">Array length.</param>
        public AtomicArray(int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("length", "Must be greater than 0");
            }

            _array = new T[length];
        }

        /// <summary>
        /// Length of the array.
        /// </summary>
        public int Length
        {
            get { return _array.Length; }
        }

        [IndexerName("Array")]
        public T this[int index]
        {
            get
            {
                return ReadAcquireFence(index);
            }

            set
            {
                WriteReleaseFence(index, value);
            }
        }

        /// <summary>
        /// Read the value without applying any fence.
        /// </summary>
        /// <returns>The current value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public T ReadUnfenced(int index)
        {
            return _array[index];
        }

        /// <summary>
        /// Read the value applying acquire fence semantic.
        /// </summary>
        /// <returns>The most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public T ReadAcquireFence(int index)
        {
            var value = _array[index];
            Interlocked.MemoryBarrier();

            return value;
        }

        /// <summary>
        /// Read the value applying full fence semantic.
        /// </summary>
        /// <returns>The most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public T ReadFullFence(int index)
        {
            Interlocked.MemoryBarrier();
            var value = _array[index];
            Interlocked.MemoryBarrier();

            return value;
        }

        /// <summary>
        /// Read the value applying a compiler only fence, no CPU fence is applied.
        /// </summary>
        /// <returns>The current value.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public T ReadCompilerOnlyFence(int index)
        {
            return _array[index];
        }

        /// <summary>
        /// Write the value applying release fence semantic.
        /// </summary>
        /// <param name="index">Array index.</param>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteReleaseFence(int index, T newValue)
        {
            Interlocked.MemoryBarrier();
            _array[index] = newValue;
        }

        /// <summary>
        /// Write the value applying full fence semantic.
        /// </summary>
        /// <param name="index">Array index.</param>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteFullFence(int index, T newValue)
        {
            Interlocked.MemoryBarrier();
            _array[index] = newValue;
            Interlocked.MemoryBarrier();
        }

        /// <summary>
        /// Write the value applying a compiler fence only, no CPU fence is applied.
        /// </summary>
        /// <param name="index">Array index.</param>
        /// <param name="newValue">The new value</param>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void WriteCompilerOnlyFence(int index, T newValue)
        {
            _array[index] = newValue;
        }

        /// <summary>
        /// Write without applying any fence.
        /// </summary>
        /// <param name="index">Array index.</param>
        /// <param name="newValue">The new value.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void WriteUnfenced(int index, T newValue)
        {
            _array[index] = newValue;
        }

        /// <summary>
        /// Atomically set the value to the given updated value if the current value equals the comparand.
        /// </summary>
        /// <param name="index">Array index.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comparand">The comparand (expected current value).</param>
        /// <returns>The original value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public T CompareExchange(int index, T newValue, T comparand)
        {
            return Interlocked.CompareExchange(ref _array[index], newValue, comparand);
        }

        /// <summary>
        /// Atomically set the value to the given updated value.
        /// </summary>
        /// <param name="index">Array index.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The original value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public T Exchange(int index, T newValue)
        {
            return Interlocked.Exchange(ref _array[index], newValue);
        }

        public override int GetHashCode()
        {
            return _array.GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(AtomicArray<T> other)
        {
            return ReferenceEquals(_array, other._array);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is AtomicArray<T> && Equals((AtomicArray<T>)obj);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var i = new AtomicInt(-1);
            var index = 0;
            do
            {
                index = i.IncrementAndGet();
                yield return _array[index];
            }
            while (index != _array.Length);
        }

        /// <summary>
        /// Returns the string representation of the current value.
        /// </summary>
        /// <returns>The string representation of the most up-to-date value.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public override string ToString()
        {
            return _array.ToString();
        }
    }
}
