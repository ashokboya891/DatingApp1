
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRespository
    {
        Task<UserLike> GetUserLike(int sourceUserId,int targetUserId);
        Task<AppUser> GetUserWithLike(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);



    }
}