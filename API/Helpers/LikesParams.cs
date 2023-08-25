using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class LikesParams:PaginationParams
    {
        public int UserId{set;get;}
        public string Predicate{set;get;}
        
    }
}