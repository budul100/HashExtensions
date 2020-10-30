using HashExtensions;
using NUnit.Framework;
using System;
using System.Linq;

namespace HashExtensionsTests
{
    public class Tests
    {
        #region Private Fields

        private const int RandomTextLength = 10000000;

        private static readonly Random random = new Random();

        private readonly string randomText;

        #endregion Private Fields

        #region Public Constructors

        public Tests()
        {
            randomText = RandomString(RandomTextLength);
        }

        #endregion Public Constructors

        #region Public Methods

        [Test]
        public void GetStaticHashLength()
        {
            var length = 8;

            var result1 = randomText.GetStaticHashText(length);

            Assert.True(result1.Length == length);

            var result2 = "1234567890".GetStaticHashText(length);

            Assert.True(result2.Length == length);

            var result3 = "1".GetStaticHashText(length);

            Assert.True(result3.Length == length);
        }

        [Test]
        public void GetStaticHashLengthMax()
        {
            var length = Extensions.HashLengthMax;

            var result1 = randomText.GetStaticHashText(length);

            Assert.True(result1.Length == length);

            var result2 = "1234567890".GetStaticHashText(length);

            Assert.True(result2.Length == length);

            var result3 = "1".GetStaticHashText(length);

            Assert.True(result3.Length == length);
        }

        [Test]
        public void GetStaticHashLengthMin()
        {
            var length = Extensions.HashLengthMin;

            var result1 = randomText.GetStaticHashText(length);

            Assert.True(result1.Length == length);

            var result2 = "1234567890".GetStaticHashText(length);

            Assert.True(result2.Length == length);

            var result3 = "1".GetStaticHashText(length);

            Assert.True(result3.Length == length);
        }

        [Test]
        public void GetStaticHashNumber()
        {
            var result1 = randomText.GetStaticHashNumber(4);
            var result2 = randomText.GetStaticHashNumber();

            Assert.True(result1 != result2);
        }

        [Test]
        public void GetStaticHashNumberNotNull()
        {
            var result1 = "ASTPAR".GetStaticHashNumber(8);

            Assert.True(result1 != 0);

            var result2 = "ASTPAS".GetStaticHashNumber(8);

            Assert.True(result1 != result2);
        }

        [Test]
        public void GetStaticHashText()
        {
            var result1 = randomText.GetStaticHashText(4);
            var result2 = randomText.GetStaticHashText();

            Assert.True(result1 != result2);
        }

        [Test]
        public void TestHashDirected()
        {
            var hash1 = Extensions.GetSequenceHashDirected(
                "a",
                "b",
                "c");

            var hash2 = Extensions.GetSequenceHashDirected(
                "c",
                "b",
                "a");

            var hash3 = Extensions.GetSequenceHashDirected(
                "a",
                "c",
                "b");

            Assert.AreEqual(
                hash1,
                hash2);

            Assert.AreNotEqual(
                hash1,
                hash3);
        }

        [Test]
        public void TestHashOrdered()
        {
            var hash1 = Extensions.GetSequenceHashOrdered(
                "a",
                "b",
                "c");

            var hash2 = Extensions.GetSequenceHashOrdered(
                "c",
                "b",
                "a");

            var hash3 = Extensions.GetSequenceHashOrdered(
                "a",
                "c",
                "b");

            Assert.AreEqual(
                hash1,
                hash2);

            Assert.AreEqual(
                hash1,
                hash3);
        }

        [Test]
        public void TestStaticHashNumberForEmpty()
        {
            string.Empty.GetStaticHashNumber();

            var hash = default(string).GetStaticHashNumber();
            Assert.IsTrue(hash == 0);

            var array = new string[] { string.Empty, default };
            array.GetStaticHashNumber();
        }

        [Test]
        public void TestUniqueness()
        {
            var hash1 = Extensions.GetSequenceHash(
                -1841707084,
                881919246,
                1060284364);

            var hash2 = Extensions.GetSequenceHash(
                -1375185562,
                1060284364);

            Assert.AreNotEqual(
                hash1,
                hash2);
        }

        #endregion Public Methods

        #region Private Methods

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var result = Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray();

            return new string(result);
        }

        #endregion Private Methods
    }
}