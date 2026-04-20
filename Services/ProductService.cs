using AutoMapper;
using DTOs;
using Entities;
using Repository;
using StackExchange.Redis; 
using System.Text.Json;    
using Microsoft.Extensions.Configuration; 

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IProductRepository _iProductRepository;
        private readonly IDatabase _cache; 
        private readonly int _ttlSeconds;  

        public ProductService(IProductRepository iProductRepository, IMapper mapper, IConnectionMultiplexer redis, IConfiguration config)
        {
            _iProductRepository = iProductRepository;
            _mapper = mapper;

            _cache = redis.GetDatabase();

            _ttlSeconds = config.GetValue<int>("RedisSettings:DefaultTTLInSeconds");
        }

        public async Task<PageResponseDTO<LessInfoProductDTO>> getProducts(int?[] categoryIds, int? min_price, int? max_price, int position, int skip)
        {
            (List<ProductTbl>, int) response = await _productRepository.getProducts(categoryIds, min_price, max_price, position, skip);
            string categoryKey = categoryIds != null ? string.Join(",", categoryIds) : "all";
            string cacheKey = $"products:page:{position}:skip:{skip}:cats:{categoryKey}:min:{min_price}:max:{max_price}";

            var cachedData = await _cache.StringGetAsync(cacheKey);
            if (cachedData.HasValue)
            {
                return JsonSerializer.Deserialize<PageResponseDTO<LessInfoProductDTO>>(cachedData);
            }

           
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

            await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(pageResponse), TimeSpan.FromSeconds(_ttlSeconds));

            return pageResponse;
        }

        public async Task<List<MoreInfoProductDTO>> GetMostOrderedProducts(int count = 5)
        {
            
            string cacheKey = $"products:most_ordered:count:{count}";

            var cachedData = await _cache.StringGetAsync(cacheKey);
            if (cachedData.HasValue)
            {
                return JsonSerializer.Deserialize<List<MoreInfoProductDTO>>(cachedData);
            }

            List<ProductTbl> mostOrdered = await _iProductRepository.GetMostOrderedProducts(count);
            var result = _mapper.Map<List<ProductTbl>, List<MoreInfoProductDTO>>(mostOrdered);

            await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(result), TimeSpan.FromSeconds(_ttlSeconds));

            return result;
        }
    }
}