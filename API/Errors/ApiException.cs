using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiException
    {
        public ApiException(int dtatusCode, string message=null, string details=null)
        {
            StatusCode = dtatusCode;
            Message = message;
            Details = details;
        }

        public int? StatusCode{set;get;}
        public string Message{set;get;}
        public string Details{set;get;}
    }
}