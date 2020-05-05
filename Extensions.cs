using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HashExtensions
{
    public static class Extensions
    {
        #region Private Fields

        private const string AllCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private const string Digits = "0123456789";

        // Defined by ulong.MaxValue
        private const int HashLength = 19;

        private const string Separator = "\n";

        #endregion Private Fields

        #region Public Methods

        public static int GetSequenceHash(params object[] items)
        {
            return items?.GetSequenceHash() ?? 0;
        }

        public static int GetSequenceHash<T>(params Func<T, object>[] properties)
        {
            return properties?.GetSequenceHash() ?? 0;
        }

        public static int GetSequenceHash<T>(this IEnumerable<T> items)
        {
            var result = default(int);

            if (items?.Any() ?? false)
            {
                var hash = new HashCode();

                foreach (var item in items)
                {
                    hash.Add(item);
                }

                result = hash.ToHashCode();
            }

            return result;
        }

        public static int GetSequenceHash<T, TProperty>(this IEnumerable<T> items, Func<T, TProperty> property)
        {
            var chain = items?
                .Select(property)?.ToArray();

            return chain.GetSequenceHash();
        }

        public static int GetSequenceHash<T>(this IEnumerable<T> items, params Func<T, object>[] properties)
        {
            return items
                .GetSequenceHashes(properties)
                .GetSequenceHash();
        }

        public static int GetSequenceHashDirected<T>(this IEnumerable<T> items)
        {
            var hash = items.GetSequenceHash();

            var reverseHash = items?
                .Reverse()
                .GetSequenceHash() ?? 0;

            var result = hash < reverseHash
                ? hash
                : reverseHash;

            return result;
        }

        public static int GetSequenceHashDirected<T, TProperty>(this IEnumerable<T> items, Func<T, TProperty> property)
        {
            var hash = items.GetSequenceHash(property);

            var reverseHash = items?
                .Reverse()
                .GetSequenceHash(property) ?? 0;

            var result = hash < reverseHash
                ? hash
                : reverseHash;

            return result;
        }

        public static int GetSequenceHashOrdered<T>(params T[] items)
        {
            return items?.GetSequenceHashOrdered() ?? 0;
        }

        public static int GetSequenceHashOrdered<T>(this IEnumerable<T> items)
        {
            return items?
                .OrderBy(s => s)?
                .GetSequenceHash() ?? 0;
        }

        public static ulong GetStaticHashNumber(params string[] values)
        {
            return values.GetStaticHashNumber();
        }

        public static ulong GetStaticHashNumber(this IEnumerable<string> values, int length = HashLength)
        {
            if (length > HashLength)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLength}.",
                    paramName: nameof(length));

            var result = default(ulong);

            if (values?.Any() ?? false)
            {
                var allValues = string.Join(
                    separator: Separator,
                    values: values);

                result = allValues.GetStaticHashNumber(length);
            }

            return result;
        }

        public static ulong GetStaticHashNumber(this string value, int length = HashLength)
        {
            if (length > HashLength)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLength}.",
                    paramName: nameof(length));

            var hashString = value.GetHashString(
                length: length,
                chars: Digits);

            var result = ulong.Parse(hashString).Limit(length);

            return result;
        }

        public static string GetStaticHashText(params string[] values)
        {
            return values.GetStaticHashText();
        }

        public static string GetStaticHashText(this IEnumerable<string> values, int length = HashLength)
        {
            if (length > HashLength)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLength}.",
                    paramName: nameof(length));

            var result = default(string);

            if (values?.Any() ?? false)
            {
                var allValues = string.Join(
                    separator: Separator,
                    values: values);

                result = allValues.GetStaticHashText(length);
            }

            return result;
        }

        public static string GetStaticHashText(this string value, int length = HashLength)
        {
            if (length > HashLength)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLength}.",
                    paramName: nameof(length));

            var result = value.GetHashString(
                length: length,
                chars: AllCharacters);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetHashString(this string value, int length, string chars)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            var hashString = new SHA256Managed();
            var hash1 = hashString.ComputeHash(bytes);
            var hash2 = new char[length];

            for (var i = 0; i < hash2.Length; i++)
            {
                hash2[i] = chars[hash1[i] % chars.Length];
            }

            var result = new string(hash2);

            return result;
        }

        private static IEnumerable<int> GetSequenceHashes<T, TProperty>(this IEnumerable<T> items, Func<T, TProperty>[] properties)
        {
            foreach (var property in properties)
            {
                yield return items.GetSequenceHash(property);
            }
        }

        private static ulong Limit(this ulong value, int length)
        {
            var reducedValue = Math.Pow(10, length - 1);
            var maximumValue = Math.Pow(10, length) - reducedValue;

            var result = (ulong)((value % maximumValue) + reducedValue);
            return result;
        }

        #endregion Private Methods
    }
}