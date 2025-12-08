using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IOrderService _iOrderService;
        public OrderController(IOrderService iOrderService)
        {
            _iOrderService = iOrderService;
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersTbl>> getOrderById(int id)
        {
            return await _iOrderService.getOrderById(id);
        }
        // POST api/<OrderController>
        [HttpPost]
        public async Task<ActionResult<OrdersTbl>> Invite([FromBody] OrdersTbl order)
        {
            OrdersTbl postOrder = await _iOrderService.Invite(order);
            if (postOrder == null)
                return BadRequest();
            return CreatedAtAction(nameof(getOrderById), new { id = postOrder.OrderId }, postOrder);
            //return await _iOrderService.Invite(order);
        }
    }
}
