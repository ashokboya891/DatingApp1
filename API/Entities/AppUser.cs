using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;

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

        public DateOnly DateOfBirth{set;get;}

        public string KnownAs  {set;get;}

        public DateTime Created{set;get;}=DateTime.UtcNow;

        public DateTime LastActive {set;get;}  =DateTime.UtcNow;

        public string Gender{set;get;}
        public string Introduction{set;get;}
        public string LookingFor{set;get;}
         public string Interests{set;get;}
         public string City{set;get;}
         public string Country{set;get;}

        public List<Photo> Photos{set;get;}=new();  //=new list<Photo> also usefull here

        public List<UserLike> LikedByUsers{set;get;} 

        public List<UserLike> LikedUsers{set;get;}





        // public int Getage()
        // {
        //     return DateOfBirth.CalculateAge();
        // }
         
    }

   
}