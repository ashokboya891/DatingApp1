using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]  //apiusers
    public class UsersController:BaseApiController
    {
        private readonly IUserRepository  _userRepository;
        private  readonly IMapper _Mapper;
        public UsersController(IUserRepository userRepository,IMapper mapper)
        {
            _Mapper = mapper;
            _userRepository=userRepository;

        }
       
        [HttpGet]
        public async  Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
           var users=await _userRepository.GetMembersAsync();
        //    var userToReturn=_Mapper.Map<IEnumerable<MemberDto>>(users);

           return Ok(users);
          
        }
         [HttpGet("{username}")]
        public async  Task<ActionResult<MemberDto>> GetUser(string username)
        {
           return await _userRepository.GetMemberAsync(username);
            // return _Mapper.Map<MemberDto>(user);
             
        }
      
    }
}