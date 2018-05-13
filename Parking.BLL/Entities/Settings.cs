namespace Parking.BLL.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    public static class Settings
    {
        public const int Timeout = 3;

        public const int LogTimeout = 60;

        public const int ParkingSpace = 25;

        public const int Fine = 3;

        private static readonly string _transactionLogFilePath;

        private static readonly string _exceptionsLogFilePath;

        private static ReadOnlyDictionary<CarType, int> _priceList = new ReadOnlyDictionary<CarType, int>(
            new Dictionary<CarType, int>(4)
                {
                    { CarType.Bus, 2 },
                    { CarType.Truck, 5 },
                    { CarType.Motorcycle, 1 },
                    { CarType.Passenger, 3 }
                });

        static Settings()
        {
            try
            {
                var logsDirPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                if (!Directory.Exists(logsDirPath))
                {
                    Directory.CreateDirectory(logsDirPath);
                }

                _transactionLogFilePath = Path.Combine(logsDirPath, "Transactions.log");
                _exceptionsLogFilePath = Path.Combine(logsDirPath, "Exceptions.log");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static ReadOnlyDictionary<CarType, int> PriceList => _priceList;

        public static string TransactionsLogFilePath => _transactionLogFilePath;

        public static string ExceptionsLogFilePath => _exceptionsLogFilePath;
    }
}
