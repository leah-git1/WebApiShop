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

        public async Task<List<LessInfoProductDTO>> getProducts(int[]? category_id, int? min_price, int? max_price, int? limit, int? page)
        {
            List<ProductTbl> products = await _iProductRepository.getProducts(category_id, min_price, max_price, limit, page);
            List<LessInfoProductDTO> productsDTOs = _mapper.Map<List<ProductTbl>, List<LessInfoProductDTO>>(products);
            return productsDTOs;
        }
    }
}
