

namespace API.SignalR
{
    public class PresenceTracker
    {
       private static readonly Dictionary<string,List<string>>  OnlineUsers=
       new Dictionary<string,List<string>>();

       public Task<bool> UserConnected(string username,string connectionId)
       {

        bool isOnline=false;
        //dictionary is not a thared safe type of object so we are using lock only online connetced users will get locked 
        lock(OnlineUsers)
        {
            if(OnlineUsers.ContainsKey(username)) 
            {
                OnlineUsers[username].Add(connectionId);
            }
            else{
                  OnlineUsers.Add(username,new List<string>{connectionId});
                  isOnline=true;
            }
        }
         return Task.FromResult(isOnline);
       }
        public Task<bool> UserDisconnected(string username,string connectionId)
        {
                    bool isOffline=false;

            lock (OnlineUsers)
            {
                if(!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                OnlineUsers[username].Remove(connectionId);
                if(OnlineUsers[username].Count==0)
                {
                    OnlineUsers.Remove(username);
                    isOffline=true;
                }

               return Task.FromResult(isOffline); 
            }
            
        }


        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers)
            {
                onlineUsers=OnlineUsers.OrderBy(k=>k.Key).Select(k=>k.Key).ToArray();

            }
            return Task.FromResult(onlineUsers);
        }
        public static Task<List<string>> GetConnectionForUser(string username)
        {
            List<string> connectionIds;
            lock(OnlineUsers)
            {
                connectionIds=OnlineUsers.GetValueOrDefault(username);

            }
            return Task.FromResult(connectionIds);
        }
        public Task<string[]> GetConnectionsForUser(string username)
        {
            string[] connectionIds;
            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username)?.ToArray();
            }
            return Task.FromResult(connectionIds);
        }
    }
    
}