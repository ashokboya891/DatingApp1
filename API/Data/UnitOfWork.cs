
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly DataContext _context;
        private readonly IMapper _mapper;

        public UnitOfWork(DataContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
           
        }
        public IUserRepository userRepository =>new UserRepository(_context,_mapper);

        public IMessageRepository messageRepository =>new MessageRepository(_context,_mapper);

        public ILikesRespository likesRespository =>new LikesRepository(_context);

        public IPhotoRepository PhotoRepository => new PhotoRepository(_context);


        public async Task<bool> Complete()
        {
           return await _context.SaveChangesAsync()>0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}