﻿using System;

using NUnit.Framework;

namespace Eos.Atomic.Tests
{
    [TestFixture]
    public class AtomicLongTests
    {
        [Test]
        public void CtorDefaultShouldBeZero()
        {
            var value = new AtomicLong();
            Assert.AreEqual(0L, value.ReadUnfenced());
        }

       [Test]
        public void CtorShouldBeMaxLongValue()
       {
           var value = new AtomicLong(long.MaxValue);
           Assert.AreEqual(long.MaxValue, value.ReadUnfenced());
       }

       [Test]
       public void CtorShouldBeMinLongValue()
       {
           var value = new AtomicLong(long.MinValue);
           Assert.AreEqual(long.MinValue, value.ReadUnfenced());
       }

        [Test]
        public void GetHashCodeShouldBeTheSetValue()
        {
            var value = new AtomicLong(1000);
            var result = value.GetHashCode();

            Assert.AreEqual(1000, result);
        }

        [Test]
        public void EqualsAtomicLongInputShouldReturnTrueOnSameValue()
        {
            var value1 = new AtomicLong(5);
            var value2 = new AtomicLong(5);

            Assert.IsTrue(value1.Equals(value2));
        }

        [Test]
        public void EqualsAtomicLongInputShouldReturnFalseOnDifferentValue()
        {
            var value1 = new AtomicLong(1);
            var value2 = new AtomicLong(2);

            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void EqualsObjectInputShouldReturnTrueOnSameValue()
        {
            var value1 = (object)new AtomicLong(1);
            var value2 = (object)new AtomicLong(1);

            Assert.IsTrue(value1.Equals(value2));
        }

        [Test]
        public void EqualsObjectInputShouldReturnFalseOnDifferentValue()
        {
            var value1 = (object)new AtomicLong(1);
            var value2 = (object)new AtomicLong(2);

            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void EqualsObjectInputShouldReturnFalseOnNullComparand()
        {
            var value1 = (object)new AtomicLong(1);
            object value2 = null;

            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void EqualsObjectInputShouldReturnFalseOnDifferentType()
        {
            var value1 = (object)new AtomicLong(1);
            var value2 = (object)DateTime.UtcNow;

            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void EqualsOperatorShouldReturnTrueOnSameValue()
        {
            var value1 = new AtomicLong(1);
            var value2 = new AtomicLong(1);

            Assert.IsTrue(value1 == value2);
        }

        [Test]
        public void EqualsOperatorShouldReturnFalseOnDifferentValues()
        {
            var value1 = new AtomicLong(1);
            var value2 = new AtomicLong(2);

            Assert.IsFalse(value1 == value2);
        }

        [Test]
        public void NotEqualsOperatorShouldReturnFalseOnSameValue()
        {
            var value1 = new AtomicLong(1);
            var value2 = new AtomicLong(1);

            Assert.IsFalse(value1 != value2);
        }

        [Test]
        public void NotEqualsOperatorShouldReturnTrueOnDifferentValues()
        {
            var value1 = new AtomicLong(1);
            var value2 = new AtomicLong(2);

            Assert.IsTrue(value1 != value2);
        }

        [Test]
        public void CompareToShouldReturnGreaterThanZeroOnTwoAndOne()
        {
            var value1 = new AtomicLong(2);
            var value2 = new AtomicLong(1);

            Assert.AreEqual(1, value1.CompareTo(value2));
        }

        [Test]
        public void CompareToShouldReturnZeroOnOneAndOne()
        {
            var value1 = new AtomicLong(1);
            var value2 = new AtomicLong(1);

            Assert.AreEqual(0, value1.CompareTo(value2));
        }

        [Test]
        public void CompareToShouldReturnLessThanZeroOnOneAndTwo()
        {
            var value1 = new AtomicLong(1);
            var value2 = new AtomicLong(2);

            Assert.AreEqual(-1, value1.CompareTo(value2));
        }

        [Test]
        public void ReadUnfencedSouldReturnOne()
        {
            var value = new AtomicLong(1);
            Assert.AreEqual(1, value.ReadUnfenced());
        }

        [Test]
        public void ReadAcquireFenceSouldReturnOne()
        {
            var value = new AtomicLong(1);
            Assert.AreEqual(1, value.ReadAcquireFence());
        }

        [Test]
        public void ReadFullFenceSouldReturnOne()
        {
            var value = new AtomicLong(1);
            Assert.AreEqual(1, value.ReadFullFence());
        }

        [Test]
        public void ReadCompilerOnlyFenceSouldReturnOne()
        {
            var value = new AtomicLong(1);
            Assert.AreEqual(1, value.ReadCompilerOnlyFence());
        }

        [Test]
        public void WriteReleaseFenceSouldChangeOneToZero()
        {
            var value = new AtomicLong(1);
            value.WriteReleaseFence(0);

            Assert.AreEqual(0, value.ReadAcquireFence());
        }

        [Test]
        public void WriteFullFenceSouldChangeOneToZero()
        {
            var value = new AtomicLong(1);
            value.WriteFullFence(0);

            Assert.AreEqual(0, value.ReadFullFence());
        }

        [Test]
        public void WriteCompilerOnlyFenceSouldChangeOneToZero()
        {
            var value = new AtomicLong(1);
            value.WriteCompilerOnlyFence(0);

            Assert.AreEqual(0, value.ReadCompilerOnlyFence());
        }

        [Test]
        public void WriteUnfencedSouldChangeOneToZero()
        {
            var value = new AtomicLong(1);
            value.WriteUnfenced(0);

            Assert.AreEqual(0, value.ReadUnfenced());
        }

        [Test]
        public void CompareExchangeSouldChangeOneToZero()
        {
            var value = new AtomicLong(1);
            var originalValue = value.CompareExchange(0, 1);

            Assert.AreEqual(0, value.ReadFullFence());
            Assert.AreEqual(1, originalValue);
        }

        [Test]
        public void CompareExchangeSouldNotChangeOneToZeroOnNotMatchComparand()
        {
            var value = new AtomicLong(1);
            var originalValue = value.CompareExchange(0, 0);

            Assert.AreEqual(1, value.ReadFullFence());
            Assert.AreEqual(1, originalValue);
        }

        [Test]
        public void ExchangeSouldChangeOneToZero()
        {
            var value = new AtomicLong(1);
            var originalValue = value.Exchange(0);

            Assert.AreEqual(0, value.ReadFullFence());
            Assert.AreEqual(1, originalValue);
        }

        [Test]
        public void AddAndGetSouldAddTwo()
        {
            var value = new AtomicLong(2);
            var result = value.AddAndGet(2);

            Assert.AreEqual(4, value.ReadFullFence());
            Assert.AreEqual(4, result);
        }

        [Test]
        public void IncrementAndGetSouldIncrement()
        {
            var value = new AtomicLong(1);
            var result = value.IncrementAndGet();

            Assert.AreEqual(2, value.ReadFullFence());
            Assert.AreEqual(2, result);
        }

        [Test]
        public void DecrementAndGetSouldIncrement()
        {
            var value = new AtomicLong(2);
            var result = value.DecrementAndGet();

            Assert.AreEqual(1, value.ReadFullFence());
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ModuleAndGetSouldReturnTwoOn12Mod5()
        {
            var value = new AtomicLong(12);
            long originalValue;
            var result = value.ModuleAndGet(5, out originalValue);

            Assert.AreEqual(2, value.ReadFullFence());
            Assert.AreEqual(2, result);
            Assert.AreEqual(12, originalValue);
        }

        [Test]
        public void MultiplyAndGetSouldReturnSixOn2Times3()
        {
            var value = new AtomicLong(2);
            long originalValue;
            var result = value.MultiplyAndGet(3, out originalValue);

            Assert.AreEqual(6, value.ReadFullFence());
            Assert.AreEqual(6, result);
            Assert.AreEqual(2, originalValue);
        }

        [Test]
        public void ToStringSouldReturnOne()
        {
            var value = new AtomicLong(1);

            Assert.AreEqual("1", value.ToString());
        }
    }
}
