using Entities;

namespace Repository
{
    public interface IProductRepository
    {
        Task<List<ProductTbl>> getProducts(int[]? category_id, int? min_price, int? max_price, int? limit, int? page);
    }
}