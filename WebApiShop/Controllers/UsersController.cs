using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersServices _usersServices;
        private readonly IPasswordService _passwordService;
        
        public UsersController(IUsersServices usersServices, IPasswordService passwordService) { 
            _usersServices = usersServices;
            _passwordService = passwordService;
        }
    
        

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            User user = _usersServices.GetUserById(id);
            if(user == null)            
                return NotFound();
            return Ok(user);
        }

        
        // POST api/<UserController>
        [HttpPost]
        public ActionResult<User> Post([FromBody] User user)
        {
            User postUser = _usersServices.RegisterUser(user);
            if (postUser == null)
                return BadRequest("Password is not strong enough");
            return CreatedAtAction(nameof(Get), new { id = postUser.UserId }, postUser);
        }

        [HttpPost("login")]
        public ActionResult<User> Login([FromBody] UserLog userToLog)
        {
            User user = _usersServices.LoginUser(userToLog);
            if (user == null)
                return Unauthorized();
            return Ok(user);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] User userToUpdate)
        {
            User user = _usersServices.UpdateUser(userToUpdate, id);
            if (user == null)
                return BadRequest("Password is not strong enough");
            return NoContent();
        }
    }
}
