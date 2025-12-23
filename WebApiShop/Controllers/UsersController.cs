using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;
using NLog.Web;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUsersServices _iUsersServices;
        private readonly ILogger<UsersController> _logger;
        public UsersController(IUsersServices iUsersServices, ILogger<UsersController> logger) { 
            _iUsersServices = iUsersServices;
            _logger = logger;
        }
        

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int ind)
        {
            User user = await _iUsersServices.getUserById(ind);
            if(user == null)            
                return NotFound();
            return Ok(user);
        }

        
        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] User user)
        {
            User postUser = await _iUsersServices.registerUser(user);
            if (postUser == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = postUser.UserId }, postUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Post([FromBody] UserLog userToLog)
        {
            User user = await _iUsersServices.loginUser(userToLog);
            if (user == null)
            {
                _logger.LogInformation("User not exist");
                return NoContent();
            }
            _logger.LogInformation("User login successfully: Name: {FullName}, Email: {Email}", $"{user.FirstName} {user.LastName}", user.UserName);
            return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async  Task<ActionResult> Put([FromBody] User userToUpdate, int id)
        {
            User user = await _iUsersServices.updateUser(userToUpdate, id);
            if (user == null)
                return BadRequest("Password is not strong enough");
            return NoContent();

        }


    }
}
