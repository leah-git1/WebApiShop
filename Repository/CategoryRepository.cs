using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        ShopContext _ShopContext;
        public CategoryRepository(ShopContext ShopContext)
        {
            this._ShopContext = ShopContext;
        }

        public async Task<List<CategoriesTbl>> getCategories()
        {
            return await _ShopContext.CategoriesTbls.ToListAsync();
        }
    }
}
