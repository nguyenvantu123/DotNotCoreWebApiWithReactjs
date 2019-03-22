using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreWithReactjs.Api.Entity;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWithReactjs.Api.Models
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
        {
                
        }

        public  DbSet<UserEntity> Users { get; set; }

    }
}
