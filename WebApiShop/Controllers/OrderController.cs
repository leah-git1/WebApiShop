using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderMoreInfoDTO>> GetOrderById(int id)
        {
            return await _orderService.getOrderById(id);
        }

        // POST api/<OrderController>
        [HttpPost]
        public async Task<ActionResult<OrderDTO>> AddOrder([FromBody] CreateOrderDTO order)
        {
            OrderDTO postOrder = await _orderService.AddOrder(order);
            if (postOrder == null)
                return BadRequest();
            return CreatedAtAction(nameof(GetOrderById), new { id = postOrder.OrderId }, postOrder);
        }
    }
}
