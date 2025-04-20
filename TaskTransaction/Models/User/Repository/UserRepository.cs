using Microsoft.EntityFrameworkCore;
using TaskTransaction.Models.DbContext;

namespace TaskTransaction.Models.User;

public class UserRepository(TransactionContext context) : IUserRepository
{
    private IUserRepository _userRepositoryImplementation;

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User> GetByIdAsync(string userId)
    {
        return await context.Users.FindAsync(userId);
    }

    public async Task AddAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(string userId)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}