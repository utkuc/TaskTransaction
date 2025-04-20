using TaskTransaction.Models.User;

namespace TaskTransaction.Services;

public class UserService (ILogger<UserService> logger,IUserRepository userRepository)
{
    public async Task<User> GetUserByIdAsync(string userId)
    {
        return await userRepository.GetByIdAsync(userId);
    }
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await userRepository.GetAllAsync();
    }

    public async Task<User> UpdateAsync(User user)
    {
        await userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        return await userRepository.DeleteAsync(userId);
    }
}