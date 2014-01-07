using System;

using NUnit.Framework;

namespace Eos.Utils.Tests
{
    [TestFixture]
    public class MathExtensionsTests
    {
        [Test]
        public void NextPowerOfTwoIntShouldReturnSamePowerOfTwo()
        {
            const int PowerOfTwoNumber = 256;

            var result = PowerOfTwoNumber.NextPowerOfTwo();

            Assert.AreEqual(PowerOfTwoNumber, result);
        }

        [Test]
        public void NextPowerOfTwoIntShouldReturnCeilingPowerOfTwo()
        {
            const int ExpectedPowerOfTwo = 128;

            var result = 70.NextPowerOfTwo();

            Assert.AreEqual(ExpectedPowerOfTwo, result);
        }

        [Test]
        public void NextPowerOfTwoIntShouldReturnZeroOnNegativeNumber()
        {
            const int ExpectedResult = 0;

            var result = (-1).NextPowerOfTwo();

            Assert.AreEqual(ExpectedResult, result);
        }

        [Test]
        public void NextPowerOfTwoIntShouldReturnZeroOnZero()
        {
            const int ExpectedResult = 0;

            var result = 0.NextPowerOfTwo();

            Assert.AreEqual(ExpectedResult, result);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NextPowerOfTwoIntShouldThrowException()
        {
            const int MaxIntPowerOfTwo = 1073741824;

            (MaxIntPowerOfTwo + 1).NextPowerOfTwo();
        }

        [Test]
        public void NextPowerOfTwoIntShouldMatchRules()
        {
            for (var i = 1; i <= 257; i++)
            {
                var nextPowerOfTwo = i.NextPowerOfTwo();
                Assert.IsTrue(nextPowerOfTwo >= i, "Next power of 2 [{0}] has to be greated than or equal to the value [{1}]", nextPowerOfTwo, i);
                Assert.IsTrue(nextPowerOfTwo.IsPowerOfTwo(), "{0} is not power of two", nextPowerOfTwo);
            }
        }

        [Test]
        public void NextPowerOfTwoLongShouldReturnSamePowerOfTwo()
        {
            const long PowerOfTwoNumber = 256;

            var result = PowerOfTwoNumber.NextPowerOfTwo();

            Assert.AreEqual(PowerOfTwoNumber, result);
        }

        [Test]
        public void NextPowerOfTwoLongShouldReturnCeilingPowerOfTwo()
        {
            const long ExpectedPowerOfTwo = 128;

            var result = 70L.NextPowerOfTwo();

            Assert.AreEqual(ExpectedPowerOfTwo, result);
        }

        [Test]
        public void NextPowerOfTwoLongShouldReturnZeroOnNegativeNumber()
        {
            const long ExpectedResult = 0;

            var result = (-1L).NextPowerOfTwo();

            Assert.AreEqual(ExpectedResult, result);
        }

        [Test]
        public void NextPowerOfTwoLongShouldReturnZeroOnZero()
        {
            const long ExpectedResult = 0;

            var result = 0L.NextPowerOfTwo();

            Assert.AreEqual(ExpectedResult, result);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NextPowerOfTwoLongShouldThrowException()
        {
            const long MaxLongPowerOfTwo = 4611686018427387904;

            (MaxLongPowerOfTwo + 1).NextPowerOfTwo();
        }

        [Test]
        public void NextPowerOfTwoLongShouldMatchRules()
        {
            for (var i = 1000L; i <= 1257; i++)
            {
                var nextPowerOfTwo = i.NextPowerOfTwo();
                Assert.IsTrue(nextPowerOfTwo >= i, "Next power of 2 [{0}] has to be greated than or equal to the value [{1}]", nextPowerOfTwo, i);
                Assert.IsTrue(nextPowerOfTwo.IsPowerOfTwo(), "{0} is not power of two", nextPowerOfTwo);
            }
        }

        [Test]
        public void IsPowerIntOfTwoIntShouldBeTrue()
        {
            var powerOfTwoSample = new[] { 1, 2, 4, 8, 16, 32, (int)Math.Pow(2, 10) };

            foreach (var value in powerOfTwoSample)
            {
                Assert.IsTrue(value.IsPowerOfTwo(), "{0} failed to be power of two", value);
            }
        }

        [Test]
        public void IsPowerIntOfTwoIntShouldBeFalse()
        {
            var notPowerOfTwoSample = new[] { 0L, 3L, 5L, 6L, 7L, 9L, (long)Math.Pow(3, 5) };

            foreach (var value in notPowerOfTwoSample)
            {
                Assert.IsFalse(value.IsPowerOfTwo(), "{0} failed NOT to be power of two", value);
            }
        }

        [Test]
        public void HaveOppositeSignsIntShouldReturntrueOnDiferrentInts()
        {
            Assert.IsTrue(1.HaveOppositeSigns(-2));
        }

        [Test]
        public void HaveOppositeSignsIntShouldSucceedOnSameInts()
        {
            Assert.IsTrue(1.HaveOppositeSigns(-1));
        }

        [Test]
        public void HaveOppositeSignsIntShouldFailOnSameSignAndSameInts()
        {
            Assert.IsFalse(1.HaveOppositeSigns(1));
        }

        [Test]
        public void HaveOppositeSignsIntShouldFailOnSameSignAndDiferentInts()
        {
            Assert.IsFalse(1.HaveOppositeSigns(2));
        }

        [Test]
        public void HaveOppositeSignsLongShouldReturntrueOnDiferrentInts()
        {
            Assert.IsTrue(1L.HaveOppositeSigns(-2L));
        }

        [Test]
        public void HaveOppositeSignsLongShouldSucceedOnSameInts()
        {
            Assert.IsTrue(1L.HaveOppositeSigns(-1L));
        }

        [Test]
        public void HaveOppositeSignsLongShouldFailOnSameSignAndSameInts()
        {
            Assert.IsFalse(1L.HaveOppositeSigns(1L));
        }

        [Test]
        public void HaveOppositeSignsLongShouldFailOnSameSignAndDiferentInts()
        {
            Assert.IsFalse(1L.HaveOppositeSigns(2L));
        }

        [Test]
        public void IsEvenIntShouldBeTrue()
        {
            var result = 2.IsEven();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsEvenIntShouldBeFalse()
        {
            var result = 3.IsEven();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsEvenLongShouldBeTrue()
        {
            var result = 2L.IsEven();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsEvenLongShouldBeFalse()
        {
            var result = 3L.IsEven();

            Assert.IsFalse(result);
        }
    }
}
