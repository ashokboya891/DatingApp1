

using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController:BaseApiController
    {
        private  UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;

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
        [Authorize(policy:"ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("admins or moderators  can see this");
            
        }
        
    }
}