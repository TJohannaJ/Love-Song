using Backend.Domain.Entities;

namespace Backend.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> GetByIdAsync(string id);
        Task<IEnumerable<Message>> GetByLikeIdAsync(string likeId);
        Task AddAsync(Message message);
        Task SaveChangesAsync();
        Task<Message?> GetLastMessageByLikeIdsAsync(List<string> likeIds);
    }
}
