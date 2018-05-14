using System;
using System.Collections.Generic;
using System.Text;

namespace Parking.BLL.Dtos
{
    using System.ComponentModel.DataAnnotations;

    using Parking.BLL.Entities;

    public class CarDto
    {
        private int _balance;

        private CarType _typeOfCar;

        public CarDto()
        {
        }

        public CarDto(int balance, CarType typeOfCar)
        {
            Balance = balance;
            TypeOfCar = typeOfCar;
        }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Car balance should be at least 0 and maximum 2147483647")]
        public int Balance { get; set; }

        [Required]
        [EnumDataType(typeof(CarType), ErrorMessage = "Wrong type of Car")]
        public CarType TypeOfCar { get; set; }


        //public int Balance
        //{
        //    get => _balance;

        //    set
        //    {
        //        if (_balance != value)
        //        {
        //            _balance = value;
        //        }
        //    }
        //}

        //public CarType TypeOfCar
        //{
        //    get => _typeOfCar;

        //    set
        //    {
        //        if (_typeOfCar != value)
        //        {
        //            _typeOfCar = value;
        //        }
        //    }
        //}

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var message = Balance >= 0 ? $"has {Balance}$ money on Balance" : $"has {Math.Abs(Balance)}$ money Debt";
            return $"Car {TypeOfCar.ToString()}, {message}";
        }
    }
}
