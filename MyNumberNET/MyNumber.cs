using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MyNumberNET
{
    public class MyNumber
    {
        // Centralized random number pool
        private readonly Random _random = new Random();

        /// <summary>
        ///     Verify "My Number" if it is a valid number
        /// </summary>
        /// <param name="number">Array of "My Number" digits</param>
        /// <exception cref="MyNumberMalformedException">Thrown when the provided number is malformed</exception>
        /// <returns>Whether it is a valid "My Number" sequence</returns>
        public static bool VerifyNumber(int[] number)
        {
            if (number == null)
                throw new MyNumberMalformedException("Input array is null.");
            if (number.Length != 12)
                throw new MyNumberMalformedException("Malformed sequence. Must be 12 digits.");
            if (Array.Exists(number, n => n < 0 || n > 9))
                throw new MyNumberMalformedException("All digits must be between 0 and 9.");
            var checkDigit = CalculateCheckDigits(Truncate(number));
            return number[11] == checkDigit;
        }

        /// <summary>
        ///     Calculate check digits from the first 11 digits of "My Number"
        /// </summary>
        /// <param name="number">Array of first 11 "My Number" digits</param>
        /// <exception cref="MyNumberMalformedException">Thrown when the provided number is malformed</exception>
        /// <returns>Check digit for the "My Number" supplied</returns>
        public static int CalculateCheckDigits(int[] number)
        {
            if (number == null)
                throw new MyNumberMalformedException("Input array is null.");
            if (number.Length != 11)
                throw new MyNumberMalformedException("Malformed sequence. Must be 11 digits.");
            if (Array.Exists(number, n => n < 0 || n > 9))
                throw new MyNumberMalformedException("All digits must be between 0 and 9.");
            
            // Calculate check digit using the official My Number algorithm
            // Process digits from right to left with specific weights
            // Array indexing: number[11-n] safely accesses indices 10,9,8,...,0 for n=1,2,3,...,11
            // This avoids Array.Reverse() while maintaining correct algorithm behavior
            var sum = 0;
            // First loop: rightmost 6 digits (indices 10,9,8,7,6,5) with weights 2,3,4,5,6,7
            for (var n = 1; n < 7; n++)
                sum += (n + 1) * number[11 - n];
            // Second loop: leftmost 5 digits (indices 4,3,2,1,0) with weights 2,3,4,5,6
            for (var n = 7; n < 12; n++)
                sum += (n - 5) * number[11 - n];
            if (sum % 11 <= 1)
                return 0;
            return 11 - sum % 11;
        }

        /// <summary>
        ///     Generate random "My Number" sequence
        /// </summary>
        /// <returns>Random "My Number" sequence</returns>
        public int[] GenerateRandomNumber()
        {
            var result = new int[12];
            for (var i = 0; i < 11; i++)
                result[i] = _random.Next(0, 10);
            result[11] = CalculateCheckDigits(Truncate(result));
            return result;
        }

        /// <summary>
        ///     Truncate "My Number" digit
        /// </summary>
        /// <param name="number">Array of "My Number" digits</param>
        /// <exception cref="MyNumberMalformedException">Thrown when the provided number is malformed</exception>
        /// <returns></returns>
        private static int[] Truncate(IReadOnlyList<int> number)
        {
            if (number == null)
                throw new MyNumberMalformedException("Input array is null.");
            if (number.Count < 11 || number.Count > 12)
                throw new MyNumberMalformedException("Malformed sequence. Must be 11 or 12 digits.");
            var result = new int[11];
            for (var i = 0; i < 11; i++)
                result[i] = number[i];
            return result;
        }

        #region Exceptions

        public class MyNumberException : Exception
        {
            public MyNumberException(string message)
                : base(message)
            {
            }
        }

        public class MyNumberMalformedException : MyNumberException
        {
            public MyNumberMalformedException(string message)
                : base(message)
            {
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a valid My Number that enforces correct format and provides type safety.
    /// This is an immutable value type that ensures the My Number is always in a valid state.
    /// </summary>
    public readonly struct MyNumberValue : IEquatable<MyNumberValue>, IFormattable
    {
        private readonly int[] _digits;

        /// <summary>
        /// Gets the 12-digit array representation of this My Number.
        /// </summary>
        public int[] Digits => (int[])_digits?.Clone() ?? throw new InvalidOperationException("MyNumberValue is not initialized.");

        /// <summary>
        /// Gets whether this MyNumberValue instance has been properly initialized.
        /// </summary>
        public bool IsInitialized => _digits != null;

        /// <summary>
        /// Initializes a new instance of MyNumberValue from a 12-digit array.
        /// </summary>
        /// <param name="digits">A 12-digit array representing a valid My Number.</param>
        /// <exception cref="MyNumber.MyNumberMalformedException">Thrown when the digits don't represent a valid My Number.</exception>
        public MyNumberValue(int[] digits)
        {
            if (!MyNumber.VerifyNumber(digits))
            {
                throw new MyNumber.MyNumberMalformedException("The provided digits do not represent a valid My Number.");
            }
            _digits = (int[])digits.Clone();
        }

        /// <summary>
        /// Initializes a new instance of MyNumberValue from individual digit parameters.
        /// </summary>
        /// <param name="d1">First digit</param>
        /// <param name="d2">Second digit</param>
        /// <param name="d3">Third digit</param>
        /// <param name="d4">Fourth digit</param>
        /// <param name="d5">Fifth digit</param>
        /// <param name="d6">Sixth digit</param>
        /// <param name="d7">Seventh digit</param>
        /// <param name="d8">Eighth digit</param>
        /// <param name="d9">Ninth digit</param>
        /// <param name="d10">Tenth digit</param>
        /// <param name="d11">Eleventh digit</param>
        /// <param name="d12">Twelfth digit (check digit)</param>
        /// <exception cref="MyNumber.MyNumberMalformedException">Thrown when the digits don't represent a valid My Number.</exception>
        public MyNumberValue(int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8, int d9, int d10, int d11, int d12)
            : this(new[] { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12 })
        {
        }

        /// <summary>
        /// Creates a MyNumberValue from the first 11 digits, automatically calculating the check digit.
        /// </summary>
        /// <param name="firstElevenDigits">The first 11 digits of the My Number.</param>
        /// <returns>A valid MyNumberValue with the calculated check digit.</returns>
        /// <exception cref="MyNumber.MyNumberMalformedException">Thrown when the input is invalid.</exception>
        public static MyNumberValue FromFirstElevenDigits(int[] firstElevenDigits)
        {
            if (firstElevenDigits == null || firstElevenDigits.Length != 11)
            {
                throw new MyNumber.MyNumberMalformedException("Must provide exactly 11 digits.");
            }

            var checkDigit = MyNumber.CalculateCheckDigits(firstElevenDigits);
            var allDigits = new int[12];
            Array.Copy(firstElevenDigits, allDigits, 11);
            allDigits[11] = checkDigit;

            return new MyNumberValue(allDigits);
        }

        /// <summary>
        /// Attempts to parse a string representation of a My Number.
        /// </summary>
        /// <param name="value">String containing 12 digits (may include separators like spaces or hyphens).</param>
        /// <param name="result">The parsed MyNumberValue if successful.</param>
        /// <returns>True if parsing was successful, false otherwise.</returns>
        public static bool TryParse(string value, out MyNumberValue result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            // Remove common separators
            var cleanValue = value.Replace(" ", "").Replace("-", "").Replace("_", "");

            if (cleanValue.Length != 12)
                return false;

            var digits = new int[12];
            for (int i = 0; i < 12; i++)
            {
                if (!char.IsDigit(cleanValue[i]))
                    return false;
                digits[i] = cleanValue[i] - '0';
            }

            try
            {
                result = new MyNumberValue(digits);
                return true;
            }
            catch (MyNumber.MyNumberMalformedException)
            {
                return false;
            }
        }

        /// <summary>
        /// Parses a string representation of a My Number.
        /// </summary>
        /// <param name="value">String containing 12 digits (may include separators like spaces or hyphens).</param>
        /// <returns>A valid MyNumberValue.</returns>
        /// <exception cref="ArgumentException">Thrown when the string cannot be parsed as a valid My Number.</exception>
        public static MyNumberValue Parse(string value)
        {
            if (TryParse(value, out var result))
                return result;

            throw new ArgumentException($"Unable to parse '{value}' as a valid My Number.", nameof(value));
        }

        /// <summary>
        /// Generates a random valid My Number.
        /// </summary>
        /// <returns>A randomly generated valid MyNumberValue.</returns>
        public static MyNumberValue GenerateRandom()
        {
            var generator = new MyNumber();
            var digits = generator.GenerateRandomNumber();
            return new MyNumberValue(digits);
        }

        /// <summary>
        /// Returns the string representation of this My Number.
        /// </summary>
        /// <returns>A 12-digit string representation.</returns>
        public override string ToString()
        {
            return ToString("N", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the string representation of this My Number with the specified format.
        /// </summary>
        /// <param name="format">
        /// Format string: 
        /// "N" or null = no separators (default): "123456789012"
        /// "S" = with spaces: "1234 5678 9012"
        /// "H" = with hyphens: "1234-5678-9012"
        /// "G" = grouped format: "1234-5678-901-2"
        /// </param>
        /// <returns>Formatted string representation.</returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the string representation of this My Number with the specified format and format provider.
        /// </summary>
        /// <param name="format">Format string (see ToString(string) for options).</param>
        /// <param name="formatProvider">Format provider (currently not used).</param>
        /// <returns>Formatted string representation.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("MyNumberValue is not initialized.");

            var digitString = string.Join("", _digits);

            return format?.ToUpperInvariant() switch
            {
                null or "N" => digitString,
                "S" => $"{digitString.Substring(0, 4)} {digitString.Substring(4, 4)} {digitString.Substring(8, 4)}",
                "H" => $"{digitString.Substring(0, 4)}-{digitString.Substring(4, 4)}-{digitString.Substring(8, 4)}",
                "G" => $"{digitString.Substring(0, 4)}-{digitString.Substring(4, 4)}-{digitString.Substring(8, 3)}-{digitString.Substring(11, 1)}",
                _ => throw new FormatException($"Format string '{format}' is not supported.")
            };
        }

        /// <summary>
        /// Determines whether this instance is equal to another MyNumberValue.
        /// </summary>
        /// <param name="other">The other MyNumberValue to compare.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals(MyNumberValue other)
        {
            if (!IsInitialized && !other.IsInitialized)
                return true;
            if (!IsInitialized || !other.IsInitialized)
                return false;

            return _digits.SequenceEqual(other._digits);
        }

        /// <summary>
        /// Determines whether this instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is MyNumberValue other && Equals(other);
        }

        /// <summary>
        /// Gets the hash code for this MyNumberValue.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode()
        {
            if (!IsInitialized)
                return 0;

            unchecked
            {
                int hash = 17;
                foreach (var digit in _digits)
                {
                    hash = hash * 31 + digit;
                }
                return hash;
            }
        }

        /// <summary>
        /// Determines whether two MyNumberValue instances are equal.
        /// </summary>
        /// <param name="left">The first MyNumberValue.</param>
        /// <param name="right">The second MyNumberValue.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator ==(MyNumberValue left, MyNumberValue right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two MyNumberValue instances are not equal.
        /// </summary>
        /// <param name="left">The first MyNumberValue.</param>
        /// <param name="right">The second MyNumberValue.</param>
        /// <returns>True if not equal, false otherwise.</returns>
        public static bool operator !=(MyNumberValue left, MyNumberValue right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implicitly converts a MyNumberValue to an int array.
        /// </summary>
        /// <param name="myNumber">The MyNumberValue to convert.</param>
        /// <returns>A 12-digit int array.</returns>
        public static implicit operator int[](MyNumberValue myNumber)
        {
            return myNumber.Digits;
        }

        /// <summary>
        /// Implicitly converts a MyNumberValue to a string.
        /// </summary>
        /// <param name="myNumber">The MyNumberValue to convert.</param>
        /// <returns>A 12-digit string representation.</returns>
        public static implicit operator string(MyNumberValue myNumber)
        {
            return myNumber.ToString();
        }

        /// <summary>
        /// Explicitly converts an int array to a MyNumberValue.
        /// </summary>
        /// <param name="digits">The 12-digit array to convert.</param>
        /// <returns>A MyNumberValue instance.</returns>
        /// <exception cref="MyNumber.MyNumberMalformedException">Thrown when the array is not a valid My Number.</exception>
        public static explicit operator MyNumberValue(int[] digits)
        {
            return new MyNumberValue(digits);
        }

        /// <summary>
        /// Explicitly converts a string to a MyNumberValue.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>A MyNumberValue instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the string is not a valid My Number.</exception>
        public static explicit operator MyNumberValue(string value)
        {
            return Parse(value);
        }
    }
}