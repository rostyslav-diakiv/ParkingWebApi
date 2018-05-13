using System;

namespace Parking.BLL.Entities
{
    using System.IO;
    using System.Threading.Tasks;

    using Parking.BLL.Interfaces;

    public class Logger : ILogger
    {
        private static readonly Lazy<Logger> _logger = new Lazy<Logger>(() => new Logger());

        private object _locker = new object();

        private Logger()
        {
        }

        public static Logger GetLogger()
        {
            return _logger.Value;
        }

        public void LogInfo(string info, DateTime time)
        {
            Log(info, time, Settings.TransactionsLogFilePath);
        }

        public void LogError(Exception ex)
        {
            var text = ex.Message + Environment.NewLine +
                ex.Source + Environment.NewLine +
                ex.InnerException + Environment.NewLine +
                ex.StackTrace;

            Log(text, DateTime.Now, Settings.ExceptionsLogFilePath);
        }

        public void LogError(string text)
        {
            Log(text, DateTime.Now, Settings.ExceptionsLogFilePath);
        }

        public void DumpLog()
        {
            lock (_locker)
            {
                if (!File.Exists(Settings.TransactionsLogFilePath))
                {
                    LogError("Log File with that path doesn't exist");
                    return;
                }

                try
                {
                    using (StreamReader r = File.OpenText(Settings.TransactionsLogFilePath))
                    {
                        string line;
                        while ((line = r.ReadLine()) != null)
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogError(e);
                    Console.WriteLine("Error occurred while trying read from Transactions.log");
                }
            }
        }

        public string GetLogs()
        {
            try
            {
                using (StreamReader sr = File.OpenText(Settings.TransactionsLogFilePath))
                {
                    string line = sr.ReadToEnd();
                    return line;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                Console.WriteLine("Error occurred while trying read from Transactions.log");
                return null;
            }
        }

        private void Log(string info, DateTime time, string path)
        {
            lock (_locker)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        File.Create(path).Dispose();
                        using (var tw = new StreamWriter(path))
                        {
                            tw.WriteLine($"{time.ToLongDateString()} {time.ToLongTimeString()}");
                            tw.WriteLine($" : {info}");
                            tw.WriteLine("--------------------------------------------");
                        }
                    }
                    else if (File.Exists(path))
                    {
                        using (var tw = new StreamWriter(path, true))
                        {
                            tw.WriteLine($"{time.ToLongDateString()} {time.ToLongTimeString()}");
                            tw.WriteLine($" : {info}");
                            tw.WriteLine("--------------------------------------------");
                        }
                    }
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }
        }
    }
}
