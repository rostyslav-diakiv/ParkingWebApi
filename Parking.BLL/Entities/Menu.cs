namespace Parking.BLL.Entities
{
    using System;

    using Parking.BLL.Interfaces;

    public class Menu : IMenu, IDisposable
    {
        private readonly ParkingEntity _parking;

        public Menu()
        {
            _parking = ParkingEntity.GetParking();
        }

        public void Start(bool showMenu)
        {
            if (showMenu)
            {
                ShowMenu();
            }

            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("\nEnter the balance of a car: ");
                    var balanceString = Console.ReadLine();
                    
                    bool intParseSesult = int.TryParse(balanceString, out int balance);
                    if (intParseSesult)
                    {
                        Console.WriteLine("\nEnter the type of a car: ");
                        var carTypeString = Console.ReadLine();
                        
                        bool typeParseResult = Enum.TryParse(carTypeString, out CarType type);
                        if (typeParseResult)
                        {
                            var car = new Car(balance, type);
                            var response = _parking.AddCar(car);
                            Console.WriteLine(response);
                        }
                        else
                        {
                            Console.WriteLine("Wrong Type of a car");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong Balance");
                    }

                    ShowStandartMessage();
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("\nEnter the if of a car you want to remove: ");
                    var carIdString = Console.ReadLine();
                    var removeResponse = _parking.RemoveCar(carIdString);
                    Console.WriteLine(removeResponse);

                    ShowStandartMessage();
                    break;
                case ConsoleKey.D3:
                    var freeSpace = _parking.GetFreeSlotsNumber();
                    Console.WriteLine($"There are {freeSpace} free places on the parking");
                    ShowStandartMessage();
                    break;
                case ConsoleKey.D4:
                    var occupiedSpace = _parking.GetOccupiedSlotsNumber();
                    Console.WriteLine($"There are {occupiedSpace} occupied places on the parking");
                    ShowStandartMessage();
                    break;
                case ConsoleKey.D5:
                    var incomeForAllTime = _parking.Balance;
                    Console.WriteLine($"There are {incomeForAllTime} parking earned for all time");
                    ShowStandartMessage();
                    break;
                case ConsoleKey.D6:
                    var incomeForLastMinute = _parking.CountIncomeForLastMinute();
                    Console.WriteLine($"There are {incomeForLastMinute} parking earned for the last minute");
                    ShowStandartMessage();
                    break;
                case ConsoleKey.D7:
                    foreach (var car in _parking.Cars)
                    {
                        Console.WriteLine(car);
                    }

                    ShowStandartMessage();
                    break;
                case ConsoleKey.D8:
                    var transactions = _parking.GetTransactionForLastMinute(DateTime.Now);
                    
                    Console.WriteLine("\n{0,-35}{1, 10}{2, 25}{3, 36}\n", "DateTime", "Money", "Type Of Charge", "Id of the Car");

                    foreach (var t in transactions)
                    {
                        Console.WriteLine(
                            "{0,-35}{1, 10}{2, 25}{3, 40}",
                            t.TransactionDate.ToLongDateString() + t.TransactionDate.ToLongTimeString(),
                            t.SpentMoney,
                            t.ChargeType,
                            t.CarId);
                    }

                    ShowStandartMessage();
                    break;
                case ConsoleKey.D9:
                    Console.WriteLine("\nEnter the id of a car you want to top up the balance: ");
                    var carIdToTopUpString = Console.ReadLine();
                    if (Guid.TryParse(carIdToTopUpString, out Guid carId))
                    {
                        Console.WriteLine("\nEnter the amount of money you want to top up: ");
                        var balanceToAddString = Console.ReadLine();

                        bool balanceParseResult = int.TryParse(balanceToAddString, out int balanceToAdd);
                        if (balanceParseResult)
                        {
                            var topUpResponse = _parking.TopUpTheCar(carId, balanceToAdd);
                            Console.WriteLine(topUpResponse);
                        }
                        else
                        {
                            Console.WriteLine("Wrong Balance");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry, id is in wrong format");
                    }

                    ShowStandartMessage();
                    break;
                case ConsoleKey.D0:
                    _parking.ShowFormattedTransactionLog();
                    ShowStandartMessage();
                    break;
                case ConsoleKey.R:
                    Start(true);
                    break;
                case ConsoleKey.C:
                    Console.Clear();
                    ShowStandartMessage();
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Wrong key!");
                    ShowStandartMessage();
                    break;
            }
        }

        private void ShowMenu()
        {
            Console.WriteLine("\nTo add car Press: 1"
                              + "\nTo remove car Press: 2"
                              + "\nTo see number of free slots Press: 3"
                              + "\nTo see number of occupied slots Press: 4"
                              + "\nTo see amount of money that Parking earned for all time Press 5"
                              + "\nTo see amount of money that Parking earned for the last minute Press: 6"
                              + "\nTo see all cars on Parking Press: 7"
                              + "\nTo see all transactions for the last minute Press: 8"
                              + "\nTo top up the car's balance Press: 9"
                              + "\nTo see the Transactions.log Parking Press: 0"
                              + "\nTo clear the console Press: C"
                              + "\nTo Exit from program Press: Esc");
        }

        private void ShowStandartMessage()
        {
            Console.WriteLine("\nPress Esc to exit or \"R\" to show menu or try previous commands again");
            Start(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _parking?.Dispose();
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~Menu()
        {
            Dispose(false);
        }
    }
}
