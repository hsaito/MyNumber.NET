using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using NLog;
using MyNumberNET;
using static System.Char;

namespace MyNumberNET_CLI
{
    public class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static int Main(string[] args)
        {
            Log.Info($"Application started. Args: [{string.Join(", ", args)}]");
            try
            {
                // NLog automatically loads nlog.config, no manual initialization needed
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal error during logger initialization");
                return -1;
            }

            if (args.Length < 1)
            {
                Log.Warn("Not enough argument.");
                Log.Info("Usage: command [arg]");
                Log.Info("Commands: generate [count]/ check [My Number] / complete [first 11 digits of My Number]");
                Log.Info("rangen [minimum] [maximum] / ranges [minimum [maximum]");
                Log.Info("Range functions are rangen (numerical range), and ranges (sequential range)");
                return -1;
            }

            try
            {
                Log.Info($"Executing command: {args[0]}");
                switch (args[0])
                {
                    case "generate":
                        Log.Debug($"generate args: {string.Join(", ", args)}");
                        if (args.Length == 1)
                        {
                            Log.Info("Generating 1 My Number");
                            Generate(1);
                            return 0;
                        }
                        else
                        {
                            if (!Regex.IsMatch(args[1], @"^\d+$"))
                            {
                                Log.Warn("Invalid argument for generate: not numeric");
                                Log.Info("Input needs to be numeric.");
                                return -1;
                            }
                            Log.Info($"Generating {args[1]} My Numbers");
                            Generate(Convert.ToInt64(args[1]));
                            return 0;
                        }
                    case "check":
                        Log.Debug($"check args: {string.Join(", ", args)}");
                        if (args.Length < 2)
                        {
                            Log.Warn("Not enough argument for check");
                            Log.Info("Supply \"My Number\" to check");
                            return -1;
                        }
                        if (!Regex.IsMatch(args[1], @"^\d+$"))
                        {
                            Log.Warn("Invalid argument for check: not numeric");
                            Log.Info("Input needs to be numeric.");
                            return -1;
                        }
                        Log.Info($"Checking My Number: {args[1]}");
                        bool result = Check(args[1]);
                        Log.Info($"Check result for {args[1]}: {(result ? "OK" : "ERROR")}");
                        Console.WriteLine(result ? "OK" : "ERROR");
                        return result ? 0 : 1;
                    case "complete":
                        Log.Debug($"complete args: {string.Join(", ", args)}");
                        if (args.Length < 2)
                        {
                            Log.Warn("Not enough argument for complete");
                            Log.Info("Supply the first 11 digits of \"My Number\" to complete");
                            return -1;
                        }
                        if (!Regex.IsMatch(args[1], @"^\d+$"))
                        {
                            Log.Warn("Invalid argument for complete: not numeric");
                            Log.Info("Input needs to be numeric.");
                            return -1;
                        }
                        Log.Info($"Completing My Number for: {args[1]}");
                        string completed = Complete(args[1]);
                        Log.Info($"Completed My Number: {completed}");
                        Console.WriteLine(completed);
                        return 0;
                    case "rangen":
                        Log.Debug($"rangen args: {string.Join(", ", args)}");
                        if (args.Length < 3)
                        {
                            Log.Warn("Not enough argument for rangen");
                            Log.Info("Supply two numbers for range.");
                            return -1;
                        }
                        if (!Regex.IsMatch(args[1], @"^\d+$") || !Regex.IsMatch(args[2], @"^\d+$"))
                        {
                            Log.Warn("Invalid argument for rangen: not numeric");
                            Log.Info("Input needs to be numeric.");
                            return -1;
                        }
                        if (args[1].Length > 12 || args[2].Length > 12)
                        {
                            Log.Warn("Min and/or Max value(s) too large for rangen");
                            Log.Info("Min and/or Max value(s) too large.");
                            return -1;
                        }
                        if (Convert.ToInt64(args[1]) > Convert.ToInt64(args[2]))
                        {
                            Log.Warn("Max value must be larger than Min for rangen");
                            Log.Info("Max value must be larger than Min");
                            return -1;
                        }
                        Log.Info($"Generating range (numerical) from {args[1]} to {args[2]}");
                        RangeN(args[1], args[2]);
                        return 0;
                    case "ranges":
                        Log.Debug($"ranges args: {string.Join(", ", args)}");
                        if (args.Length < 3)
                        {
                            Log.Warn("Not enough argument for ranges");
                            Log.Info("Supply two numbers for range.");
                            return -1;
                        }
                        if (!Regex.IsMatch(args[1], @"^\d+$") || !Regex.IsMatch(args[2], @"^\d+$"))
                        {
                            Log.Warn("Invalid argument for ranges: not numeric");
                            Log.Info("Input needs to be numeric.");
                            return -1;
                        }
                        if (args[1].Length > 11 || args[2].Length > 11)
                        {
                            Log.Warn("Min and/or Max value(s) too large for ranges");
                            Log.Info("Min and/or Max value(s) too large.");
                            return -1;
                        }
                        if (Convert.ToInt64(args[1]) > Convert.ToInt64(args[2]))
                        {
                            Log.Warn("Max value must be larger than Min for ranges");
                            Log.Info("Max value must be larger than Min");
                            return -1;
                        }
                        Log.Info($"Generating range (sequential) from {args[1]} to {args[2]}");
                        RangeS(args[1], args[2]);
                        return 0;
                    default:
                        Log.Error($"Unknown command: {args[0]}");
                        throw new ArgumentOutOfRangeException(args[0]);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Unhandled exception in command: {args[0]}");
                return -1;
            }
        }

