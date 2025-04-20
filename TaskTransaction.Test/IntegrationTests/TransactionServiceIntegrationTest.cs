using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskTransaction.Models.DbContext;
using TaskTransaction.Models.Transaction;
using TaskTransaction.Models.Transaction.Enum;
using TaskTransaction.Models.Transaction.Repository;
using TaskTransaction.Services;

namespace TaskTransaction.Test.IntegrationTests;

[TestFixture]
public class TransactionServiceIntegrationTest
{
    private TransactionContext _context;
    private TransactionService _transactionService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TransactionContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new TransactionContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();
        //disable fk for testing transactions on their own.
        _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");

        _transactionService = new TransactionService(new LoggerFactory().CreateLogger<TransactionService>(),
            new TransactionRepository(_context),
            _context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [Test]
    public async Task CreateTransactionAsync_AddsTransaction()
    {
        var transaction = new Transaction
        {
            UserID = "123",
            Amount = 555,
            TransactionType = TransactionType.Credit,
            CreatedTime = DateTime.UtcNow
        };

        await _transactionService.CreateTransactionAsync(transaction);

        var result = await _context.Transactions.FindAsync(transaction.TransactionID);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Amount, Is.EqualTo(555));
    }
}