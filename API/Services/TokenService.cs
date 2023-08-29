
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // private readonly UserManager<AppUser> _userManager;
        private readonly UserManager<AppUser> _userManager ;

        private readonly SymmetricSecurityKey _key; 
        public TokenService(IConfiguration config,UserManager<AppUser> userManager)
        {
            _userManager = userManager;
              
            _key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
           

        }
      
        public async Task<string> createToken(AppUser user)
        {
           var claims=new List<Claim>
           {
            // new Claim(JwtRegisteredClaimNames.NameId,user.UserName)
             new Claim(JwtRegisteredClaimNames.NameId,user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName),

           };
           var roles= await _userManager.GetRolesAsync(user);

           claims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role)));

           var creds=new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature);
       
           var tokenDescriptor=new SecurityTokenDescriptor
           {
                Subject =new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(7),
                SigningCredentials=creds
           };
           var tokenhandler=new JwtSecurityTokenHandler();
           var token = tokenhandler.CreateToken(tokenDescriptor);
           return tokenhandler.WriteToken(token);
        }
    }
}