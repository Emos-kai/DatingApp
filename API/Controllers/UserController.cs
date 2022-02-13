using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;
         private readonly ILogger<UserController> _logger;

        public UserController(DataContext dataContext, ILogger<UserController> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            try{
                return Ok(await _dataContext.Users.ToListAsync());
            }
            catch(Exception ex){
                _logger.LogInformation(ex.Message);
                return BadRequest("Somethng go wrong");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserById(int id){
            try{
                return Ok(await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id));
            }
            catch(Exception ex){
                _logger.LogInformation(ex.Message);
                return BadRequest("Somethng go wrong");
            }
        }
    }
}