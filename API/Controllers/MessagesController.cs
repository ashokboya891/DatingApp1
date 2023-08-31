
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController:BaseApiController
    {
        public readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public readonly IUserRepository _userRepository;
        public MessagesController(IUserRepository userRepository,IMessageRepository messageRepository,IMapper mapper)
        {
           _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;

        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username=User.GetUsername();
            if(username==createMessageDto.RecipientUsername.ToLower())
            return BadRequest("you cannot send messages to yourself");
            var sender=await _userRepository.GetUserByUserNameAsync(username);
            var recipient=await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
            if(recipient==null)return NotFound();
            var message=new Message
            {
                Sender=sender,
                Recipient=recipient,
                SenderUsername=sender.UserName,
                // senderPhotoUrl=sender.PhotoUrl,
                RecipientUsername=recipient.UserName,
                Content=createMessageDto.Content

            };
            _messageRepository.AddMessage(message);
            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("failed to send to message");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username=User.GetUsername();
            var message=await _messageRepository.GetMessageForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(message.CurrentPage,
            message.PageSize,message.TotalCount,message.TotalPages));
            return message;

        }
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentusername=User.GetUsername();
            return Ok(await _messageRepository.GetMessageThread(currentusername,username));


        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username=User.GetUsername();
            var message=await _messageRepository.GetMessage(id);
            if(message.SenderUsername!=username && message.RecipientUsername!=username)
            return Unauthorized();
            if(message.SenderUsername==username) message.SenderDeleted=true;
            if(message.RecipientUsername==username) message.RecipientDeleted=true;
            if(message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }
            if(await _messageRepository.SaveAllAsync())return Ok();
            return BadRequest("problem deleting the messages");


        }
    }
}