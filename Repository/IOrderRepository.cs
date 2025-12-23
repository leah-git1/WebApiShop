using Entities;

namespace Repository
{
    public interface IOrderRepository
    {
        Task<OrdersTbl> getOrderById(int ind);
        Task<OrdersTbl> AddOrder(OrdersTbl order);
    }
}