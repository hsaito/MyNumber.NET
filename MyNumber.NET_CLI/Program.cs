using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository;
using MyNumberNET;

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
                log.Info("Commands: generate [count]/ check [My Number] / complete [first 11 digits of My Number]");
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
                            Console.WriteLine(Complete(args[1]));
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
