
namespace API.Helpers
{
    public class UserParams:PaginationParams
    {
      
        public string CurrentUsername{set;get;}
        public string Gender{set;get;}
        
        public int MinAge{set;get;}=18;

        public int MaxAge{set;get;}=100;

        
        public string OrderBy{set;get;}="lastActive";

    }
}