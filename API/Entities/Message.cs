

namespace API.Entities
{
    public class Message
    {
        public int Id{set;get;}
        public int SenderId{set;get;}
        public string SenderUsername{get;set;}
        public AppUser Sender{set;get;}

// public string senderPhotoUrl{get;set;}  
        public int RecipientId{set;get;}
        public string RecipientUsername{get;set;}
        public AppUser Recipient{set;get;}
        public string Content{set;get;}
        public DateTime? DateRead{set;get;}
        public DateTime MessageSent{set;get;}=DateTime.UtcNow;
        public bool SenderDeleted{set;get;}
        public bool RecipientDeleted{set;get;}

    }
}