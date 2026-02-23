using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IProductService _iProductService;
        public ProductController(IProductService iProductService)
        {
            _iProductService = iProductService;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<PageResponseDTO<LessInfoProductDTO>> GetProducts([FromQuery] int?[] categoryIds,int? min_price, int? max_price, int position, int skip)
        {
            return await _iProductService.getProducts(categoryIds, min_price, max_price, position, skip);
        }

        [HttpGet("most-ordered")]
        public async Task<ActionResult<List<MoreInfoProductDTO>>> GetMostOrderedProducts([FromQuery] int count = 5)
        {
            try
            {
                List<MoreInfoProductDTO> products = await _iProductService.GetMostOrderedProducts(count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}