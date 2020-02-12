using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class HashExtensions
    {
        #region Private Fields

        // Defined by int.MaxValue
        private const int HashLength = 9;

        #endregion Private Fields

        #region Public Methods

        public static int GetSequenceHash
            (params object[] sequence)
        {
            return sequence?.GetSequenceHash() ?? 0;
        }

        public static int GetSequenceHash<T>
            (params Func<T, object>[] properties)
        {
            return properties?.GetSequenceHash() ?? 0;
        }

        public static int GetSequenceHash<T>
            (this IEnumerable<T> sequence)
        {
            var result = 0;

            if (sequence?.Any() ?? false)
            {
                const int seed = 17;
                const int modifier = 23;

                var hash = 17;

                result = sequence.ToArray()
                    .Aggregate(
                        seed: seed,
                        func: (c, i) =>
                        {
                            var h = EqualityComparer<T>.Default.GetHashCode(i);
                            if (h != 0) hash = unchecked(hash * h);
                            return (c * modifier) + (hash);
                        });
            }

            return result;
        }

        public static int GetSequenceHash<T, TProperty>
            (this T[] sequence, Func<T, TProperty> property)
        {
            return sequence?.ToArray()
                .GetSequenceHash(property) ?? 0;
        }

        public static int GetSequenceHash<T, TProperty>
            (this IEnumerable<T> sequence, Func<T, TProperty> property)
        {
            var chain = sequence?
                .Select(property)?
                .ToArray();

            return chain.GetSequenceHash();
        }

        public static int GetSequenceHash<T>
            (this IEnumerable<T> sequence, params Func<T, object>[] properties)
        {
            return sequence
                .GetSequenceHashes(properties)
                .GetSequenceHash();
        }

        public static int GetSequenceHashDirected<T, TProperty>
            (this T[] sequence, Func<T, TProperty> property)
        {
            return sequence?.ToArray()
                .GetSequenceHashDirected(property) ?? 0;
        }

        public static int GetSequenceHashDirected<T>
            (params T[] sequence)
        {
            return sequence?.ToArray()
                .GetSequenceHashDirected() ?? 0;
        }

        public static int GetSequenceHashDirected<T>
            (this IEnumerable<T> sequence)
        {
            var hash = sequence.GetSequenceHash();

            var reverseHash = sequence?
                .Reverse()
                .GetSequenceHash() ?? 0;

            var result = hash < reverseHash ? hash : reverseHash;

            return result;
        }

        public static int GetSequenceHashDirected<T, TProperty>
            (this IEnumerable<T> sequence, Func<T, TProperty> property)
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
            return sequence?.GetSequenceHashOrdered() ?? 0;
        }

        public static int GetSequenceHashOrdered<T>
            (this IEnumerable<T> sequence)
        {
            return sequence?
                .OrderBy(s => s)?
                .GetSequenceHash() ?? 0;
        }

        public static int GetStaticHash
            (params string[] values)
        {
            return values.GetStaticHash();
        }

        public static int GetStaticHash
            (this IEnumerable<string> values)
        {
            var result = 0;

            if (values != null)
            {
                foreach (var value in values)
                {
                    result += value.GetStaticHashValue(HashLength);
                }
            }

            return result.ToString().GetStaticHash();
        }

        public static int GetStaticHash
            (this string value, int length = HashLength)
        {
            if (length > HashLength)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLength}.",
                    paramName: nameof(length));

            return value.GetStaticHashValue(length);
        }

        #endregion Public Methods

        #region Private Methods

        private static IEnumerable<int> GetSequenceHashes<T, TProperty>
            (this IEnumerable<T> sequence, Func<T, TProperty>[] properties)
        {
            foreach (var property in properties)
            {
                yield return sequence.GetSequenceHash(property);
            }
        }

        private static int GetStaticHashValue(this string value, int length)
        {
            var result = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                var hashReducedValue = Math.Pow(10, length - 1);
                var hashMaximumValue = Math.Pow(10, length) - hashReducedValue;

                uint hash = 0;

                // if you care this can be done much faster with unsafe
                // using fixed char* reinterpreted as a byte*
                foreach (var current in System.Text.Encoding.Unicode.GetBytes(value))
                {
                    hash += current;
                    hash += (hash << 10);
                    hash ^= (hash >> 6);
                }

                // final avalanche
                hash += (hash << 3);
                hash ^= (hash >> 11);
                hash += (hash << 15);

                // helpfully we only want positive integer < MUST_BE_LESS_THAN
                // so simple truncate cast is ok if not perfect
                // Ensure that no values with a leading zero can be created
                result = (int)((hash % hashMaximumValue) + hashReducedValue);
            }

            return result;
        }

        #endregion Private Methods
    }
}