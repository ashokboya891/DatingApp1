
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
          var messages=await _context.Messages
              .Include(u=>u.Sender).ThenInclude(u=>u.Photos)
              .Include(u=>u.Recipient).ThenInclude(p=>p.Photos)
              .Where(
                m=>m.RecipientUsername==currentUserName  && m.RecipientDeleted==false 
                && m.SenderUsername==recipienUserName ||
                m.RecipientUsername==recipienUserName  && m.SenderDeleted==false
                && m.SenderUsername==currentUserName
              )
              .OrderBy(messages=>messages.MessageSent)
              .ToListAsync();
              var unreadMessages=messages.Where(m=>m.DateRead==null
               && m.RecipientUsername==currentUserName).ToList();
               if(unreadMessages.Any())
               {
                foreach(var message in unreadMessages)
                {
                        message.DateRead=DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
               }
               return _mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public  async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync()>0;
        }
    }
}