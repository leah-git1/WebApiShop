using AutoMapper;
using DTOs;
using Entities;
using Repository;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<OrderMoreInfoDTO> getOrderById(int id)
        {
            OrdersTbl order = await _orderRepository.getOrderById(id);
            OrderMoreInfoDTO orderDTO = _mapper.Map<OrdersTbl, OrderMoreInfoDTO>(order);
            return orderDTO;
        }

        public async Task<OrderDTO> AddOrder(CreateOrderDTO createOrder)
        {
            double? sum = 0;
            foreach (var item in createOrder.OrderItems)
            {
                sum += item.ProductPrice * item.Quantity;
            }
            OrdersTbl order = _mapper.Map<CreateOrderDTO, OrdersTbl>(createOrder);
            order.OrderSum = sum;
            OrdersTbl orderTbl = await _orderRepository.AddOrder(order);
            OrderDTO orderDTO = _mapper.Map<OrdersTbl, OrderDTO>(orderTbl);
            return orderDTO;
        }
    }
}
