using System;
using System.Threading.Tasks;
using FailingWebApplication.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FailingWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private Random _random;
        
        [HttpGet]
        [Fail(Name = "Demo")]
        public async Task<IActionResult>  Get()
        {
            Console.WriteLine("A request is coming in...");
            await Task.Delay(_random.Next(5000));
            return Ok( "Success");
        }

        public ValuesController()
        {
            _random = new Random(DateTime.Now.Second);
        }
    }
}
