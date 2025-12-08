using Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductRepository, IProductService
    {
        IProductRepository _iProductRepository;

        public ProductService(IProductRepository iProductRepository)
        {
            this._iProductRepository = iProductRepository;
        }

        public async Task<List<ProductTbl>> getProducts(int[]? category_id, int? min_price, int? max_price, int? limit, int? page)
        {
            return await _iProductRepository.getProducts(category_id, min_price, max_price, limit, page);
        }
    }
}
