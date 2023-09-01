
using API.Extensions;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using API.Helpers;

namespace API.Controllers
{
    public class LikesController:BaseApiController
    {
        //this filed removed after adding untiofwork and commentin saveall method 
        // private readonly IUserRepository _userRepository;
        // private readonly ILikesRespository _likesRespository;
        // public LikesController(IUserRepository userRepository,ILikesRespository likesRespository)
        // {
        //     _userRepository = userRepository;
        //     _likesRespository = likesRespository;
        public readonly IUnitOfWork _uow;

        // }
        public LikesController(IUnitOfWork uow)
        {
            _uow = uow;

        }
        [HttpPost("{username}")]
        public async Task<IActionResult> AddLike(string username)
        {
          
            var sourceUserId=User.GetUserId();
            var likedUser=await _uow.userRepository.GetUserByUserNameAsync(username);
            var SourceUser=await _uow.likesRespository.GetUserWithLike(sourceUserId);

            if(likedUser==null)return NotFound();
            if(SourceUser.UserName==username) return BadRequest("you cannot like yourself");
            var userLike=await _uow.likesRespository.GetUserLike(sourceUserId,likedUser.Id);
            if(userLike!=null) return BadRequest("you already liked this user");
            userLike=new UserLike{
                SourceUserId=sourceUserId,
                TargetUserId=likedUser.Id
            };
            SourceUser.LikedUsers.Add(userLike);
            if(await _uow.Complete())return Ok();
            return BadRequest("failed to like user");

        }

         [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId=User.GetUserId();
              var users=await _uow.likesRespository.GetUserLikes(likesParams);
              Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
            return Ok(users);

        }
       
        
    }
       
}