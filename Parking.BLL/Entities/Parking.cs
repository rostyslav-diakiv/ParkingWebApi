using System.Collections.Generic;

namespace Parking.BLL.Entities
{
    using System;
    using System.Linq;
    using System.Timers;

    using Parking.BLL.Interfaces;

    public class ParkingEntity : IParking, IDisposable
    {
        private static readonly Lazy<ParkingEntity> _parking = new Lazy<ParkingEntity>(() => new ParkingEntity());

        private readonly Logger _logger;

        private object _balanceLocker = new object();
        private object _carLocker = new object();
        private object _transLocker = new object();
        private object _loggerLocker = new object();

        private readonly Timer _chargeMoneyTimer;
        private readonly Timer _writeLogsTimer;

        private ParkingEntity()
        {
            _logger = Logger.GetLogger();

            Transactions = new List<Transaction>();
            Balance = 0;

            try
            {
                Cars = new List<Car>(Settings.ParkingSpace);
                 _chargeMoneyTimer = new Timer(Settings.Timeout * 1000);
                _writeLogsTimer = new Timer(Settings.LogTimeout * 1000);

                _chargeMoneyTimer.Elapsed += Charge;
                _chargeMoneyTimer.AutoReset = true;
                _chargeMoneyTimer.Enabled = true;

                _writeLogsTimer.Elapsed += WriteLogs;
                _writeLogsTimer.AutoReset = true;
                _writeLogsTimer.Enabled = true;

            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Can't Initialize Cars on Parking");
                lock (_loggerLocker)
                {
                    _logger.LogError(e);
                }

                Cars = new List<Car>();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Can't setup money charging and writing logs");
                lock (_loggerLocker)
                {
                    _logger.LogError(e);
                }

                Cars = new List<Car>();
            }
        }

        public bool IsFreeSpaceInTheParking => GetFreeSlotsNumber() > 0;

        public List<Car> Cars { get; set; }

        public List<Transaction> Transactions { get; set; }

        public int Balance { get; set; }

        public static ParkingEntity GetParking()
        {
            return _parking.Value;
        }

        public int GetFreeSlotsNumber()
        {
            return Cars.Capacity - Cars.Count;
        }

        public int GetOccupiedSlotsNumber()
        {
            return Cars.Capacity - GetFreeSlotsNumber();
        }

        public void ShowFormattedTransactionLog()
        {
            Console.WriteLine();
            lock (_loggerLocker)
            {
                _logger.DumpLog();
            }
        }

        public string AddCar(Car carToAdd)
        {
            lock (_carLocker)
            {
                if (Cars.Contains(carToAdd))
                {
                    return "There are already such car on the parking";
                }

                if (!IsFreeSpaceInTheParking)
                {
                    return "The parking is full. remove some car and try again";
                }

                Cars.Add(carToAdd);

                return $"{carToAdd} was successfully added on parking";
            }
        }

        public string RemoveCar(string carIdToAdd)
        {
            if (!Guid.TryParse(carIdToAdd, out Guid carId))
            {
                return "Sorry, id is in wrong format";
            }

            try
            {
                lock (_carLocker)
                {
                    var carsToRemove = Cars.Where(c => c.Id == carId).ToList();

                    if (!carsToRemove.Any())
                    {
                        return "Sorry, there no cars with such id";
                    }

                    if (carsToRemove.Count > 1)
                    {
                        lock (_loggerLocker)
                        {
                            _logger.LogError("Several cars with equal ids");
                        }

                        return "Sorry, there are several cars with such id. \nPlease, try another id";
                    }

                    if (carsToRemove[0].Balance < 0)
                    {
                        return "Sorry, car has a debt before parking and can't be removed from parking. \nPopulate the balance of the car and try again";
                    }

                    Cars.Remove(carsToRemove[0]);
                    return $"{carsToRemove} was successfully removed from parking";
                }
            }
            catch (ArgumentNullException e)
            {
                lock (_loggerLocker)
                {
                    _logger.LogError(e);
                }
                return "Error occured while removing car from parking. \nPlease try again later";
            }
        }

