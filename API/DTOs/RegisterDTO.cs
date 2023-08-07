

using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Username{set;get;}
         [Required]
        public string Password{set;get;}
    }
}