using System.Collections.Generic;

namespace Parking.BLL.Entities
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Timers;

    using Parking.BLL.Dtos;
    using Parking.BLL.Interfaces;

    public class ParkingEntity : IParkingEntity
    {
        private readonly Logger _logger;

        private object _balanceLocker = new object();
        private object _carLocker = new object();
        private object _transLocker = new object();
        private object _loggerLocker = new object();

        private readonly Timer _chargeMoneyTimer;
        private readonly Timer _writeLogsTimer;

        public ParkingEntity()
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

        public List<Car> Cars { get; }

        private List<Transaction> Transactions { get; }

        public int Balance { get; private set; }

        public int GetFreeSlotsNumber()
        {
            return Cars.Capacity - Cars.Count;
        }

        public int GetOccupiedSlotsNumber()
        {
            return Cars.Capacity - GetFreeSlotsNumber();
        }

        public string GetTransactionsLog()
        {
            lock (_loggerLocker)
            {
                return _logger.GetLogs();
            }
        }

        public Car TopUpTheCar(Guid carIdToTopUp, int money)
        {
            try
            {
                lock (_carLocker)
                {
                    var carToTopUp = Cars.Where(c => c.Id == carIdToTopUp).ToList();

                    if (!carToTopUp.Any())
                    {
                        throw new HttpStatusCodeException((int)HttpStatusCode.NotFound, "Car with such id not found");
                    }

                    if (carToTopUp.Count > 1)
                    {
                        lock (_loggerLocker)
                        {
                            _logger.LogError("Several cars with equal ids");
                        }

                        throw new HttpStatusCodeException((int)HttpStatusCode.Conflict, "Several cars with equal ids");
                    }

                    carToTopUp[0].Balance += money;
                    return carToTopUp[0];
                }
            }
            catch (ArgumentNullException e)
            {
                lock (_loggerLocker)
                {
                    _logger.LogError(e);
                }

                return null;
            }
        }

        public IEnumerable<Transaction> GetTransactionsForLastMinute(DateTime timeNow)
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

                return null;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex);
                lock (_loggerLocker)
                {
                    _logger.LogError(ex);
                }

                return null;
            }
        }

        public IEnumerable<Transaction> GetCarTransactionsForLastMinute(Guid carId)
        {
            try
            {
                lock (_carLocker)
                {
                    var cars = Cars.Where(c => c.Id == carId).ToList();

                    if (!cars.Any())
                    {
                        throw new HttpStatusCodeException((int)HttpStatusCode.NotFound, "Car with such id not found");
                    }

                    if (cars.Count > 1)
                    {
                        lock (_loggerLocker)
                        {
                            _logger.LogError("Several cars with equal ids");
                        }

                        throw new HttpStatusCodeException((int)HttpStatusCode.Conflict, "Several cars with equal ids");
                    }
                }

                var timeMinuteAgo = DateTime.Now.AddMinutes(-1);
                lock (_transLocker)
                {
                    var transactions = Transactions.Where(t => t.TransactionDate > timeMinuteAgo && t.CarId == carId).ToList();
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

                return null;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex);
                lock (_loggerLocker)
                {
                    _logger.LogError(ex);
                }

                return null;
            }
        }

        public Car AddCar(Car carToAdd)
        {
            lock (_carLocker)
            {
                if (!IsFreeSpaceInTheParking)
                {
                    return null;
                }
                Cars.Add(carToAdd);
                return carToAdd;
            }
        }

        public Car AddCar(CarDto carDto)
        {
            lock (_carLocker)
            {
                if (!IsFreeSpaceInTheParking)
                {
                    return null;
                }

                var car = new Car(carDto);
                Cars.Add(car);
                return car;
            }
        }

        public Car GetCarById(Guid carId)
        {
            try
            {
                lock (_carLocker)
                {
                    var car = Cars.FirstOrDefault(c => c.Id == carId);
                    return car;
                }
            }
            catch (ArgumentNullException e)
            {
                lock (_loggerLocker)
                {
                    _logger.LogError(e);
                }

                return null;
            }
        }

        public bool DeleteCarById(Guid carId)
        {
            try
            {
                lock (_carLocker)
                {
                    var carsToRemove = Cars.Where(c => c.Id == carId).ToList();

                    if (!carsToRemove.Any())
                    {
                        throw new HttpStatusCodeException((int)HttpStatusCode.NotFound, "Car with such id not found");
                    }

                    if (carsToRemove.Count > 1)
                    {
                        lock (_loggerLocker)
                        {
                            _logger.LogError("Several cars with equal ids");
                        }

                        throw new HttpStatusCodeException((int)HttpStatusCode.Conflict, "Several cars with equal ids");
                    }

                    Cars.Remove(carsToRemove[0]);
                    return true;
                }
            }
            catch (ArgumentNullException e)
            {
                lock (_loggerLocker)
                {
                    _logger.LogError(e);
                }

                return false;
            }
        }

        public int CountIncomeForLastMinute()
        {
            var transactions = GetTransactionsForLastMinute(DateTime.Now);

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
            var transactions = GetTransactionsForLastMinute(e.SignalTime);

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
