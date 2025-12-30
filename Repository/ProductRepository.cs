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

        public async Task<(List<ProductTbl>,int total)> getProducts(int?[] categoryIds, int? minPrice, int? maxPrice, int position=1, int skip=10)
        {
            var query = _ShopContext.ProductTbls.Where(product =>
            //(desc == null ? (true) : (product.Description.Contains(desc)))
            ((minPrice == null) ? (true) : (product.ProductPrice >= minPrice))
            && ((maxPrice == null) ? (true) : (product.ProductPrice <= maxPrice))
            && ((categoryIds.Length == 0) ? (true) : (categoryIds.Contains(product.CategoryId))))
            .OrderBy(product => product.ProductPrice);

            Console.WriteLine(query.ToQueryString());
            List<ProductTbl> products = await query.Skip(((position - 1) * skip))
            .Take(skip).Include(product => product.Category).ToListAsync();
            var total = await query.CountAsync();
            return (products, total);
            
        }
    }
}
