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
        IUsersServices _iUsersServices;
        IPasswordService _passwordService;
        public UsersController(IUsersServices iUsersServices, IPasswordService passwordService) { 
            _iUsersServices = iUsersServices;
            _passwordService = passwordService;
        }
        
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            
           return new string[] { "user1", "user2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public ActionResult<User> Get(int ind)
        {
            User user = _iUsersServices.getUserById(ind);
            if(user == null)            
                return NoContent();
            return Ok(user);
        }

        
        // POST api/<UserController>
        [HttpPost]
        public ActionResult<User> Post([FromBody] User user)
        {
            User postUser = _iUsersServices.registerUser(user);
            if (postUser == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = postUser.userId }, postUser);
        }

        [HttpPost("login")]
        public ActionResult<User> Post([FromBody] UserLog userToLog)
        {
            User user = _iUsersServices.loginUser(userToLog);
            if (user == null)
                return NoContent();
            return CreatedAtAction(nameof(Get), new { id = user.userId }, user);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public ActionResult<User> Put([FromBody] User userToUpdate, int id)
        {
            User user = _iUsersServices.updateUser(userToUpdate, id);
            if (user == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = user.userId }, user);
            
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
