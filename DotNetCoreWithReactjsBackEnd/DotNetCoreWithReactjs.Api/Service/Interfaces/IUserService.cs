using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreWithReactjs.Api.Entity;
using DotNetCoreWithReactjs.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWithReactjs.Api.Service.Interfaces
{
    public interface IUserService
    {
        UserEntity Authenticate(string email, string password);

        UserResult GetUserByGuid(Guid Guid);

        UserEntity CreateUser([FromBody] UserEntity user, string password);
        UserEntity Update([FromBody] UserEntity user, string password);
    }
}
