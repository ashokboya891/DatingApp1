

using API.Data;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController:BaseApiController
    {
        private  UserManager<AppUser> _userManager;
        private readonly IUnitOfWork unitOfWork; // Assuming you have a unit of work pattern implemented
        private readonly IPhotoService photoService; // Assuming you have a photo service for handling photo operations
        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.photoService = photoService;
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {

            var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if (photo == null) return BadRequest("Could not get photo from db");
            photo.IsApproved = true;
            var user = await unitOfWork.userRepository.GetUserByPhotoId(photoId);
            if (user == null) return BadRequest("Could not get user from db");
            if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;
            await unitOfWork.Complete();
            return Ok();
        }

            [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            
                var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();
                return Ok(photos);
        }

        //[Authorize(Policy = "ModeratePhotoRole")]
        //[HttpPost("approve-photo/{photoId}")]
        //public async Task<ActionResult> ApprovePhoto(int photoId)
        //{
        //    var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
        //    if (photo == null) return BadRequest("Could not get photo from db");
        //    photo.IsApproved = true;
        //    await unitOfWork.Complete();
        //    return Ok();
        //}

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if (photo == null) return BadRequest("Could not get photo from db");
            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Result == "ok")
                {
                    unitOfWork.PhotoRepository.RemovePhoto(photo);

                }
            }
            else
            {
            unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
            await unitOfWork.Complete();
            return Ok();
        }

       [Authorize(policy:"RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
             var users = await _userManager.Users
             .OrderBy(u => u.UserName)
             .Select(u => new
             {
                 u.Id,
                 Username = u.UserName,
                 Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
             })
             .ToListAsync();

        return Ok(users);
            // var users= await _userManager.Users.OrderBy(u=>u.UserName).Select(u=>new{
            //     u.Id,
            //     UserName=u.UserName,
            //     Roles=u.UserRoles.Select(r=>r.Role.Name).ToList()
            // }).ToList();
            // return Ok(users);
        }
    [Authorize(policy:"RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username,[FromQuery]string roles)
    {
        if(string.IsNullOrEmpty(roles))return BadRequest("you have to choose one");
        var SelectedRoles=roles.Split(',').ToArray();
        var user=await _userManager.FindByNameAsync(username);
        if(user==null) return NotFound();
        var userRoles=await _userManager.GetRolesAsync(user);
        var result=await _userManager.AddToRolesAsync(user,SelectedRoles.Except(userRoles));
        if(!result.Succeeded)return BadRequest("failed to add to roles");
        result=await _userManager.RemoveFromRolesAsync(user,userRoles.Except(SelectedRoles));
        if(!result.Succeeded)return BadRequest("failed to remove from roles");
        return Ok(await _userManager.GetRolesAsync(user));
    }
    
        

    }
}