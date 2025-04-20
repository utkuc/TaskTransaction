using Microsoft.EntityFrameworkCore;
using TaskTransaction.Models.DbContext;
using TaskTransaction.Models.Transaction.Enum;

namespace TaskTransaction.Models.Transaction.Repository;

public class TransactionRepository(TransactionContext context) : ITransactionRepository
{
    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await context.Transactions.ToListAsync();
    }

    public async Task<Transaction> GetByIdAsync(long transactionId)
    {
        return await context.Transactions.FindAsync(transactionId);
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(string userId)
    {
        return await context.Transactions
            .Where(t => t.UserID == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Transaction transaction)
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        context.Transactions.Update(transaction);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(long transactionId)
    {
        var transaction = await context.Transactions.FindAsync(transactionId);
        if (transaction != null)
        {
            context.Transactions.Remove(transaction);
            await context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<List<Transaction>> GetTransactionsAboveThresholdAsync(decimal threshold)
    {
        return await context.Transactions
            .Where(t => t.Amount > threshold)
            .ToListAsync();
    }
}