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
            CreateMap<OrdersTbl, LessInfoOrderDTO>();
            CreateMap<OrdersTbl, MoreInfoOrderDTO>();
            CreateMap<User, LessInfoUserDTO>();
            CreateMap<User, MoreInfoUserDTO>();
        }
    }
}
