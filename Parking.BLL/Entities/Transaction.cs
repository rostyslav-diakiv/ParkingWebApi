namespace Parking.BLL.Entities
{
    using System;

    using Parking.BLL.Interfaces;

    public class Transaction : ITransaction
    {
        private DateTime _transactionDate = DateTime.Now;

        private Guid _carId;

        private int _spentMoney;

        private ChargeType _chargeType;

        public Transaction(int spentMoney, Guid carId, ChargeType typeOfCharge)
        {
            SpentMoney = spentMoney;
            _carId = carId;
            _chargeType = typeOfCharge;
        }

        public DateTime TransactionDate => _transactionDate;

        public Guid CarId => _carId;

        public int SpentMoney
        {
            get => _spentMoney;

            set
            {
                if (_spentMoney != value)
                {
                    _spentMoney = value;
                }
            }
        }

        public ChargeType ChargeType
        {
            get => _chargeType;

            set
            {
                if (_chargeType != value)
                {
                    _chargeType = value;
                }
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return
                $"Transaction was created in {TransactionDate}, {SpentMoney}$ money was charged from {ChargeType.ToString()} of car with id of: {CarId}";
        }
    }
}

