using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryServices;

        public CategoryController(ICategoryService categoryServices)
        {
            _categoryServices = categoryServices;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<List<CategoriesTbl>> GetCategories()
        {
            return await _categoryServices.getCategories();
        }
    }
}
