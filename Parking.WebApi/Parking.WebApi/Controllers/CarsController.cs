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
    [Route("api/Cars")]
    public class CarsController : Controller
    {
        private readonly IParking _parking;
        public CarsController(IParking parking)
        {
            _parking = parking;
        }
        // GET: api/Cars
        [HttpGet]
        public IEnumerable<ICar> Get()
        {

            return _parking.Cars;
        }

        // GET: api/Cars/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Cars
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Cars/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
