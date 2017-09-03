using System;

namespace MyNumberNET
{
    public class MyNumber
    {
        // Centralized random number pool
        private readonly Random _random = new Random();

        /// <summary>
        /// Verify "My Number" if it is a valid number
        /// </summary>
        /// <param name="number">Array of "My Number" digits</param>
        /// <returns>Whether it is a valid "My Number" sequence</returns>
        public bool VerifyNumber(int[] number)
        {
            if (number.Length != 12)
            {
                throw new MyNumberMalformedException("Malformed sequence. Must be 12 digits.");
            }

            var checkDigit = CalculateCheckDigits(Truncate(number));

            if (number[11] == checkDigit)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calculate check digits from the first 11 digits of "My Number"
        /// </summary>
        /// <param name="number">Array of first 11 "My Number" digits</param>
        /// <returns>Check digit for the "My Number" supplied</returns>
        public int CalculateCheckDigits(int[] number)
        {
            if (number.Length != 11)
            {
                throw new MyNumberMalformedException("Malformed sequence. Must be 11 digits.");
            }

            // They count their digits high to low, so reorder it
            Array.Reverse(number);

            int sum = 0;
            for (int n = 1; n < 7; n++)
            {
                // From digit 1 to 6, sum is n+1
                sum += (n + 1) * number[n - 1];
            }

            for (int n = 7; n < 12; n++)
            {
                // From digit 7 to 11, sum is n-5
                sum += (n - 5) * number[n - 1];
            }
            Array.Reverse(number);
            // Calculate against MOD
            if (sum % 11 <= 1)
                return 0;
            else
                return 11 - sum % 11;
        }

        /// <summary>
        /// Generate random "My Number" sequence
        /// </summary>
        /// <returns>Random "My Number" sequence</returns>
        public int[] GenerateRandomNumber()
        {
            var result = new int[12];
            for (int i = 0; i < 10; i++)
                result[i] = _random.Next(0, 9);

            result[11] = CalculateCheckDigits(Truncate(result));
            return result;
        }

        /// <summary>
        /// Truncate "My Number" digit
        /// </summary>
        /// <param name="number">Array of "My Number" digits</param>
        /// <returns></returns>
        private int[] Truncate(int[] number)
        {
            if (number.Length < 11 || number.Length > 12)
            {
                throw new MyNumberMalformedException("Malformed sequence. Must be 11 or 12 digits.");
            }
            var result = new int[11];
            for (int i = 0; i < 11; i++)
            {
                result[i] = number[i];
            }
            return result;
        }

        public class MyNumberException : Exception
        {
            public MyNumberException()
            {
            }

            public MyNumberException(string message)
                : base(message)
            {
            }

            public MyNumberException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class MyNumberMalformedException : Exception
        {
            public MyNumberMalformedException()
            {
            }

            public MyNumberMalformedException(string message)
                : base(message)
            {
            }

            public MyNumberMalformedException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}
