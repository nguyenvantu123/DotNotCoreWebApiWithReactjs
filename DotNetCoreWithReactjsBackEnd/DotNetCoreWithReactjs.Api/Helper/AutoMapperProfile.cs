using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCoreWithReactjs.Api.Dto;
using DotNetCoreWithReactjs.Api.Entity;
using DotNetCoreWithReactjs.Api.Models;

namespace DotNetCoreWithReactjs.Api.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserEntity, UserDto>();
            CreateMap<UserDto, UserEntity>();
        }
    }
}
