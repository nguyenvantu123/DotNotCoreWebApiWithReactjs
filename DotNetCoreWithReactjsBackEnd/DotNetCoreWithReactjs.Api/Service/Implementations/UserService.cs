using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreWithReactjs.Api.Models;
using DotNetCoreWithReactjs.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Remotion.Linq.Clauses;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;
using System.Web.Http;
using DotNetCoreWithReactjs.Api.Dto;
using DotNetCoreWithReactjs.Api.Entity;
using DotNetCoreWithReactjs.Infrastructure.Helper;

namespace DotNetCoreWithReactjs.Api.Service.Implementations
{
    public class UserService : IUserService
    {

        private DatabaseContext _databaseContext;

        public UserService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public UserEntity Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty((password)))
            {
                return null;
            }

            var user = _databaseContext.Users.Where(x => x.Email == email).FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Incorrect user or password");

            // authentication successful
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (passwordHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (passwordSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }

            return true;
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public UserResult GetUserByGuid(Guid Guid)
        {
            UserResult user = (from u in _databaseContext.Set<UserEntity>()
                               where u.Guid == Guid
                               select new UserResult
                               {
                                   Guid = u.Guid,
                                   Email = u.Email,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   FullName = u.FullName,
                                   Password = ""
                               }).FirstOrDefault();
            return user;
        }

        public UserEntity Update([FromBody]UserEntity user, string password = null)
        {

            if (!user.ArePropertiesNotNull())
            {
                throw new Exception("Data cannot be blank");
            }

            if (user == null)
            {
                throw new Exception("Please insert data");
            }

            UserEntity userIsUpdate = (from u in _databaseContext.Set<UserEntity>()
                                       where u.Guid == user.Guid
                                       select u).FirstOrDefault();
            try
            {

                if (userIsUpdate == null)
                {
                    throw new Exception("Not have user with email" + user.Email);
                }

                userIsUpdate.Email = user.Email;
                userIsUpdate.FirstName = user.FirstName;
                userIsUpdate.LastName = user.LastName;
                userIsUpdate.FullName = user.FirstName + " " + user.LastName;

                if (!string.IsNullOrEmpty(password))
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash(password, out passwordHash, out passwordSalt);
                    userIsUpdate.PasswordHash = passwordHash;
                    userIsUpdate.PasswordSalt = passwordSalt;
                }

                _databaseContext.Entry(userIsUpdate).State = EntityState.Modified;
                _databaseContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return user;
        }

        public UserEntity CreateUser([FromBody] UserEntity user, string password)
        {
            if (user == null)
            {
                throw new Exception("Please insert data");
            }

            if (!user.ArePropertiesNotNull())
            {
                throw new Exception("Data cannot be blank");
            }

            if (_databaseContext.Users.Any(x => x.Email == user.Email))
            {
                throw new Exception("Email is already exists");
            }

            UserEntity newUser = new UserEntity();

            try
            {
                newUser.Guid = Guid.NewGuid();
                newUser.FirstName = user.FirstName;
                newUser.LastName = user.LastName;
                newUser.FullName = user.FirstName + " " + user.LastName;
                newUser.Email = user.Email;
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);
                newUser.PasswordHash = passwordHash;
                newUser.PasswordSalt = passwordSalt;
                _databaseContext.Entry(newUser).State = EntityState.Added;
                _databaseContext.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }

            return newUser;
        }
    }
}
