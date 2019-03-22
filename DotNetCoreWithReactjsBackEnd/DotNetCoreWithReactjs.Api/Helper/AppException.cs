using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreWithReactjs.Api.Helper
{
    public class AppException : Exception
    {
        public AppException() : base()
        {

        }

        public AppException(string message) : base(message)
        {

        }

        public AppException(string message, object[] args) : base(string.Format(CultureInfo.CurrentUICulture, message, args))
        {

        }
    }
}
