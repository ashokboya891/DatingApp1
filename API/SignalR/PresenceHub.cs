

using System;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub:Hub
    {
        private readonly PresenceTracker _tracker ;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;

        }
        public override async Task OnConnectedAsync()
        {
           var isOnline= await _tracker.UserConnected(Context.User.GetUsername(),Context.ConnectionId);
           if(isOnline)
            await Clients.Others.SendAsync("UserIsOnline",Context.User.GetUsername());

            var currentUsers = await _tracker.GetOnlineUsers();

            await Clients.Caller.SendAsync("GetOnlineUsers",currentUsers);
                //rather sending all active onliner all user we are sending active user list to caller user  who is in online  

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
           
            bool isOffline =  await  _tracker.UserDisconnected(Context.User.GetUsername(),Context.ConnectionId);
            if(isOffline)
            await Clients.Others.SendAsync("UserIsOffline",Context.User.GetUsername());
        
            await base.OnDisconnectedAsync(exception);
            

            // var currentUser =   await _tracker.GetOnlineUsers();

            // await Clients.All.SendAsync("GetOnlineUsers",currentUser);

        }

    }
}