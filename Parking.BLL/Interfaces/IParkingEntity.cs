using System;
using System.Collections.Generic;
using Parking.BLL.Dtos;

namespace Parking.BLL.Interfaces
{
    using Parking.BLL.Entities;

    public interface IParkingEntity : IDisposable
    {
        int Balance { get; }
        List<Car> Cars { get; }
        bool IsFreeSpaceInTheParking { get; }

        Car AddCar(Car carToAdd);
        Car AddCar(CarDto carDto);
        int CountIncomeForLastMinute();
        int CountIncomeForLastMinute(IEnumerable<Transaction> transactions);
        bool DeleteCarById(Guid carId);
        Car GetCarById(Guid carId);
        IEnumerable<Transaction> GetCarTransactionsForLastMinute(Guid carId);
        int GetFreeSlotsNumber();
        int GetOccupiedSlotsNumber();
        IEnumerable<Transaction> GetTransactionsForLastMinute(DateTime timeNow);
        string GetTransactionsLog();
        Car TopUpTheCar(Guid carIdToTopUp, int money);
        string ToString();
    }
}