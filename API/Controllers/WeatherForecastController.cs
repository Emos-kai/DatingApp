using API.Entities;
using Microsoft.AspNetCore.Mvc;
using API.Data;

namespace API.Controllers{

    [ApiController]
    [Route("v1/User")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DataContext _dataContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        [HttpGet("Username")]
        public ActionResult<AppUser> GetUserName()
        {
            try{
                return Ok(_dataContext.Users.Select(u => u));
            }
            catch(Exception ex){
                _logger.LogInformation(ex.Message);
                return BadRequest("Somethng go wrong");
            }
        }
    }
}


