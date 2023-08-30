using System.Runtime.InteropServices;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository,IMapper mapper,IPhotoService photoService)
        {
            _photoService = photoService;
            _Mapper = mapper;
            _userRepository=userRepository;

        }
        // [Authorize(Roles ="Admin")]
    //    [Authorize(Roles ="Admin")]
        [HttpGet]
        public async  Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUser=await _userRepository.GetUserByUserNameAsync(User.GetUsername());
           userParams.CurrentUsername=currentUser?.UserName;
            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender=currentUser.Gender=="male"?"female":"male";

            }
            
           var users=await _userRepository.GetMembersAsync(userParams);
    
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,
            users.PageSize,users.TotalCount,users.TotalPages));
           return Ok(users);
          
        }
        // [Authorize(Roles="Member")]
         [HttpGet("{username}")]
        public async  Task<ActionResult<MemberDto>> GetUser(string username)
        {
           return await _userRepository.GetMemberAsync(username);
            // return _Mapper.Map<MemberDto>(user);
             
        }
        [HttpPut]
        public async Task<ActionResult>UpdateUser(MemberUpdateDto memberUpdateDto)
        {
                // var username=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  --this line removed and added inside cliamsprincipal extension
                var user=await _userRepository.GetUserByUserNameAsync(User.GetUsername());
                if(user==null)return NotFound();
                _Mapper.Map(memberUpdateDto,user);
                if(await _userRepository.SaveAllAsync())return NoContent();
                return BadRequest("Failed to update user");
        }
         [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            if(user==null)
            {
                return NotFound();
            }
            var result=await _photoService.AddPhotoAsync(file);
            if(result.Error!=null)return BadRequest(result.Error.Message);
            var photo=new Photo
            {
                Url=result.SecureUrl.AbsoluteUri,
                PublicId=result.PublicId
            };
            if(user.Photos.Count==0)photo.IsMain=true;
            user.Photos.Add(photo);
            if(await _userRepository.SaveAllAsync()) 
            {
                return CreatedAtAction(nameof(GetUser),new {username=user.UserName}, _Mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("problem adding photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user=await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            if(user==null) return NotFound();
            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if(photo==null)NotFound();
            if(photo.IsMain)return BadRequest("this is already you main photo");
            var currentMain=user.Photos.FirstOrDefault(x=>x.IsMain);
            if(currentMain!=null) currentMain.IsMain=false;
            photo.IsMain=true;
            if(await _userRepository.SaveAllAsync())    return NoContent();
            return BadRequest("problem setting the main photo");
        }
       
    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user= await _userRepository.GetUserByUserNameAsync(User.GetUsername());

        var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
        if(photo==null)return NotFound();
        if(photo.IsMain)return BadRequest("you can not delete your main photo");
        if(photo.PublicId!=null)
        {
            var result=await _photoService.DeletePhotoAsync(photo.PublicId)  ;
            if(result.Error!=null)return BadRequest(result.Error.Message);


        }
        user.Photos.Remove(photo);
        if(await _userRepository.SaveAllAsync())return Ok();
        return BadRequest("problem deleting photo");
    }
    }
}