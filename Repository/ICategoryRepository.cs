using Entities;

namespace Repository
{
    public interface ICategoryRepository
    {
        Task<List<CategoriesTbl>> getCategories();
    }
}