        public string TopUpTheCar(Guid carIdToTopUp, int money)
        {
            if (money <= 0)
            {
                return "Input positive amount of money you want to top up";
            }

            try
            {
                lock (_carLocker)
                {
                    var carToTopUp = Cars.Where(c => c.Id == carIdToTopUp).ToList();

                    if (!carToTopUp.Any())
                    {
                        return "Sorry, there no cars with such id";
                    }

                    if (carToTopUp.Count > 1)
                    {
                        lock (_loggerLocker)
                        {
                            _logger.LogError("Several cars with equal ids");
                        }

                        return "Sorry, there are several cars with such id. \nPlease, try another id";
                    }

                    carToTopUp[0].Balance += money;
                    return $"The balance of {carIdToTopUp} was successfully topped up";
                }
            }
            catch (ArgumentNullException e)
            {
                lock (_loggerLocker)
                {
                    _logger.LogError(e);
                }

                return "Error occured while topping up the car. \nPlease try again later";
            }
        }

        public IEnumerable<Transaction> GetTransactionForLastMinute(DateTime timeNow)
        {
            try
            {
                var timeMinuteAgo = timeNow.AddMinutes(-1);
                lock (_transLocker)
                {
                    var transactions = Transactions.Where(t => t.TransactionDate > timeMinuteAgo).ToList();
                    return transactions;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                lock (_loggerLocker)
                {
                    _logger.LogError(e);
                }

                return new List<Transaction>();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex);
                lock (_loggerLocker)
                {
                    _logger.LogError(ex);
                }

                return new List<Transaction>();
            }
        }

        public int CountIncomeForLastMinute()
        {
            var transactions = GetTransactionForLastMinute(DateTime.Now);

            var sumOfTransactionsPayments = 0;

            foreach (var t in transactions)
            {
                sumOfTransactionsPayments += t.SpentMoney;
            }

            return sumOfTransactionsPayments;
        }

        public int CountIncomeForLastMinute(IEnumerable<Transaction> transactions)
        {
            var sumOfTransactionsPayments = 0;

            foreach (var t in transactions)
            {
                sumOfTransactionsPayments += t.SpentMoney;
            }

            return sumOfTransactionsPayments;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string priceList = "Price list:\n";
            foreach (var key in Settings.PriceList.Keys)
            {
                priceList += $"{key.ToString()} - {Settings.PriceList[key]}\n";
            }

            return $"Single Parking has {GetFreeSlotsNumber()} free parking slots of {Settings.ParkingSpace}.\n" +
                   $"Current balance of parking is {Balance}$.\n {priceList}";
        }

        #region Private methods

        private void WriteLogs(object source, ElapsedEventArgs e)
        {
            var transactions = GetTransactionForLastMinute(e.SignalTime);

            RemoveOldTransactions(e.SignalTime.AddMinutes(-1));

            var sumOfTransactionsPayments = CountIncomeForLastMinute(transactions);

            lock (_loggerLocker)
            {
                _logger.LogInfo($"{sumOfTransactionsPayments}$ Parking earned this minute", e.SignalTime);
            }
        }

        private void Charge(object source, ElapsedEventArgs e)
        {
            lock (_carLocker)
            {
                foreach (var car in Cars)
                {
                    if (Settings.PriceList.TryGetValue(car.TypeOfCar, out int price))
                    {
                        var chargeType = ChargeType.Balance;
                        if (car.Balance < price)
                        {
                            price *= Settings.Fine;
                            chargeType = ChargeType.Debt;
                        }

                        car.Balance = car.Balance - price;
                        lock (_balanceLocker)
                        {
                            Balance += price;
                        }

                        lock (_transLocker)
                        {
                            Transactions.Add(new Transaction(price, car.Id, chargeType));
                        }
                    }
                }
            }
        }

        private void RemoveOldTransactions(DateTime time)
        {
            try
            {
                lock (_transLocker)
                {
                    Transactions.RemoveAll(t => t.TransactionDate < time);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                lock (_loggerLocker)
                {
                    _logger.LogError(e);
                }
            }
        }

        #endregion


        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _chargeMoneyTimer.Elapsed -= Charge;
                _writeLogsTimer.Elapsed -= WriteLogs;

                _chargeMoneyTimer?.Dispose();
                _writeLogsTimer?.Dispose();
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ParkingEntity"/> class. Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~ParkingEntity()
        {
            Dispose(false);
        }
    }
}
