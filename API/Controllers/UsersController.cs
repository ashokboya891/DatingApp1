using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]  //apiusers
    public class UsersController:BaseApiController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            this._context=context;

        }
        [AllowAnonymous]
        [HttpGet]
        public async  Task<ActionResult  <IEnumerable<AppUser>>> GetUsers()
        {
            var result= await _context.Users.ToListAsync();
            return result; 
        }
         [HttpGet("{id}")]
        public async  Task<ActionResult<AppUser>> GetUsers(int id)
        {
            return  await _context.Users.FindAsync(id);
             
        }
      
    }
}