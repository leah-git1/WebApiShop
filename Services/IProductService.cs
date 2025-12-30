using DTOs;
using Entities;

namespace Services
{
    public interface IProductService
    {
        Task<PageResponseDTO<LessInfoProductDTO>> getProducts(int?[] categoryIds, int? min_price, int? max_price, int position, int skip);
    }
}