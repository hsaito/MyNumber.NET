using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository;
using MyNumberNET;
using System.Text.RegularExpressions;

namespace MyNumberNET_CLI
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        static int Main(string[] args)
        {
            try
            {
                var result = InitializeLogging();
                if(!result)
                    throw new Exception("Logging could not be enabled.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
                return -1;
            }

            if (args.Length < 1)
            {
                Log.Fatal("Not enough argument.");
                Log.Info("Usage: command [arg]");
                Log.Info("Commands: generate [count]/ check [My Number] / complete [first 11 digits of My Number]");
                Log.Info("rangen [minimum] [maximum] / ranges [minimum [maximum]");
                Log.Info("Range functionss are rangen (numerical range), and ranges (sequential range)");
                return -1;
            }

            try
            {
                switch (args[0])
                {

                    case "generate":
                        if (args.Length == 1)
                        {
                            Generate(1);
                            return 0;
                        }
                        else
                        {
                            if (!Regex.IsMatch(args[1], @"^\d+$"))
                            {
                                Log.Fatal("Invalid argument.");
                                Log.Info("Input needs to be numberic.");
                                return -1;

                            }

                            Generate(Convert.ToInt64(args[1]));
                            return 0;
                        }

                    case "check":
                        if (args.Length < 2)
                        {
                            Log.Fatal("Not enough argument.");
                            Log.Info("Supply \"My Number\" to check");
                            return -1;
                        }
                        if (!Regex.IsMatch(args[1], @"^\d+$"))
                        {
                            Log.Fatal("Invalid argument.");
                            Log.Info("Input needs to be numberic.");
                            return -1;

                        }
                        if (Check(args[1]))
                        {
                            Console.WriteLine("OK");
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine("ERROR");
                            return 1;
                        }

                    case "complete":
                        {
                            if (args.Length < 2)
                            {
                                Log.Fatal("Not enough argument.");
                                Log.Info("Supply the first 11 digits of \"My Number\" to complete");
                                return -1;
                            }
                            if (!Regex.IsMatch(args[1], @"^\d+$"))
                            {
                                Log.Fatal("Invalid argument.");
                                Log.Info("Input needs to be numberic.");
                                return -1;

                            }

                            Console.WriteLine(Complete(args[1]));
                            return 0;
                        }

                    case "rangen":
                        {
                            if (args.Length < 3)
                            {
                                Log.Fatal("Not enough argument.");
                                Log.Info("Supply two numbers for range.");
                            }
                            if (!Regex.IsMatch(args[1], @"^\d+$") || !Regex.IsMatch(args[2], @"^\d+$"))
                            {
                                Log.Fatal("Invalid argument.");
                                Log.Info("Input needs to be numberic.");
                                return -1;
                            }
                            if (args[1].Length > 12 || args[2].Length > 12)
                            {
                                Log.Fatal("Invalid argument.");
                                Log.Info("Min and/or Max value(s) too large.");
                                return -1;
                            }
                            if (Convert.ToInt64(args[1]) > Convert.ToInt64(args[2]))
                            {
                                Log.Fatal("Invalid argument.");
                                Log.Info("Max value must be larger than Min");
                                return -1;
                            }

                            RangeN(args[1], args[2]);

                            return 0;
                        }

                    case "ranges":
                        {
                            if (args.Length < 3)
                            {
                                Log.Fatal("Not enough argument.");
                                Log.Info("Supply two numbers for range.");
                            }
                            if (!Regex.IsMatch(args[1], @"^\d+$") || !Regex.IsMatch(args[2], @"^\d+$"))
                            {
                                Log.Fatal("Invalid argument.");
                                Log.Info("Input needs to be numberic.");
                                return -1;
                            }
                            if (args[1].Length > 11 || args[2].Length > 11)
                            {
                                Log.Fatal("Invalid argument.");
                                Log.Info("Min and/or Max value(s) too large.");
                                return -1;
                            }
                            if (Convert.ToInt64(args[1]) > Convert.ToInt64(args[2]))
                            {
                                Log.Fatal("Invalid argument.");
                                Log.Info("Max value must be larger than Min");
                                return -1;
                            }

                            RangeS(args[1], args[2]);

                            return 0;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
                Log.Debug(ex.StackTrace);
                return -1;
            }
            return -1;
        }

        /// <summary>
        /// Generate "My Number"
        /// </summary>
        /// <param name="count">Count of "My Number"</param>
        private static void Generate(Int64 count)
        {
            var n = new MyNumber();
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(string.Join("", n.GenerateRandomNumber()));
            }
        }

        /// <summary>
        /// Check "My Number" for validity
        /// </summary>
        /// <param name="number">"My Number" to check</param>
        /// <returns>Validation result</returns>
        private static bool Check(string number)
        {
            var n = new MyNumber();
            var sanitized = Sanitize(number);

            return n.VerifyNumber(sanitized);
        }

        /// <summary>
        /// Supplement first 11th digit of "My Number" with check digit
        /// </summary>
        /// <param name="number">First 11 digits of "My Number"</param>
        /// <returns>Resulting "My Number"</returns>
        static string Complete(string number)
        {
            var n = new MyNumber();

            var sanitized = Sanitize(number);

            var sum = n.CalculateCheckDigits(sanitized);
            return number + sum;
        }

        /// <summary>
        /// Sanitize and convert value for processing
        /// </summary>
        /// <param name="number">String of "My Number" sequence</param>
        /// <returns>Sanitized array of "My Number" sequence</returns>
        private static int[] Sanitize(string number)
        {
            var subject = number.ToCharArray();
            int[] value = Array.ConvertAll(subject, c => (int)Char.GetNumericValue(c));
            return value;
        }

        /// <summary>
        /// Generate "My Number" of range specified
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        private static void RangeN(string min, string max)
        {
            var minFilled = Fill(min, RangeMode.Numerical);
            var maxFilled = Fill(max, RangeMode.Numerical);

            Log.Debug("Filled min: " + minFilled);
            Log.Debug("Filled max: " + maxFilled);


            int[] value = Array.ConvertAll(minFilled.ToCharArray(), c => (int)Char.GetNumericValue(c));
            int[] stop = Array.ConvertAll(maxFilled.ToCharArray(), c => (int)Char.GetNumericValue(c));

            var n = new MyNumber();

            do
            {
                if (n.VerifyNumber(value))
                {
                    Console.WriteLine(string.Join("", value));
                }
                value = Increment(value);

            } while (!Compare(value, stop, RangeMode.Numerical) && value != null);
        }

        /// <summary>
        /// Generate "My Number" of range specified
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        private static void RangeS(string min, string max)
        {
            var minFilled = Fill(min, RangeMode.Sequential);
            var maxFilled = Fill(max, RangeMode.Sequential);

            Log.Debug("Filled min: " + minFilled);
            Log.Debug("Filled max: " + maxFilled);


            int[] value = Array.ConvertAll(minFilled.ToCharArray(), c => (int)Char.GetNumericValue(c));
            int[] stop = Array.ConvertAll(maxFilled.ToCharArray(), c => (int)Char.GetNumericValue(c));

            var n = new MyNumber();

            do
            {
                var checkDigit = n.CalculateCheckDigits(value);
                Console.WriteLine(string.Join("", value)+checkDigit.ToString());
                value = Increment(value);

            } while (!Compare(value, stop, RangeMode.Sequential) && value != null);
        }

        private enum RangeMode { Numerical, Sequential }

        /// <summary>
        /// Fill the value to 12 didits
        /// </summary>
        /// <param name="input">Digits to fill</param>
        /// <param name="mode">Mode of operation</param>
        /// <returns>Value filled to 12 digits</returns>
        private static string Fill(string input, RangeMode mode)
        {
            int reminder = 12;

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
            }

            reminder = reminder - input.Length;
            if (reminder != 0)
            {
                for (int i = 0; i < reminder; i++)
                {
                    input = "0" + input;
                }
            }
            return input;
        }

        /// <summary>
        /// Compare two arrays
        /// </summary>
        /// <param name="first">First value</param>
        /// <param name="second">Second value</param>
        /// <param name="mode">Mode of operation</param>
        /// <returns>Result of the comparison</returns>
        private static bool Compare(int[] first, int[] second, RangeMode mode)
        {
            int reminder = 12;

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
            }

            for (int i = 0; i < reminder; i++)
            {
                if (first[i] != second[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Increment the "My Number" array value
        /// </summary>
        /// <param name="input">Current value</param>
        /// <returns>Value incremented by 1, null if overflow</returns>
        private static int[] Increment(int[] input)
        {
            Array.Reverse(input);
            int digits = input.Length;

            input[0]++;
            for (int i = 0; i < digits; i++)
            {
                if (input[i] > 9)
                {
                    if (i + 1 > digits - 1)
                    {
                        return null;
                    }
                    input[i + 1]++;
                    input[i] = 0;
                }
            }
            Array.Reverse(input);
            return input;
        }

        /// <summary>
        /// Initialize logging
        /// </summary>
        private static bool InitializeLogging()
        {
            try
            {
                // Configuration for logging
                XmlDocument log4NetConfig = new XmlDocument();

                using (StreamReader reader = new StreamReader(new FileStream("log4net.config", FileMode.Open, FileAccess.Read)))
                {
                    log4NetConfig.Load(reader);
                }

                ILoggerRepository rep = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                XmlConfigurator.Configure(rep, log4NetConfig["log4net"]);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing the logging.");
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
