using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUser
    {
        // [Key]P
        public int Id{set;get;}
        public string UserName{set;get;}
    
    }
}