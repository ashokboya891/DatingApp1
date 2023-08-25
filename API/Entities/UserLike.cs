

namespace API.Entities
{
    public class UserLike
    {

     public AppUser SourceUser{set;get;}

     public int SourceUserId{set;get;}
    public AppUser TargetUser{set;get;}
    public int TargetUserId{set;get;}

    }
}