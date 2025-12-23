using DTOs;
using Entities;

namespace Services
{
    public interface IProductService
    {
        Task<List<LessInfoProductDTO>> getProducts(int[]? category_id, int? min_price, int? max_price, int? limit, int? page);
    }
}