
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
        private readonly IUserRepository _userRepository;
        private readonly ILikesRespository _likesRespository;
        public LikesController(IUserRepository userRepository,ILikesRespository likesRespository)
        {
            _userRepository = userRepository;
            _likesRespository = likesRespository;

        }
        [HttpPost("{username}")]
        public async Task<IActionResult> AddLike(string username)
        {
          
            var sourceUserId=User.GetUserId();
            var likedUser=await _userRepository.GetUserByUserNameAsync(username);
            var SourceUser=await _likesRespository.GetUserWithLike(sourceUserId);

            if(likedUser==null)return NotFound();
            if(SourceUser.UserName==username) return BadRequest("you cannot like yourself");
            var userLike=await _likesRespository.GetUserLike(sourceUserId,likedUser.Id);
            if(userLike!=null) return BadRequest("you already liked this user");
            userLike=new UserLike{
                SourceUserId=sourceUserId,
                TargetUserId=likedUser.Id
            };
            SourceUser.LikedUsers.Add(userLike);
            if(await _userRepository.SaveAllAsync())return Ok();
            return BadRequest("failed to like user");

        }

         [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId=User.GetUserId();
              var users=await _likesRespository.GetUserLikes(likesParams);
              Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
            return Ok(users);

        }
       
        
    }
       
}