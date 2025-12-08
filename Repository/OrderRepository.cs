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
        ShopContext _ShopContext;
        public OrderRepository(ShopContext ShopContext)
        {
            this._ShopContext = ShopContext;
        }

        public async Task<OrdersTbl> getOrderById(int ind)
        {
            return await _ShopContext.OrdersTbls.FirstOrDefaultAsync(x => x.OrderId == ind);
        }

        public async Task<OrdersTbl> Invite(OrdersTbl order)
        {
            await _ShopContext.OrdersTbls.AddAsync(order);
            await _ShopContext.SaveChangesAsync();
            return await _ShopContext.OrdersTbls.FindAsync(order.OrderId);
        }
    }
}
