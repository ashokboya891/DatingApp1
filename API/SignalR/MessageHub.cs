
using System.Security.AccessControl;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub:Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub; 
        public MessageHub(IMessageRepository messageRepository,
        IUserRepository userRepository,IMapper mapper,IHubContext<PresenceHub> presenceHub)
        {
            _presenceHub = presenceHub;
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;

        }
        public override async Task OnConnectedAsync()
        {
            var httpContex=Context.GetHttpContext();
            var otherUser=httpContex.Request.Query["user"];
            var groupName=GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
            var group=await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdateGroup",group);
            var messages=await _messageRepository.
            GetMessageThread(Context.User.GetUsername(),otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread",messages);
        }



        public override async Task OnDisconnectedAsync(Exception exception)
        {
             var group=await RemoveFromMessageGroup();
             await Clients.Group(group.Name).SendAsync("UpdateGroup");
            await base.OnDisconnectedAsync(exception);
            
        }

        
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username=Context.User.GetUsername();
            if(username==createMessageDto.RecipientUsername.ToLower())
            throw new HubException("you can not send message to your-self");
            var sender=await _userRepository.GetUserByUserNameAsync(username);
            var recipient=await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
            if(recipient==null) throw new HubException("Not found user");

            var message=new Message
            {
                Sender=sender,
                Recipient=recipient,
                SenderUsername=sender.UserName,
                // senderPhotoUrl=sender.PhotoUrl,
                RecipientUsername=recipient.UserName,
                Content=createMessageDto.Content

            };
            var groupName=GetGroupName(sender.UserName,recipient.UserName);
            var group=await _messageRepository.GetMessageGroup(groupName);
            if(group.Connections.Any(x=>x.UserName==recipient.UserName))
            {
                message.DateRead=DateTime.UtcNow;
            }
            else{
                var connection=await PresenceTracker.GetConnectionForUser(recipient.UserName);
                if(connection!=null)
                {
                    await _presenceHub.Clients.Clients(connection).SendAsync("NewMessageReceived",
                    new {username=sender.UserName,KnownAs=sender.KnownAs});
                }
            }
            _messageRepository.AddMessage(message);
            if(await _messageRepository.SaveAllAsync()){
                // var group=GetGroupName(sender.UserName,recipient.UserName);
                await Clients.Group(groupName).SendAsync("NewMessages",_mapper.Map<MessageDto>(message));
            }

        }

        private string GetGroupName(string caller,string other)
        {
            var stringCompare=string.CompareOrdinal(caller,other)<0;
            return stringCompare?$"{caller}-{other}":$"{other}-{caller}";


        }
        private async Task<Group> AddToGroup(string groupName)
        {
            var group=await _messageRepository.GetMessageGroup(groupName);
            var connection=new Connection(Context.ConnectionId,Context.User.GetUsername());
            if(group==null){
                group=new Group(groupName);
                _messageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
            if( await _messageRepository.SaveAllAsync()) return group;
            throw new HubException("failed to add to group");
        }
        private async Task<Group> RemoveFromMessageGroup()
        {
            var group=await _messageRepository.GetGroupForConnection(Context.ConnectionId); 
            var connection=group.Connections.FirstOrDefault(x=>x.ConnectionId==Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);    
           
           if( await _messageRepository.SaveAllAsync()) return group;
           throw new HubException("failed to remove from group");
        }

    }
}