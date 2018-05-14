namespace Parking.BLL.Entities
{
    using System;

    using Parking.BLL.Dtos;
    using Parking.BLL.Interfaces;

    public class Car : ICar
    {
        private int _balance;

        private CarType _typeOfCar;

        public Car(CarDto carDto)
        {
            Balance = carDto.Balance;
            TypeOfCar = carDto.TypeOfCar;
        }

        public Car(int balance, CarType typeOfCar)
        {
            Balance = balance;
            TypeOfCar = typeOfCar;
        }

        public Guid Id { get; } = Guid.NewGuid();

        public int Balance
        {
            get => _balance;

            set
            {
                if (_balance != value)
                {
                    _balance = value;
                }
            }
        }

        public CarType TypeOfCar
        {
            get => _typeOfCar;

            set
            {
                if (_typeOfCar != value)
                {
                    _typeOfCar = value;
                }
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var message = Balance >= 0 ? $"has {Balance}$ money on Balance" : $"has {Math.Abs(Balance)}$ money Debt";
            return $"Car {TypeOfCar.ToString()} with id: {Id}, {message}";
        }
    }
}
