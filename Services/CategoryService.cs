using Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryRepository, ICategoryService
    {
        ICategoryRepository _iCategoryRepository;

        public CategoryService(ICategoryRepository iCategoryRepository)
        {
            this._iCategoryRepository = iCategoryRepository;
        }

        public async Task<List<CategoriesTbl>> getCategories()
        {
            return await _iCategoryRepository.getCategories();
        }
    }
}
