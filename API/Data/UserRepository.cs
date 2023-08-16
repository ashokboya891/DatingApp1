
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _Context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context,IMapper mapper)
        {
            _Context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(String username)
        {
         return await _Context.Users.Where(x=>x.UserName==username)
         .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
         .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _Context.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public  async Task<AppUser> GetUserByIdAsync(int id)
        {
          return await _Context.Users.FindAsync(id);
        }

        // public async Task<AppUser> GetUserByUserNameAsync(string username)
        // {
        //   return await _Context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        // }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _Context.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);      
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
          return await _Context.Users.Include(x=>x.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _Context.SaveChangesAsync()>0;
        }

        public void Update(AppUser user)
        {
            _Context.Entry(user).State = EntityState.Modified;  
        }
    }
}