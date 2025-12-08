using Entities;

namespace Services
{
    public interface IProductService
    {
        Task<List<ProductTbl>> getProducts(int[]? category_id, int? min_price, int? max_price, int? limit, int? page);
    }
}