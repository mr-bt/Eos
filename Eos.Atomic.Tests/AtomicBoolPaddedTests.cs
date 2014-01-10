using System;

using NUnit.Framework;

namespace Eos.Atomic.Tests
{
    [TestFixture]
    public class AtomicBoolPaddedPaddedTests
    {
        [Test]
        public void CtorDefaultShouldCreateAsFalse()
        {
            var value = new AtomicBoolPadded();
            Assert.IsFalse(value.ReadUnfenced());
        }

       [Test]
       public void CtorShouldCreateAsTrue()
       {
           var value = new AtomicBoolPadded(true);
           Assert.IsTrue(value.ReadUnfenced());
       }

       [Test]
       public void CtorShouldCreateAsFalse()
       {
           var value = new AtomicBoolPadded(false);
           Assert.IsFalse(value.ReadUnfenced());
       }

        [Test]
        public void GetHashCodeShouldBeOneOnTrue()
        {
            var value = new AtomicBoolPadded(true);
            var result = value.GetHashCode();

            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetHashCodeShouldBeZeroOnFalse()
        {
            var value = new AtomicBoolPadded();
            var result = value.GetHashCode();

            Assert.AreEqual(0, result);
        }

        [Test]
        public void EqualsAtomicBoolPaddedInputShouldReturnTrueOnSameValue()
        {
            var value1 = new AtomicBoolPadded();
            var value2 = new AtomicBoolPadded();

            Assert.IsTrue(value1.Equals(value2));
        }

        [Test]
        public void EqualsAtomicBoolPaddedInputShouldReturnFalseOnDifferentValue()
        {
            var value1 = new AtomicBoolPadded();
            var value2 = new AtomicBoolPadded(true);

            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void EqualsObjectInputShouldReturnTrueOnSameValue()
        {
            var value1 = (object)new AtomicBoolPadded();
            var value2 = (object)new AtomicBoolPadded();

            Assert.IsTrue(value1.Equals(value2));
        }

        [Test]
        public void EqualsObjectInputShouldReturnFalseOnDifferentValue()
        {
            var value1 = (object)new AtomicBoolPadded();
            var value2 = (object)new AtomicBoolPadded(true);

            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void EqualsObjectInputShouldReturnFalseOnNullComparand()
        {
            var value1 = (object)new AtomicBoolPadded();
            object value2 = null;

            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void EqualsObjectInputShouldReturnFalseOnDifferentType()
        {
            var value1 = (object)new AtomicBoolPadded();
            var value2 = (object)DateTime.UtcNow;

            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void EqualsOperatorShouldReturnTrueOnSameValue()
        {
            var value1 = new AtomicBoolPadded();
            var value2 = new AtomicBoolPadded();

            Assert.IsTrue(value1 == value2);
        }

        [Test]
        public void EqualsOperatorShouldReturnFalseOnDifferentValues()
        {
            var value1 = new AtomicBoolPadded();
            var value2 = new AtomicBoolPadded(true);

            Assert.IsFalse(value1 == value2);
        }

        [Test]
        public void NotEqualsOperatorShouldReturnFalseOnSameValue()
        {
            var value1 = new AtomicBoolPadded();
            var value2 = new AtomicBoolPadded();

            Assert.IsFalse(value1 != value2);
        }

        [Test]
        public void NotEqualsOperatorShouldReturnTrueOnDifferentValues()
        {
            var value1 = new AtomicBoolPadded();
            var value2 = new AtomicBoolPadded(true);

            Assert.IsTrue(value1 != value2);
        }

        [Test]
        public void CompareToShouldReturnGreaterThanZeroOnTrueFalse()
        {
            var value1 = new AtomicBoolPadded(true);
            var value2 = new AtomicBoolPadded(false);

            Assert.AreEqual(1, value1.CompareTo(value2));
        }

        [Test]
        public void CompareToShouldReturnZeroOnTrueTrue()
        {
            var value1 = new AtomicBoolPadded(true);
            var value2 = new AtomicBoolPadded(true);

            Assert.AreEqual(0, value1.CompareTo(value2));
        }

        [Test]
        public void CompareToShouldReturnLessThanZeroOnFalseTrue()
        {
            var value1 = new AtomicBoolPadded();
            var value2 = new AtomicBoolPadded(true);

            Assert.AreEqual(-1, value1.CompareTo(value2));
        }

        [Test]
        public void CompareToShouldReturnZeroOnFalseFalse()
        {
            var value1 = new AtomicBoolPadded();
            var value2 = new AtomicBoolPadded();

            Assert.AreEqual(0, value1.CompareTo(value2));
        }

        [Test]
        public void ReadUnfencedSouldReturnTrueOnValueTrue()
        {
            var value = new AtomicBoolPadded(true);
            Assert.IsTrue(value.ReadUnfenced());
        }

        [Test]
        public void ReadUnfencedSouldReturnFalseOnValueFalse()
        {
            var value = new AtomicBoolPadded(false);
            Assert.IsFalse(value.ReadUnfenced());
        }

        [Test]
        public void ReadAcquireFenceSouldReturnTrueOnValueTrue()
        {
            var value = new AtomicBoolPadded(true);
            Assert.IsTrue(value.ReadAcquireFence());
        }

        [Test]
        public void ReadAcquireFenceSouldReturnFalseOnValueFalse()
        {
            var value = new AtomicBoolPadded(false);
            Assert.IsFalse(value.ReadAcquireFence());
        }

        [Test]
        public void ReadFullFenceSouldReturnTrueOnValueTrue()
        {
            var value = new AtomicBoolPadded(true);
            Assert.IsTrue(value.ReadFullFence());
        }

        [Test]
        public void ReadFullFenceSouldReturnFalseOnValueFalse()
        {
            var value = new AtomicBoolPadded(false);
            Assert.IsFalse(value.ReadFullFence());
        }

        [Test]
        public void ReadCompilerOnlyFenceSouldReturnTrueOnValueTrue()
        {
            var value = new AtomicBoolPadded(true);
            Assert.IsTrue(value.ReadCompilerOnlyFence());
        }

        [Test]
        public void ReadCompilerOnlyFenceSouldReturnFalseOnValueFalse()
        {
            var value = new AtomicBoolPadded(false);
            Assert.IsFalse(value.ReadCompilerOnlyFence());
        }

        [Test]
        public void WriteReleaseFenceSouldChangeFalseToTrue()
        {
            var value = new AtomicBoolPadded(false);
            value.WriteReleaseFence(true);

            Assert.IsTrue(value.ReadAcquireFence());
        }

        [Test]
        public void WriteReleaseFenceSouldChangeTrueToFalse()
        {
            var value = new AtomicBoolPadded(true);
            value.WriteReleaseFence(false);

            Assert.IsFalse(value.ReadAcquireFence());
        }

        [Test]
        public void WriteFullFenceSouldChangeFalseToTrue()
        {
            var value = new AtomicBoolPadded(false);
            value.WriteFullFence(true);

            Assert.IsTrue(value.ReadFullFence());
        }

        [Test]
        public void WriteFullFenceSouldChangeTrueToFalse()
        {
            var value = new AtomicBoolPadded(true);
            value.WriteFullFence(false);

            Assert.IsFalse(value.ReadFullFence());
        }

        [Test]
        public void WriteCompilerOnlyFenceSouldChangeFalseToTrue()
        {
            var value = new AtomicBoolPadded(false);
            value.WriteCompilerOnlyFence(true);

            Assert.IsTrue(value.ReadCompilerOnlyFence());
        }

        [Test]
        public void WriteCompilerOnlyFenceSouldChangeTrueToFalse()
        {
            var value = new AtomicBoolPadded(true);
            value.WriteCompilerOnlyFence(false);

            Assert.IsFalse(value.ReadCompilerOnlyFence());
        }

        [Test]
        public void WriteUnfencedSouldChangeFalseToTrue()
        {
            var value = new AtomicBoolPadded(false);
            value.WriteUnfenced(true);

            Assert.IsTrue(value.ReadUnfenced());
        }

        [Test]
        public void WriteUnfencedSouldChangeTrueToFalse()
        {
            var value = new AtomicBoolPadded(true);
            value.WriteUnfenced(false);

            Assert.IsFalse(value.ReadUnfenced());
        }

        [Test]
        public void CompareExchangeSouldChangeFalseToTrue()
        {
            var value = new AtomicBoolPadded(false);
            var originalValue = value.CompareExchange(true, false);

            Assert.IsTrue(value.ReadFullFence());
            Assert.IsFalse(originalValue);
        }

        [Test]
        public void CompareExchangeSouldChangeTrueToFalse()
        {
            var value = new AtomicBoolPadded(true);
            var originalValue = value.CompareExchange(false, true);

            Assert.IsFalse(value.ReadFullFence());
            Assert.IsTrue(originalValue);
        }

        [Test]
        public void CompareExchangeSouldNotChangeFalseToTrueOnNotMatchComparand()
        {
            var value = new AtomicBoolPadded(false);
            var originalValue = value.CompareExchange(true, true);

            Assert.IsFalse(value.ReadFullFence());
            Assert.IsFalse(originalValue);
        }

        [Test]
        public void CompareExchangeSouldNotChangeTrueToFalseOnNotMatchComparand()
        {
            var value = new AtomicBoolPadded(true);
            var originalValue = value.CompareExchange(false, false);

            Assert.IsTrue(value.ReadFullFence());
            Assert.IsTrue(originalValue);
        }

        [Test]
        public void ExchangeSouldChangeFalseToTrue()
        {
            var value = new AtomicBoolPadded(false);
            var originalValue = value.Exchange(true);

            Assert.IsTrue(value.ReadFullFence());
            Assert.IsFalse(originalValue);
        }

        [Test]
        public void ExchangeSouldChangeTrueToFalse()
        {
            var value = new AtomicBoolPadded(true);
            var originalValue = value.Exchange(false);

            Assert.IsFalse(value.ReadFullFence());
            Assert.IsTrue(originalValue);
        }

        [Test]
        public void ToStringSouldReturnTrue()
        {
            var value = new AtomicBoolPadded(true);

            Assert.AreEqual("True", value.ToString());
        }

        [Test]
        public void ToStringSouldReturnFalse()
        {
            var value = new AtomicBoolPadded();

            Assert.AreEqual("False", value.ToString());
        }
    }
}
