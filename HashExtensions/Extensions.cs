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
        #region Public Fields

        // Defined by length of hash creator output and ByteLength
        public const int HashLengthMax = 12;

        public const int HashLengthMin = 0;

        #endregion Public Fields

        #region Private Fields

        // Defined by int32
        private const int ByteLength = 4;

        private const string Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
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
            if (length <= HashLengthMin)
                throw new ArgumentException(
                    message: $"The hash length must be greater or equal to {HashLengthMin}.",
                    paramName: nameof(length));

            if (length > HashLengthMax)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLengthMax}.",
                    paramName: nameof(length));

            var hashString = value.GetHashString(
                size: length,
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
            if (length < HashLengthMin)
                throw new ArgumentException(
                    message: $"The hash length must be greater or equal to {HashLengthMin}.",
                    paramName: nameof(length));

            if (length > HashLengthMax)
                throw new ArgumentException(
                    message: $"The hash length cannot be greater than {HashLengthMax}.",
                    paramName: nameof(length));

            var result = value.GetHashString(
                size: length,
                availableChars: Characters);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetHashString(this string value, int size, string availableChars)
        {
            var result = default(string);

            if (!string.IsNullOrEmpty(value))
            {
                var bytes = encoding.GetBytes(value);
                var hash = hashCreator
                    .ComputeHash(bytes).Hash;

                var hashString = new StringBuilder();

                var position = 0;
                var index = 0;
                var length = hash.Length - ByteLength;

                while (index <= length && position < size)
                {
                    var hashNumber = Math.Abs(BitConverter.ToInt32(
                        value: hash,
                        startIndex: index));

                    var currentChar = availableChars[hashNumber % availableChars.Length];
                    hashString.Append(currentChar);

                    index += (length - index) / (size - position);
                    position++;
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

        private static int PartLength(int length, int parts)
        {
            return (length + parts - 1) / parts;
        }

        #endregion Private Methods
    }
}