using Microsoft.EntityFrameworkCore;
using TaskTransaction.Models.DbContext;
using TaskTransaction.Models.Transaction;
using TaskTransaction.Models.Transaction.Dto;
using TaskTransaction.Models.Transaction.Enum;
using TaskTransaction.Models.Transaction.Repository;
using TaskTransaction.Models.User;

namespace TaskTransaction.Services;

public class TransactionService(
    ILogger<TransactionService> logger,
    ITransactionRepository transactionRepository,
    TransactionContext context)
{
    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await transactionRepository.GetAllAsync();
    }

    public async Task<Transaction> GetTransactionByIdAsync(long transactionId)
    {
        return await transactionRepository.GetByIdAsync(transactionId);
    }

    public async Task CreateTransactionAsync(Transaction transaction)
    {
        await transactionRepository.AddAsync(transaction);
    }

    public async Task<Dictionary<TransactionType, decimal>> GetTotalAmountByTransactionTypeAsync()
    {
        var totalByType = await context.Transactions
            .GroupBy(transaction => transaction.TransactionType)
            .Select(group => new
            {
                TransactionType = group.Key,
                TotalAmount = group.Sum(t => t.Amount)
            })
            .ToListAsync();
        var total = totalByType.ToDictionary(x => x.TransactionType, x => x.TotalAmount);

        return total;
    }

    public async Task<List<Transaction>> GetTransactionsAboveThresholdAsync(decimal threshold)
    {
        var result = await transactionRepository.GetTransactionsAboveThresholdAsync(threshold);
        return result;
    }
}