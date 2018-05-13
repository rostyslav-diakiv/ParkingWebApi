using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Parking.WebApi.Controllers
{
    using Parking.BLL.Interfaces;

    [Produces("application/json")]
    [Route("api/Parking")]
    public class ParkingController : Controller
    {
        private readonly IParkingEntity _parking;
        public ParkingController(IParkingEntity parking)
        {
            _parking = parking;
        }

        // GET: api/Parking/TotalIncome
        [HttpGet("TotalIncome")]
        public IActionResult Get()
        { 
            return Ok(_parking.Balance);
        }

        // GET: api/Parking/FreeSlots
        [HttpGet("FreeSlots")]
        public IActionResult GetFreeSlots()
        {
            return Ok(_parking.GetFreeSlotsNumber());
        }

        // GET: api/Parking/OccupiedSlots
        [HttpGet("OccupiedSlots")]
        public IActionResult GetOccupiedSlots()
        {
            return Ok(_parking.GetOccupiedSlotsNumber());
        }
    }
}
