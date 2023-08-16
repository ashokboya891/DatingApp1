

namespace API.DTOs
{
    public class MemberDto
    {
             public int Id{set;get;}
    //    [Required]  if dont want to take null values you can make it required filed and add mirgation
        public string UserName{set;get;}

        public string PhotoUrl{set;get;}

        // public byte[] PasswordHash{set;get;}

        // public byte[] PasswordSalt{set;get;}  

        public int Age{set;get;}

        public string KnownAs  {set;get;}

        public DateTime Created{set;get;}

        public DateTime LastActive {set;get;}

        public string Gender{set;get;}
        public string Introduction{set;get;}
        public string LookingFor{set;get;}
         public string Interests{set;get;}
         public string City{set;get;}
         public string Country{set;get;}

        public List<PhotoDto> Photos{set;get;}=new();
        
    }

    
}