

namespace API.Helpers
{
    public class MessageParams:PaginationParams
    {
        public string Username{set;get;}
        public string Container{set;get;}="Unread";
        
    }
}