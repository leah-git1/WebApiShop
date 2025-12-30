using AutoMapper;
using DTOs;
using Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService :  IOrderService
    {
        IOrderRepository _iOrderRepository;
        IMapper _mapper;
        public OrderService(IOrderRepository iOrderRepository, IMapper mapper)
        {
            this._iOrderRepository = iOrderRepository;
            this._mapper = mapper;
        }

        public async Task<OrderMoreInfoDTO> getOrderById(int id)
        {
            OrdersTbl order = await _iOrderRepository.getOrderById(id);
            OrderMoreInfoDTO orderDTO = _mapper.Map<OrdersTbl, OrderMoreInfoDTO>(order);
            return orderDTO;
        }

        public async Task<OrderDTO> AddOrder(CreateOrderDTO createOrder)
        {
            double? sum = 0;
            foreach(var item in createOrder.OrderItems)
            {
                sum += item.ProductPrice * item.Quantity;
            }
            OrdersTbl order = _mapper.Map<CreateOrderDTO, OrdersTbl>(createOrder);
            order.OrderSum = sum;
            OrdersTbl orderTbl = await _iOrderRepository.AddOrder(order);
            OrderDTO orderDTO = _mapper.Map<OrdersTbl, OrderDTO>(orderTbl);
            return orderDTO;
        }
    }
}
