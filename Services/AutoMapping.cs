using AutoMapper;
using DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AutoMapping:Profile
    {
        public AutoMapping() 
        {
            CreateMap<ProductTbl,LessInfoProductDTO>();
            CreateMap<ProductTbl, MoreInfoProductDTO>();
            CreateMap<OrdersTbl, OrderDTO>();
            CreateMap<OrdersTbl, OrderMoreInfoDTO>();
            CreateMap<OrdersTbl, CreateOrderDTO>();
            CreateMap<OrderDTO, OrdersTbl>();
            CreateMap<OrderMoreInfoDTO, OrdersTbl>();
            CreateMap<CreateOrderDTO, OrdersTbl>();
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<UserToRegisterDTO, User>();
        }
    }
}
