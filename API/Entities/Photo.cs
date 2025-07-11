

using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
      [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain{set;get;}

        public string  PublicId { get; set; }

        public bool IsApproved { get; set; } = false;

        //navigation property
        public int AppUserId{get;set;}
        public AppUser AppUser{set;get;}

    }
}