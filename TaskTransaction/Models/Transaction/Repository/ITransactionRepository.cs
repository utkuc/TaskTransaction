namespace TaskTransaction.Models.Transaction.Repository;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction> GetByIdAsync(long transactionId);
    Task<IEnumerable<Transaction>> GetByUserIdAsync(string userId);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task<bool> DeleteAsync(long transactionId);
}