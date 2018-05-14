using System;

using Microsoft.AspNetCore.Mvc;

namespace Parking.WebApi.Controllers
{
    using Parking.BLL.Dtos;
    using Parking.BLL.Interfaces;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CarsController : Controller
    {
        private readonly IParkingService _parking;

        public CarsController(IParkingService parking)
        {
            _parking = parking;
        }


        /// <summary>
        /// Returns all cars on the parking
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult Get()
        {
            var cars = _parking.Cars;
            
            return Ok(cars);
        }

        /// <summary>
        /// Returns car by id
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(string id)
        {
            if (!Guid.TryParse(id, out Guid guidCarId))
            {
                return BadRequest("Wrong id format");
            }

            var car = _parking.GetCarById(guidCarId);

            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }
        
        /// <summary>
        /// Adds car on the parking
        /// </summary>
        /// <param name="carDto">
        /// The car dto.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost]
        public IActionResult Post([FromBody]CarDto carDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var car = _parking.AddCar(carDto);

            if (car == null)
            {
                return BadRequest("No free Space on the Parking"); // No Free Space on the Parking
            }

            var host = HttpContext.Request.Host;
            var path = HttpContext.Request.Path;
            var scheme = HttpContext.Request.Scheme;

            return Created(new Uri($"{scheme}://{host.Value}{path.Value}/{car.Id}"), car);
        }

        /// <summary>
        /// The delete car by id
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] string id)
        {
            if (!Guid.TryParse(id, out Guid guidCarId))
            {
                return BadRequest("Wrong id format");
            }

            var response = _parking.DeleteCarById(guidCarId);

            if (response)
            {
                return NoContent();
            }

            return StatusCode(500);
        }
    }
}
