using System;
using System.Threading;

using Eos.Atomic;
using Eos.Utils;

namespace Eos.DataStructures
{
    public class RingBuffer<T>
    {
        /// <summary>
        /// The buffer capacity
        /// </summary>
        private readonly int _capacity;

        /// <summary>
        /// The array that will contain all the elements
        /// </summary>
        private readonly T[] _buffer;

        /// <summary>
        /// The head of the queue
        /// </summary>
        private AtomicInt _headIndex;

        /// <summary>
        /// The available slot to be commited
        /// </summary>
        private AtomicInt _writeIndex;

        /// <summary>
        /// The unreserved slot
        /// </summary>
        private AtomicInt _availableSlotIndex;

        public RingBuffer(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException("capacity", capacity, "Must be greater than 0");
            }

            _capacity = capacity.NextPowerOfTwo();
            _buffer = new T[_capacity];
            _headIndex = new AtomicInt();
            _writeIndex = new AtomicInt();
            _availableSlotIndex = new AtomicInt();
        }

        public int Capacity
        {
            get
            {
                return _capacity;
            }
        }

        public int HeadIndex
        {
            get
            {
                return _headIndex.ReadAcquireFence();
            }
        }

        public int WriteIndex
        {
            get
            {
                return _writeIndex.ReadAcquireFence();
            }
        }

        public int AvailableSlotIndex
        {
            get
            {
                return _availableSlotIndex.ReadAcquireFence();
            }
        }

        public void Add(T item)
        {
            var reservedSlotIndex = StageSlot();

            CommitSlot(reservedSlotIndex, item);
        }

        private int StageSlot()
        {
            int reservedSlotIndex;
            _availableSlotIndex.ModuleAndGet(Capacity, out reservedSlotIndex);

            return reservedSlotIndex;
        }

        private void CommitSlot(int index, T item)
        {
            var spinWait = new SpinWait();
            var nextWriteIndex = index % Capacity;

            while (_writeIndex.CompareExchange(nextWriteIndex, index) != index)
            {
                spinWait.SpinOnce();
            }

            _buffer[index] = item;
        }
    }
}
