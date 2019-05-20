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

        private static readonly double hashMaximumValue;
        private static readonly double hashReducedValue;

        #endregion Private Fields

        #region Public Constructors

        static HashExtensions()
        {
            hashReducedValue = Math.Pow(10, HashLength - 1);
            hashMaximumValue = Math.Pow(10, HashLength) - hashReducedValue;
        }

        #endregion Public Constructors

        #region Public Methods

        public static int GetSequenceHash
            (params object[] sequence)
        {
            return sequence?.ToArray()
                .GetSequenceHash() ?? 0;
        }

        public static int GetSequenceHash<T>
            (params Func<T, object>[] properties)
        {
            return properties?.ToArray()
                .GetSequenceHash() ?? 0;
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
                    func: (c, i) => (c * modifier) + (i?.GetHashCode() ?? 0)) ?? 0;
                return result;
            }
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
                foreach (var v in values)
                {
                    result += v.GetStaticHashValue();
                }
            }

            return result.ToString().GetStaticHash();
        }

        public static int GetStaticHash
            (this string value)
        {
            return value.GetStaticHashValue();
        }

        #endregion Public Methods

        #region Private Methods

        private static IEnumerable<int> GetSequenceHashes<T, TProperty>
            (this IEnumerable<T> sequence, Func<T, TProperty>[] properties)
        {
            foreach (var p in properties)
            {
                yield return sequence.GetSequenceHash(p);
            }
        }

        private static int GetStaticHashValue(this string value)
        {
            var result = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                uint hash = 0;

                // if you care this can be done much faster with unsafe
                // using fixed char* reinterpreted as a byte*
                foreach (byte b in System.Text.Encoding.Unicode.GetBytes(value))
                {
                    hash += b;
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