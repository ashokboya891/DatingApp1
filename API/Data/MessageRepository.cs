
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        public DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
           return  await _context.Messages.FindAsync(id);
        }


        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query=_context.Messages
            .OrderByDescending(x=>x.MessageSent)
            .AsQueryable();
            query=messageParams.Container switch
            {
                "Inbox"=>query.Where(x=>x.RecipientUsername==messageParams.Username && 
                    x.RecipientDeleted==false),
                "Outbox"=>query.Where(x=>x.SenderUsername==messageParams.Username && 
                x.SenderDeleted==false),
                _=>query.Where(u=>u.RecipientUsername==messageParams.Username 
                 && u.RecipientDeleted==false
                &&  u.DateRead==null)

            };
            var messages=query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>
            .CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName,string recipienUserName)
        {

            //this below also works it collected from comment section
            // var messages = await _context.Messages
            //     .Where(m => m.Recipient.UserName == currentUserName && m.RecipientDeleted == false
            //             && m.Sender.UserName == recipienUserName
            //             || m.Recipient.UserName == recipienUserName
            //             && m.Sender.UserName == currentUserName && m.SenderDeleted == false
            //     )
            //     .MarkUnreadAsRead(currentUserName)
            //     .OrderBy(m => m.MessageSent)
            //     .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            //     .ToListAsync();
 
            // return messages;
          var query= _context.Messages
          //commented after complenting unitofwork 243
            //   .Include(u=>u.Sender).ThenInclude(u=>u.Photos)
            //   .Include(u=>u.Recipient).ThenInclude(p=>p.Photos)
              .Where(
                m=>m.RecipientUsername==currentUserName  && m.RecipientDeleted==false 
                && m.SenderUsername==recipienUserName ||
                m.RecipientUsername==recipienUserName  && m.SenderDeleted==false
                && m.SenderUsername==currentUserName
              )
              .OrderBy(messages=>messages.MessageSent)
              .AsQueryable();
            //   .ToListAsync();  //commented after complenting unitofwork 243  by converintg tolist to asqurable above also before =contex and
            // query we using under this line code
              var unreadMessages=query.Where(m=>m.DateRead==null
               && m.RecipientUsername==currentUserName).ToList();
               if(unreadMessages.Any())
               {
                foreach(var message in unreadMessages)
                {
                        message.DateRead=DateTime.UtcNow;
                }
                // await _context.SaveChangesAsync();  it removing after unitofwork
               }


            //    return _mapper.Map<IEnumerable<MessageDto>>(messages);  after adding query changing this line to below line
            return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();

        }

        // public  async Task<bool> SaveAllAsync()
        // {
        //     return await _context.SaveChangesAsync()>0;
        // }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public   void RemoveConnection(Connection connection)
        {
            _context.connections.Remove(connection);
        }

        public  async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.connections.FindAsync(connectionId);
        }

        public async  Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(x=>x.Connections).FirstOrDefaultAsync(x=>x.Name==groupName);
        }

        public async  Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
            .Include(x=>x.Connections)
            .Where(x=>x.Connections.Any(x=>x.ConnectionId==connectionId))
            .FirstOrDefaultAsync();
        }
    }
}