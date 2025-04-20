namespace TaskTransaction.Models.User;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(string userId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> DeleteAsync(string userId);
}