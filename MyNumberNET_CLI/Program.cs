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
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static int Main(string[] args)
        {
            try
            {
                InitializeLogging();
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
                return -1;
            }

            if (args.Length < 1)
            {
                log.Fatal("Not enough argument.");
                log.Info("Usage: command [arg]");
                log.Info("Commands: generate [count]/ check [My Number] / complete [first 11 digits of My Number] / range [minimum] [maximum]");
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
                                log.Fatal("Invalid argument.");
                                log.Info("Input needs to be numberic.");
                                return -1;

                            }

                            Generate(Convert.ToInt64(args[1]));
                            return 0;
                        }

                    case "check":
                        if (args.Length < 2)
                        {
                            log.Fatal("Not enough argument.");
                            log.Info("Supply \"My Number\" to check");
                            return -1;
                        }
                        if (!Regex.IsMatch(args[1], @"^\d+$"))
                        {
                            log.Fatal("Invalid argument.");
                            log.Info("Input needs to be numberic.");
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
                                log.Fatal("Not enough argument.");
                                log.Info("Supply the first 11 digits of \"My Number\" to complete");
                                return -1;
                            }
                            if (!Regex.IsMatch(args[1], @"^\d+$"))
                            {
                                log.Fatal("Invalid argument.");
                                log.Info("Input needs to be numberic.");
                                return -1;

                            }

                            Console.WriteLine(Complete(args[1]));
                            return 0;
                        }

                    case "range":
                        {
                            if (args.Length < 3)
                            {
                                log.Fatal("Not enough argument.");
                                log.Info("Supply two numbers for range.");
                            }
                            if (!Regex.IsMatch(args[1], @"^\d+$") || !Regex.IsMatch(args[2], @"^\d+$"))
                            {
                                log.Fatal("Invalid argument.");
                                log.Info("Input needs to be numberic.");
                                return -1;
                            }
                            if(args[1].Length > 12 || args[2].Length > 12)
                            {
                                log.Fatal("Invalid argument.");
                                log.Info("Min and/or Max value(s) too large.");
                                return -1;
                            }
                            if(Convert.ToInt64(args[1]) > Convert.ToInt64(args[2]))
                            {
                                log.Fatal("Invalid argument.");
                                log.Info("Max value must be larger than Min");
                                return -1;
                            }

                            Range(args[1], args[2]);

                            return 0;
                        }
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
                log.Debug(ex.StackTrace);
                return -1;
            }
            return -1;
        }

        /// <summary>
        /// Generate "My Number"
        /// </summary>
        /// <param name="count">Count of "My Number"</param>
        static private void Generate(Int64 count)
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
        static private bool Check(string number)
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
        private static void Range(string min, string  max)
        {
            var min_filled = Fill(min);
            var max_filled = Fill(max);

            log.Debug("Filled min: " + min_filled);
            log.Debug("Filled max: " + max_filled);


            int[] value = Array.ConvertAll(min_filled.ToCharArray(), c => (int)Char.GetNumericValue(c));
            int[] stop = Array.ConvertAll(max_filled.ToCharArray(), c => (int)Char.GetNumericValue(c));

            var n = new MyNumber();

            do
            {
                if (n.VerifyNumber(value))
                {
                    Console.WriteLine(string.Join("", value));
                }
                value = Increment(value);

            } while (!Compare(value, stop) && value != null);
        }

        /// <summary>
        /// Fill the value to 12 didits
        /// </summary>
        /// <param name="input">Digits to fill</param>
        /// <returns>Value filled to 12 digits</returns>
        private static string Fill(string input)
        {
            var reminder = 12 - input.Length;
            if(reminder != 0)
            {
                for(int i = 0; i < reminder; i++)
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
        /// <returns>Result of the comparison</returns>
        private static bool Compare(int[] first, int[] second)
        {
            for(int i = 0; i < 12; i++)
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
            input[0]++;
            for(int i = 0; i < 12; i++)
            {
                if(input[i] > 9)
                {
                    if(i+1 > 11)
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
                XmlDocument log4netConfig = new XmlDocument();

                using (StreamReader reader = new StreamReader(new FileStream("log4net.config", FileMode.Open, FileAccess.Read)))
                {
                    log4netConfig.Load(reader);
                }

                ILoggerRepository rep = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                XmlConfigurator.Configure(rep, log4netConfig["log4net"]);
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
