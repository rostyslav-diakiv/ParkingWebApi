using System;

namespace Parking.BLL.Interfaces
{
    using Parking.BLL.Entities;

    public interface ITransaction
    {
        Guid CarId { get; }
        ChargeType ChargeType { get; set; }
        int SpentMoney { get; set; }
        DateTime TransactionDate { get; }

        string ToString();
    }
}