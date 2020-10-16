using System;
using System.Collections.Generic;
using System.Data.HashFunction.SpookyHash;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HashExtensions
{
    public static class Extensions
    {
        #region Private Fields

        private const string AllCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const int HashLengthMax = 19;   // Defined by ulong.MaxValue
        private const string Separator = "\n";

        private static readonly UTF8Encoding encoding = new UTF8Encoding();
        private static readonly ISpookyHashV2 hashCreator = SpookyHashV2Factory.Instance.Create();

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

                hash.Add(items.Count());

                result = hash.ToHashCode();
            }

            return result;
        }

        public static int GetSequenceHash<T, TProperty>(this IEnumerable<T> items, Func<T, TProperty> property)
        {
            var chain = items?
                .Select(property)?.ToArray();

            var result = chain.GetSequenceHash();

            return result;
        }

        public static int GetSequenceHash<T>(this IEnumerable<T> items, params Func<T, object>[] properties)
        {
            var result = items
                .GetSequenceHashes(properties)
                .GetSequenceHash();

            return result;
        }

        public static int GetSequenceHashDirected<T>(params T[] items)
        {
            var result = items?.GetSequenceHashDirected() ?? 0;

            return result;
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
            var chain = items?
                .Select(property)?.ToArray();

            var result = chain.GetSequenceHashDirected();

            return result;
        }

        public static int GetSequenceHashOrdered<T>(params T[] items)
        {
            var result = items?.GetSequenceHashOrdered() ?? 0;

            return result;
        }

        public static int GetSequenceHashOrdered<T>(this IEnumerable<T> items)
        {
            var result = items?
                .OrderBy(s => s)?
                .GetSequenceHash() ?? 0;

            return result;
        }

        public static int GetSequenceHashOrdered<T, TProperty>(this IEnumerable<T> items, Func<T, TProperty> property)
        {
            var chain = items?
                .Select(property)?.ToArray();

            var result = chain.GetSequenceHashOrdered();

            return result;
        }

        public static ulong GetStaticHashNumber(params string[] values)
        {
            return values.GetStaticHashNumber();
        }

        public static ulong GetStaticHashNumber(this IEnumerable<string> values, int length = HashLengthMax)
        {
            if (length > HashLengthMax)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLengthMax}.",
                    paramName: nameof(length));

            var result = default(ulong);

            if (values?.Any() ?? false)
            {
                var merged = string.Join(
                    separator: Separator,
                    values: values);

                result = merged.GetStaticHashNumber(length);
            }

            return result;
        }

        public static ulong GetStaticHashNumber(this string value, int length = HashLengthMax)
        {
            if (length > HashLengthMax)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLengthMax}.",
                    paramName: nameof(length));

            var hashString = value.GetHashString(
                length: length,
                availableChars: Digits);

            var result = default(ulong);

            if (!string.IsNullOrEmpty(hashString))
            {
                result = ulong.Parse(
                    s: hashString ?? string.Empty,
                    provider: CultureInfo.InvariantCulture).Limit(length);
            }

            return result;
        }

        public static string GetStaticHashText(params string[] values)
        {
            return values.GetStaticHashText();
        }

        public static string GetStaticHashText(this IEnumerable<string> values, int length = HashLengthMax)
        {
            if (length > HashLengthMax)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLengthMax}.",
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

        public static string GetStaticHashText(this string value, int length = HashLengthMax)
        {
            if (length > HashLengthMax)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLengthMax}.",
                    paramName: nameof(length));

            var result = value.GetHashString(
                length: length,
                availableChars: AllCharacters);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetHashString(this string value, int length, string availableChars)
        {
            var result = default(string);

            if (!string.IsNullOrEmpty(value))
            {
                var bytes = encoding.GetBytes(value);
                var hashArray = hashCreator.ComputeHash(bytes).Hash;
                var hashNumbers = BitConverter.ToInt32(
                    value: hashArray,
                    startIndex: 0)
                    .SplitEvenly(length).ToArray();

                var hashString = new StringBuilder();

                foreach (var hashNumber in hashNumbers)
                {
                    var currentChar = availableChars[hashNumber % availableChars.Length];
                    hashString.Append(currentChar);
                }

                result = hashString.ToString();
            }

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

        private static double Pow10(this double number)
        {
            var result = Math.Pow(
                x: 10,
                y: number);

            return result;
        }

        private static IEnumerable<int> SplitEvenly(this int number, int parts)
        {
            number = Math.Abs(number);

            var digitsCount = Math.Floor(Math.Log10(number) + 1);
            var chunkSize = (digitsCount + parts - 1) / parts;

            for (double start = 0; start < digitsCount; start += chunkSize)
            {
                var current = number % (digitsCount - start).Pow10();

                var end = digitsCount - start - chunkSize;
                var result = (int)(current / end.Pow10());

                yield return result;
            }
        }

        #endregion Private Methods
    }
}