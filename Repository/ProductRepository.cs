using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProductRepository : IProductRepository
    {
        ShopContext _ShopContext;
        public ProductRepository(ShopContext ShopContext)
        {
            this._ShopContext = ShopContext;
        }

        public async Task<List<ProductTbl>> getProducts(int[]? category_id, int? min_price, int? max_price, int? limit, int? page)
        {
            return await _ShopContext.ProductTbls.ToListAsync();
        }
    }
}
