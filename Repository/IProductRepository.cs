using Entities;

namespace Repository
{
    public interface IProductRepository
    {
        Task<(List<ProductTbl>, int total)> getProducts(int?[] categoryIds, int? minPrice, int? maxPrice, int position, int skip);
    }
}