using System;

namespace Parking.BLL.Interfaces
{
    using Parking.BLL.Entities;

    public interface ICar
    {
        int Balance { get; set; }
        Guid Id { get; }
        CarType TypeOfCar { get; set; }

        string ToString();
    }
}