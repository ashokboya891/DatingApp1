using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
// using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
   [ApiController]
[Route("api/[controller]")]
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private  readonly IMapper _mapper;  
        public AccountController(DataContext context,ITokenService tokenService,IMapper mapper)
     {
        _mapper = mapper;
        _context=context;   
        _tokenService=tokenService;
     }

    [HttpPost("Register")] //post:api/account/register?username=ash&password=dev
    public async Task<ActionResult<UserDto>> Register(RegisterDTO   registerDTO)
        {
           if(await UserExists(registerDTO.Username))
           {
            return BadRequest("name already taken user different one");
           }
           var user=_mapper.Map<AppUser>(registerDTO);

                using var hmac=new HMACSHA512();

               
                    user.UserName=registerDTO.Username.ToLower();
                    user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
                    user.PasswordSalt=hmac.Key;
                
                _context.Users.Add(user);    
                await _context.SaveChangesAsync();
         
            return new UserDto
                {
                    Username=user.UserName,
                    Token=_tokenService.createToken(user),
                    KnownAs=user.KnownAs,
                    Gender=user.Gender
                };
        }
        private async Task<bool> UserExists(string username)
        {
         return await _context.Users.AnyAsync(x=>x.UserName==username.ToLower());  
        }
        
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO logindto)
        {
                var user=await _context.Users.Include(p=>p.Photos).
                SingleOrDefaultAsync(x=>x.UserName==logindto.UserName);
                if(user==null) return Unauthorized("not valid username");

                using var hmac=new HMACSHA512(user.PasswordSalt);

                var computedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.Password)) ; 

                for(int i=0;i<computedHash.Length;i++)
                {
                    if(computedHash[i]!=user.PasswordHash[i]) return Unauthorized("invalid password");
                }
                 return new UserDto
                {
                    Username=user.UserName,
                    Token=_tokenService.createToken(user),
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                    KnownAs=user.KnownAs,
                    Gender=user.Gender
                }; 
        }

    }
}