        /// <summary>
        ///     Generate "My Number"
        /// </summary>
        /// <param name="count">Count of "My Number"</param>
        private static void Generate(long count)
        {
            var n = new MyNumber();
            for (var i = 0; i < count; i++)
                Console.WriteLine(string.Join("", n.GenerateRandomNumber()));
        }

        /// <summary>
        ///     Check "My Number" for validity
        /// </summary>
        /// <param name="number">"My Number" to check</param>
        /// <returns>Validation result</returns>
        private static bool Check(string number)
        {
            var sanitized = Sanitize(number);

            return MyNumber.VerifyNumber(sanitized);
        }

        /// <summary>
        ///     Supplement first 11th digit of "My Number" with check digit
        /// </summary>
        /// <param name="number">First 11 digits of "My Number"</param>
        /// <returns>Resulting "My Number"</returns>
        private static string Complete(string number)
        {
            var sanitized = Sanitize(number);

            var sum = MyNumber.CalculateCheckDigits(sanitized);
            return number + sum;
        }

        /// <summary>
        ///     Sanitize and convert value for processing
        /// </summary>
        /// <param name="number">String of "My Number" sequence</param>
        /// <returns>Sanitized array of "My Number" sequence</returns>
        private static int[] Sanitize(string number)
        {
            var subject = number.ToCharArray();
            var value = Array.ConvertAll(subject, c => (int) GetNumericValue(c));
            return value;
        }

        /// <summary>
        ///     Generate "My Number" of range specified
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        private static void RangeN(string min, string max)
        {
            var minFilled = Fill(min, RangeMode.Numerical);
            var maxFilled = Fill(max, RangeMode.Numerical);

            Log.Debug("Filled min: " + minFilled);
            Log.Debug("Filled max: " + maxFilled);


            var value = Array.ConvertAll(minFilled.ToCharArray(), c => (int) GetNumericValue(c));
            var stop = Array.ConvertAll(maxFilled.ToCharArray(), c => (int) GetNumericValue(c));

            do
            {
                if (MyNumber.VerifyNumber(value))
                    Console.WriteLine(string.Join("", value));
                value = Increment(value);
            } while (!Compare(value, stop, RangeMode.Numerical) && value != null);
        }

        /// <summary>
        ///     Generate "My Number" of range specified
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        private static void RangeS(string min, string max)
        {
            var minFilled = Fill(min, RangeMode.Sequential);
            var maxFilled = Fill(max, RangeMode.Sequential);

            Log.Debug("Filled min: " + minFilled);
            Log.Debug("Filled max: " + maxFilled);


            var value = Array.ConvertAll(minFilled.ToCharArray(), c => (int) GetNumericValue(c));
            var stop = Array.ConvertAll(maxFilled.ToCharArray(), c => (int) GetNumericValue(c));

            do
            {
                var checkDigit = MyNumber.CalculateCheckDigits(value);
                Console.WriteLine(string.Join("", value) + checkDigit);
                value = Increment(value);
            } while (!Compare(value, stop, RangeMode.Sequential) && value != null);
        }

        /// <summary>
        ///     Fill the value to 12 digits
        /// </summary>
        /// <param name="input">Digits to fill</param>
        /// <param name="mode">Mode of operation</param>
        /// <returns>Value filled to 12 digits</returns>
        private static string Fill(string input, RangeMode mode)
        {
            int reminder;

            switch (mode)
            {
                case RangeMode.Numerical:
                {
                    reminder = 12;
                    break;
                }

                case RangeMode.Sequential:
                {
                    reminder = 11;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            reminder = reminder - input.Length;
            if (reminder == 0) return input;
            for (var i = 0; i < reminder; i++)
                input = "0" + input;
            return input;
        }

        /// <summary>
        ///     Compare two arrays
        /// </summary>
        /// <param name="first">First value</param>
        /// <param name="second">Second value</param>
        /// <param name="mode">Mode of operation</param>
        /// <returns>Result of the comparison</returns>
        private static bool Compare(IReadOnlyList<int> first, IReadOnlyList<int> second, RangeMode mode)
        {
            int reminder;

            switch (mode)
            {
                case RangeMode.Numerical:
                {
                    reminder = 12;
                    break;
                }

                case RangeMode.Sequential:
                {
                    reminder = 11;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            for (var i = 0; i < reminder; i++)
                if (first[i] != second[i])
                    return false;
            return true;
        }

        /// <summary>
        ///     Increment the "My Number" array value
        /// </summary>
        /// <param name="input">Current value</param>
        /// <returns>Value incremented by 1, null if overflow</returns>
        private static int[] Increment(int[] input)
        {
            Array.Reverse(input);
            var digits = input.Length;

            input[0]++;
            for (var i = 0; i < digits; i++)
            {
                if (input[i] <= 9) continue;
                if (i + 1 > digits - 1)
                    return null;
                input[i + 1]++;
                input[i] = 0;
            }

            Array.Reverse(input);
            return input;
        }


        private enum RangeMode
        {
            Numerical,
            Sequential
        }
    }
}