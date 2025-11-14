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
        UsersServices service = new UsersServices();
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
            User user = service.getUserById(ind);
            if(user == null)            
                return NoContent();
            return Ok(user);
        }

        
        // POST api/<UserController>
        [HttpPost]
        public ActionResult<User> Post([FromBody] User user)
        {
            User postUser = service.registerUser(user);
            if (postUser == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = postUser.userId }, postUser);
        }

        [HttpPost("login")]
        public ActionResult<User> Post([FromBody] UserLog userToLog)
        {
            User user = service.loginUser(userToLog);
            if (user == null)
                return NoContent();
            return CreatedAtAction(nameof(Get), new { id = user.userId }, user);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put([FromBody] User userToUpdate, int id)
        {
            service.updateUser(userToUpdate,id);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
