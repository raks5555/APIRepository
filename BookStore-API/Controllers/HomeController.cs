using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers
{
    /// <summary>
    /// This is an API Controller
    /// </summary>
    /// 
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly iLoggerService _logger;

        public HomeController(iLoggerService Logger)
        {
            _logger = Logger;
        }

        /// <summary>
        /// Inside HomeController
        /// </summary>
        /// <returns></returns>
        ///


        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Accessed Home Controller");

            return new string[] { "value1", "value2" };
        }
        /// <summary>
        /// After GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            _logger.LogDebug("Got a value");

            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _logger.LogError("This is an Error");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.LogWarn("This is a warning");
        }
    }
}
