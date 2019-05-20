using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class HashExtensions
    {
        #region Private Fields

        private const int HashLength = 8;

        #endregion Private Fields

        #region Public Methods

        public static int GetSequenceHash
            (params object[] sequence)
        {
            return sequence?.ToArray().GetSequenceHash() ?? 0;
        }

        public static int GetSequenceHash<T>
            (this IEnumerable<T> sequence)
        {
            const int seed = 487;
            const int modifier = 31;

            unchecked
            {
                var result = sequence?.Aggregate(
                    seed: seed,
                    func: (current, item) => (current * modifier) + (item?.GetHashCode() ?? 0)) ?? 0;
                return result;
            }
        }

        public static int GetSequenceHash<T, TProperty>
            (this IEnumerable<T> sequence, Func<T, TProperty> property)
            where TProperty : class
        {
            return sequence?.ToArray().Select(property)?.GetSequenceHash() ?? 0;
        }

        public static int GetSequenceHashDirected<T, TProperty>
            (this T[] sequence, Func<T, TProperty> property)
            where TProperty : class
        {
            return sequence?.ToArray()
                .GetSequenceHashDirected(property) ?? 0;
        }

        public static int GetSequenceHashDirected<T, TProperty>
            (this IEnumerable<T> sequence, Func<T, TProperty> property)
            where TProperty : class
        {
            var hash = sequence.GetSequenceHash(property);

            var reverseHash = sequence?
                .Reverse()
                .GetSequenceHash(property) ?? 0;

            var result = hash < reverseHash ? hash : reverseHash;

            return result;
        }

        public static int GetSequenceHashOrdered<T>
            (params T[] sequence)
        {
            return sequence?.ToArray()
                .GetSequenceHashOrdered() ?? 0;
        }

        public static int GetSequenceHashOrdered<T>
            (this IEnumerable<T> sequence)
        {
            return sequence?
                .OrderBy(s => s)?
                .GetSequenceHash() ?? 0;
        }

        public static int GetStaticHash
            (this IEnumerable<object> value, int? length = HashLength)
        {
            var result = 0;

            if (value != null)
            {
                foreach (var v in value)
                {
                    result += v.GetStaticHash(length ?? HashLength);
                }
            }

            return result.GetStaticHash(length ?? HashLength);
        }

        public static int GetStaticHash
            (this object value, int? length = HashLength)
        {
            var len = length ?? HashLength;
            var result = value.GetStaticHash(len).ToString("D" + len);

            return Convert.ToInt32(result);
        }

        #endregion Public Methods

        #region Private Methods

        private static int GetStaticHash
            (this object value, int length)
        {
            var result = 0;
            var text = value?.ToString();

            if (!string.IsNullOrWhiteSpace(text) && length > 0)
            {
                uint hash = 0;

                // if you care this can be done much faster with unsafe
                // using fixed char* reinterpreted as a byte*
                foreach (byte b in System.Text.Encoding.Unicode.GetBytes(text))
                {
                    hash += b;
                    hash += (hash << 10);
                    hash ^= (hash >> 6);
                }

                // final avalanche
                hash += (hash << 3);
                hash ^= (hash >> 11);
                hash += (hash << 15);

                // Ensure that no values with a leading zero can be created
                var reducedValue = Math.Pow(10, length - 1);
                var maximumValue = Math.Pow(10, length) - reducedValue;

                // helpfully we only want positive integer < MUST_BE_LESS_THAN
                // so simple truncate cast is ok if not perfect
                result = (int)((hash % maximumValue) + reducedValue);
            }

            return result;
        }

        #endregion Private Methods
    }
}