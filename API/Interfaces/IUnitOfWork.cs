

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository userRepository{get;}
        IMessageRepository messageRepository{get;}

        ILikesRespository likesRespository{get;}

        IPhotoRepository PhotoRepository { get; }

        Task<bool> Complete();

        bool HasChanges();
        
    }
}