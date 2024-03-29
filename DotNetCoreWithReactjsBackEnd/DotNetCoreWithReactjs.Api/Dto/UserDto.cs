﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreWithReactjs.Api.Dto
{
    public class UserDto
    {
        public Guid Guid { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
