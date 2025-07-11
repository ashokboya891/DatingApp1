
using API.DTOs;
using API.Entities;
using API.Helpers;
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

        public UserRepository(DataContext context, IMapper mapper)
        {
            _Context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(String username)
        {
            return await _Context.Users.Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _Context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);



            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive),
            };


            return await PagedList<MemberDto>.CreateAsync(
            query.AsNoTracking().ProjectTo<MemberDto>
            (_mapper.ConfigurationProvider),
            userParams.PageNumber, userParams.PageSize);

        }

        public async Task<AppUser> GetUserByIdAsync(int id)
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
        //below new method added in opt2 244
        public async Task<string> GetUserGender(string username)
        {
            return await _Context.Users.Where(x => x.UserName == username).Select(x => x.Gender).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _Context.Users.Include(x => x.Photos).ToListAsync();
        }

        // public async Task<bool> SaveAllAsync()
        // {
        //     return await _Context.SaveChangesAsync()>0;
        // }

        public void Update(AppUser user)
        {
            _Context.Entry(user).State = EntityState.Modified;
        }

        public async Task<MemberDto> GetMemberAsync(string username, bool
                                 isCurrentUser)
        {
            var query = _Context.Users
                           .Where(x
           => x.UserName == username)
                           .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                           .AsQueryable();
            if (isCurrentUser) query = query.IgnoreQueryFilters();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<AppUser?> GetUserByPhotoId(int photoId)
        {
            return await _Context.Users
                       .Include(p => p.Photos)
                       .IgnoreQueryFilters()
                       .Where(p => p.Photos.Any(p => p.Id == photoId))
                       .FirstOrDefaultAsync();

        }
    }
}