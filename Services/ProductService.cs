using AutoMapper;
using DTOs;
using Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductService
    {
        IProductRepository _iProductRepository;
        IMapper _mapper;

        public ProductService(IProductRepository iProductRepository, IMapper mapper)
        {
            this._iProductRepository = iProductRepository;
            _mapper = mapper;
        }

        public async Task<PageResponseDTO<LessInfoProductDTO>> getProducts(int?[] categoryIds, int? min_price, int? max_price, int position, int skip)
        {
            (List<ProductTbl>, int) response = await _iProductRepository.getProducts(categoryIds, min_price, max_price, position, skip);
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
    }
}
