using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public async Task<PageResponseDTO<LessInfoProductDTO>> GetProducts([FromQuery] int?[] categoryIds, int? min_price, int? max_price, int position, int skip)
        {
            return await _productService.getProducts(categoryIds, min_price, max_price, position, skip);
        }

        [HttpGet("most-ordered")]
        public async Task<ActionResult<List<MoreInfoProductDTO>>> GetMostOrderedProducts([FromQuery] int count = 5)
        {
            try
            {
                List<MoreInfoProductDTO> products = await _productService.GetMostOrderedProducts(count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}