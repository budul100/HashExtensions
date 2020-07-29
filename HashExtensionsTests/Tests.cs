using HashExtensions;
using NUnit.Framework;

namespace HashExtensionsTests
{
    public class Tests
    {
        #region Public Methods

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

            Assert.IsTrue(true);

            var hash = default(string).GetStaticHashNumber();
            Assert.IsTrue(hash == 0);

            var array = new string[] { string.Empty, default };
            array.GetStaticHashNumber();

            Assert.IsTrue(true);
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
    }
}