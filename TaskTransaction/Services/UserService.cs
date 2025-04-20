using Microsoft.EntityFrameworkCore;
using TaskTransaction.Models.DbContext;
using TaskTransaction.Models.User;

namespace TaskTransaction.Services;

public class UserService(ILogger<UserService> logger, IUserRepository userRepository, TransactionContext context)
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

    public async Task<User> UpdateUserIdAsync(string previousUserId, string newUserId)
    {
        var existingUser = await userRepository.GetByIdAsync(previousUserId);
        if (existingUser == null)
            return null;
        var possibleConflictedUser = await userRepository.GetByIdAsync(newUserId);
        if (possibleConflictedUser != null)
        {
            logger.LogWarning("UpdateUserIdAsync conflicted with existing user");
            return null;
        }

        var newUser = new User
        {
            UserID = newUserId,
        };
        await userRepository.DeleteAsync(existingUser.UserID);
        await userRepository.AddAsync(newUser);
        return newUser;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        return await userRepository.DeleteAsync(userId);
    }

    public async Task CreateUserAsync(User user)
    {
        await userRepository.AddAsync(user);
    }

    public async Task<decimal> GetTotalTransactionAmountAsync(string userId)
    {
        return await context.Transactions
            .Where(t => t.UserID == userId)
            .SumAsync(t => t.Amount);
    }
}