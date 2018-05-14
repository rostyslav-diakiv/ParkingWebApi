using System;

using Microsoft.AspNetCore.Mvc;

namespace Parking.WebApi.Controllers
{
    using Parking.BLL.Dtos;
    using Parking.BLL.Interfaces;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly IParkingService _parking;

        public TransactionsController(IParkingService parking)
        {
            _parking = parking;
        }

        /// <summary>
        /// Gets all Transactions for the last minute
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
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

        /// <summary>
        /// Updates car on the parking
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="carDto">
        /// The car dto.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPut("Cars/{id}")]
        public IActionResult Put([FromRoute] string id, [FromBody]TopUpCarDto carDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Guid.TryParse(id, out Guid guidCarId))
            {
                return BadRequest("Wrong id format");
            }

            var car = _parking.TopUpTheCar(guidCarId, carDto.Balance);

            if (car == null)
            {
                return StatusCode(500);
            }

            return NoContent();
        }

        /// <summary>
        /// Gets all car's Transactions for the last minute by car's id
        /// </summary>
        /// <param name="carId">
        /// The car id.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
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

        /// <summary>
        /// Returns logs as json objects about parking's minute income 
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet("LogJson")]
        public IActionResult GetLogJson()
        {
            var logDtos = _parking.GetTransactionJsonLog();

            if (logDtos == null)
            {
                return StatusCode(500);
            }

            return Ok(logDtos);
        }

        /// <summary>
        /// Returns logs in json format about parking's minute income 
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet("Log")]
        public IActionResult GetLog()
        {
            var logs = _parking.GetTransactionsLog();

            if (logs == null)
            {
                return StatusCode(500);
            }

            return Ok(logs);
        }
    }
}
