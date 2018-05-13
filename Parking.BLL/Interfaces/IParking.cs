using System;
using System.Collections.Generic;

namespace Parking.BLL.Interfaces
{
    using Parking.BLL.Entities;

    public interface IParking
    {
        int Balance { get; set; }
        List<Car> Cars { get; set; }
        bool IsFreeSpaceInTheParking { get; }
        List<Transaction> Transactions { get; set; }

        string AddCar(Car carToAdd);
        int CountIncomeForLastMinute();
        int CountIncomeForLastMinute(IEnumerable<Transaction> transactions);
        int GetFreeSlotsNumber();
        int GetOccupiedSlotsNumber();
        IEnumerable<Transaction> GetTransactionForLastMinute(DateTime timeNow);
        string RemoveCar(string carIdToAdd);
        void ShowFormattedTransactionLog();
        string TopUpTheCar(Guid carIdToTopUp, int money);
        string ToString();
    }
}