using DTOs;
using Entities;

namespace Services
{
    public interface IOrderService
    {
        Task<MoreInfoOrderDTO> getOrderById(int id);
        Task<LessInfoOrderDTO> AddOrder(OrdersTbl order);
    }
}