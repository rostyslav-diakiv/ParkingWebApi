using System;

namespace Parking.BLL.Entities
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;

    using Newtonsoft.Json;

    using Parking.BLL.Dtos;
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

        public void LogJsonInfo(LogDto logDto)
        {
            LogJson(JsonConvert.SerializeObject(logDto), Settings.TransactionsJsonLogFilePath);
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

        public StringCollection GetLogs()
        {
            StringCollection logLines = new StringCollection();
            try
            {
                using (StreamReader sr = File.OpenText(Settings.TransactionsLogFilePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        logLines.Add(line);
                    }

                    return logLines;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                Console.WriteLine("Error occurred while trying read from Transactions.log");
                return null;
            }
        }

        public IEnumerable<LogDto> GetJsonLogs()
        {
            ICollection<LogDto> logDtos = new List<LogDto>();
            try
            {
                using (StreamReader sr = File.OpenText(Settings.TransactionsJsonLogFilePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var logDto = JsonConvert.DeserializeObject<LogDto>(line);
                        logDtos.Add(logDto);
                    }
                }

                return logDtos;
            }
            catch (Exception ex)
            {
                LogError(ex);
                Console.WriteLine("Error occurred while trying read from Transactions.json");
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

        private void LogJson(string json, string path)
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
                            tw.WriteLine(json);
                        }
                    }
                    else if (File.Exists(path))
                    {
                        using (var tw = new StreamWriter(path, true))
                        {
                            tw.WriteLine(json);
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
