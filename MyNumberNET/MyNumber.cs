using System;
using System.Collections.Generic;

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
            var sum = 0;
            for (var n = 1; n < 7; n++)
                sum += (n + 1) * number[11 - n];
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
}