using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext context;
        private object value;

        public PhotoRepository(DataContext context)
        {
            this.context = context;
        }

        public PhotoRepository(DataContext context, object value) : this(context)
        {
            this.value = value;
        }

        public async Task<Photo?> GetPhotoById(int id)
        {
            return await context.Photos
                       .IgnoreQueryFilters()
                       .SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await context.Photos
                       .IgnoreQueryFilters()
                       .Where(p => p.IsApproved == false)
                       .Select(u => new PhotoForApprovalDto
                       {
                           Id
           = u.Id,
                           Username
           = u.AppUser.UserName,
                           Url
           = u.Url,
                           IsApproved = u.IsApproved
                       }).ToListAsync();
        }
        public void RemovePhoto(Photo photo)
        {
            context.Photos.Remove(photo);
        }
    }
}
