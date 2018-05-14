using Microsoft.AspNetCore.Mvc;

namespace Parking.WebApi.Controllers
{
    using Parking.BLL.Interfaces;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ParkingController : Controller
    {
        private readonly IParkingService _parking;
        public ParkingController(IParkingService parking)
        {
            _parking = parking;
        }

        /// <summary>
        /// Returns total income of the parking
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet("TotalIncome")]
        public IActionResult Get()
        { 
            return Ok(_parking.Balance);
        }

        /// <summary>
        /// Returns amount of free slots on the parking
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet("FreeSlots")]
        public IActionResult GetFreeSlots()
        {
            return Ok(_parking.GetFreeSlotsNumber());
        }

        /// <summary>
        /// Returns amount of occupied slots on the parking
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet("OccupiedSlots")]
        public IActionResult GetOccupiedSlots()
        {
            return Ok(_parking.GetOccupiedSlotsNumber());
        }
    }
}
