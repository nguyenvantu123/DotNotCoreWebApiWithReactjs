using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using DotNetCoreWithReactjs.Api.Models;
using Microsoft.AspNetCore.Authorization;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;
using DotNetCoreWithReactjs.Api.Service;
using DotNetCoreWithReactjs.Api.Service.Interfaces;
using DotNetCoreWithReactjs.Api.Helper;
using AutoMapper;
using DotNetCoreWithReactjs.Api.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using DotNetCoreWithReactjs.Api.Entity;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotNetCoreWithReactjs.Api.Controllers
{
    [Route("api")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    //[Authorize]
    public partial class HomeController : Controller
    {
        private IUserService _userService;
        private readonly AppSetting _appSettings;
        private IMapper _mapper;
        private DatabaseContext _databaseContext;

        public HomeController(IUserService userService, IOptions<AppSetting> appSetting, IMapper mapper, DatabaseContext databaseContext)
        {
            _userService = userService;
            _appSettings = appSetting.Value;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate(UserDto userDto)
        {
            var user = _userService.Authenticate(userDto.Email, userDto.Password);
            if (user == null)
            {
                return BadRequest("User name or password is incorrect!!!");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name,user.Guid.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Guid = user.Guid,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserDto userDto)
        {
            var user = _mapper.Map<UserEntity>(userDto);
            using (var transaction = _databaseContext.Database.BeginTransaction())
            {
                try
                {
                    UserEntity userEntity = _userService.CreateUser(user, userDto.Password);
                    transaction.Commit();
                    return Ok(userEntity);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message.ToString());
                }
            }
        }

        [HttpGet]
        [Route("getbyguid")]
        public IActionResult GetById(Guid guid)
        {
            var user = _userService.GetUserByGuid(guid);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Update(UserDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<UserEntity>(userDto);
            using (var transaction = _databaseContext.Database.BeginTransaction())
            {
                try
                {
                    // save 
                    UserEntity userUpdate = _userService.Update(user, userDto.Password);
                    transaction.Commit();
                    return Ok(userUpdate);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // return error message if there was an exception
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
