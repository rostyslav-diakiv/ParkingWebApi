using System;

using Microsoft.AspNetCore.Mvc;

namespace Parking.WebApi.Controllers
{
    using Parking.BLL.Interfaces;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly IParkingEntity _parking;

        public TransactionsController(IParkingEntity parking)
        {
            _parking = parking;
        }

        // GET: api/Transactions/LastMinute
        [HttpGet("LastMinute")]
        public IActionResult GetForLastMinute()
        {
            var transactions = _parking.GetTransactionsForLastMinute(DateTime.Now);

            if (transactions == null)
            {
                return StatusCode(500);
            }

            return Ok(transactions);
        }

        // GET: api/Transactions/Cars/{carId}
        [HttpGet("Cars/{carId}")]
        public IActionResult Get(string carId)
        {
            if (!Guid.TryParse(carId, out Guid guidCarId))
            {
                return BadRequest("Wrong id format");
            }

            var transactions = _parking.GetCarTransactionsForLastMinute(guidCarId);

            if (transactions == null)
            {
                return StatusCode(500);
            }

            return Ok(transactions);
        }

        // GET: api/Transactions/Log
        [HttpGet("Log")]
        public IActionResult GetLog()
        {
            var transactionsString = _parking.GetTransactionsLog();

            if (transactionsString == null)
            {
                return StatusCode(500);
            }

            return Ok(transactionsString);
        }
    }
}
