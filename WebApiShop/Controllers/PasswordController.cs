using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordService _passwordService;
        
        public PasswordController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }
       
        // POST api/<PasswordController>
        [HttpPost]
        public ActionResult<CheckPassword> Post([FromBody] string password)
        {
            return Ok(_passwordService.CheckStrengthPassword(password));
        }
    }
}
