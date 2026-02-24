using AutoMapper;
using DTOs;
using Entities;
using Repository;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PageResponseDTO<LessInfoProductDTO>> getProducts(int?[] categoryIds, int? min_price, int? max_price, int position, int skip)
        {
            (List<ProductTbl>, int) response = await _productRepository.getProducts(categoryIds, min_price, max_price, position, skip);
            List<LessInfoProductDTO> data = _mapper.Map<List<ProductTbl>, List<LessInfoProductDTO>>(response.Item1);
            PageResponseDTO<LessInfoProductDTO> pageResponse = new();
            pageResponse.Data = data;
            pageResponse.TotalItems = response.Item2;
            pageResponse.CurrentPage = position;
            pageResponse.PageSize = skip;
            pageResponse.HasPreviousPage = position > 1;
            int numOfPages = pageResponse.TotalItems / skip;
            if (pageResponse.TotalItems % skip != 0)
                numOfPages++;
            pageResponse.HasNextPage = position < numOfPages;
            return pageResponse;
        }

        public async Task<List<MoreInfoProductDTO>> GetMostOrderedProducts(int count = 5)
        {
            List<ProductTbl> mostOrdered = await _productRepository.GetMostOrderedProducts(count);
            return _mapper.Map<List<ProductTbl>, List<MoreInfoProductDTO>>(mostOrdered);
        }
    }
}
