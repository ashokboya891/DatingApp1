
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
                //this filed removed after adding untiofwork and commentin saveall method 

        // public readonly IMapper _mapper;
        // private readonly IMessageRepository _unitOfWork.messageRepository;
        // public readonly IUserRepository _unitOfWork.userRepository;
        // public MessagesController(IUserRepository userRepository,IMessageRepository messageRepository,IMapper mapper)
        // {
        //    _unitOfWork.userRepository = userRepository;
        //     _unitOfWork.messageRepository = messageRepository;
        //     _mapper = mapper;

        // }
        private readonly IMapper _mapper;
        public  readonly IUnitOfWork _unitOfWork;

        public MessagesController(IMapper mapper,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username=User.GetUsername();
            if(username==createMessageDto.RecipientUsername.ToLower())
            return BadRequest("you cannot send messages to yourself");
            var sender=await _unitOfWork.userRepository.GetUserByUserNameAsync(username);
            var recipient=await _unitOfWork.userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
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
            _unitOfWork.messageRepository.AddMessage(message);
            if(await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("failed to send to message");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username=User.GetUsername();
            var message=await _unitOfWork.messageRepository.GetMessageForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(message.CurrentPage,
            message.PageSize,message.TotalCount,message.TotalPages));
            return message;

        }

        //removed here after adding unitof work 242 8 and after this adding extra code
        // inside messagehub=> onconnectedasync  
        // [HttpGet("thread/{username}")]
        // public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        // {
        //     var currentusername=User.GetUsername();
        //     return Ok(await _unitOfWork.messageRepository.GetMessageThread(currentusername,username));


        // }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username=User.GetUsername();
            var message=await _unitOfWork.messageRepository.GetMessage(id);
            if(message.SenderUsername!=username && message.RecipientUsername!=username)
            return Unauthorized();
            if(message.SenderUsername==username) message.SenderDeleted=true;
            if(message.RecipientUsername==username) message.RecipientDeleted=true;
            if(message.SenderDeleted && message.RecipientDeleted)
            {
                _unitOfWork.messageRepository.DeleteMessage(message);
            }
            if(await _unitOfWork.Complete())return Ok();
            return BadRequest("problem deleting the messages");


        }
    }
}