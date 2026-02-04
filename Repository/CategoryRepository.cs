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
        private readonly ShopContext _shopContext;
        public CategoryRepository(ShopContext shopContext)
        {
            this._shopContext = shopContext;
        }

        public async Task<List<CategoriesTbl>> getCategories()
        {
            return await _ShopContext.CategoriesTbls.ToListAsync();
        }
    }
}
