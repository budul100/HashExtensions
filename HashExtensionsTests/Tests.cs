using NUnit.Framework;

namespace HashExtensionsTests
{
    public class Tests
    {
        #region Public Methods

        [Test]
        public void TestUniquness()
        {
            var hash1 = HashExtensions.Extensions.GetSequenceHash(
                -1841707084,
                881919246,
                1060284364);

            var hash2 = HashExtensions.Extensions.GetSequenceHash(
                -1375185562,
                1060284364);

            Assert.AreNotEqual(
                hash1,
                hash2);
        }

        #endregion Public Methods
    }
}