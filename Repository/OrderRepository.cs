using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShopContext _shopContext;
        public OrderRepository(ShopContext shopContext)
        {
            this._shopContext = shopContext;
        }

        public async Task<OrdersTbl> getOrderById(int ind)
        {
            return await _ShopContext.OrdersTbls.FirstOrDefaultAsync(x => x.OrderId == ind);
        }

        public async Task<OrdersTbl> AddOrder(OrdersTbl order)
        {
            await _ShopContext.OrdersTbls.AddAsync(order);
            await _ShopContext.SaveChangesAsync();
            return order;
        }
    }
}
