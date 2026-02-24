using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersServices _usersServices;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersServices usersServices, ILogger<UsersController> logger)
        {
            _usersServices = usersServices;
            _logger = logger;
        }
        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            UserDTO user = await _usersServices.getUserById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post([FromBody] UserToRegisterDTO user)
        {
            UserDTO postUser = await _usersServices.registerUser(user);
            if (postUser == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = postUser.UserId }, postUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] UserLog userToLog)
        {
            UserDTO user = await _usersServices.loginUser(userToLog);
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
        public async Task<ActionResult> Put([FromBody] UserToRegisterDTO userToUpdate, int id)
        {
            UserDTO user = await _usersServices.updateUser(userToUpdate, id);
            if (user == null)
                return BadRequest("Password is not strong enough");
            return NoContent();
        }
    }
}
