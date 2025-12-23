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

        public async Task<MoreInfoOrderDTO> getOrderById(int id)
        {
            OrdersTbl order = await _iOrderRepository.getOrderById(id);
            MoreInfoOrderDTO orderDTO = _mapper.Map<OrdersTbl, MoreInfoOrderDTO>(order);
            return orderDTO;
        }

        public async Task<LessInfoOrderDTO> AddOrder(OrdersTbl order)
        {
            OrdersTbl orderTbl = await _iOrderRepository.AddOrder(order);
            LessInfoOrderDTO orderDTO = _mapper.Map<OrdersTbl, LessInfoOrderDTO>(orderTbl);
            return orderDTO;
        }
    }
}
