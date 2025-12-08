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
        public async Task<List<ProductTbl>> getProducts(int[]? category_id, int? min_price, int? max_price, int? limit, int? page)
        {
            return await _iProductService.getProducts(category_id, min_price, max_price, limit, page);
        }
    }
}