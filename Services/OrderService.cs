using Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderRepository, IOrderService
    {
        IOrderRepository _iOrderRepository;
        public OrderService(IOrderRepository iOrderRepository)
        {
            this._iOrderRepository = iOrderRepository;
        }

        public async Task<OrdersTbl> getOrderById(int id)
        {
            return await _iOrderRepository.getOrderById(id);
        }

        public async Task<OrdersTbl> Invite(OrdersTbl order)
        {
            return await _iOrderRepository.Invite(order);
        }
    }
}
