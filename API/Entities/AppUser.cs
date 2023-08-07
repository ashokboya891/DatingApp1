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
    //    [Required]  if dont want to take null values you can make it required filed and add mirgation
        public string UserName{set;get;}

        public byte[] PasswordHash{set;get;}

        public byte[] PasswordSalt{set;get;}    
    
    }
}