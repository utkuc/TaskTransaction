using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskTransaction.Models.DbContext;
using TaskTransaction.Models.Transaction;
using TaskTransaction.Models.Transaction.Enum;
using TaskTransaction.Models.User;
using TaskTransaction.Models.User.Repository;
using TaskTransaction.Services;

namespace TaskTransaction.Test.IntegrationTests;

[TestFixture]
public class UserServiceIntegrationTests
{
    private TransactionContext _context;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TransactionContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new TransactionContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _userService = new UserService(new LoggerFactory().CreateLogger<UserService>(), new UserRepository(_context),
            _context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [Test]
    public async Task UpdateUserIdAsync_UpdatesTransactionsAndDeletesOldUser()
    {
        var oldUser = new User { UserID = "old" };
        _context.Users.Add(oldUser);
        await _context.SaveChangesAsync();

        var transaction = new Transaction
        {
            UserID = "old",
            Amount = 50,
            TransactionType = TransactionType.Credit,
            CreatedTime = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        _ = await _userService.UpdateUserIdAsync("old", "new");

        var updatedTransaction = await _context.Transactions.FirstOrDefaultAsync(t => t.UserID == "new");
        Assert.That(updatedTransaction, Is.Not.Null);
        Assert.That(updatedTransaction.UserID, Is.EqualTo("new"));

        var deletedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserID == "old");
        Assert.That(deletedUser, Is.Null);

        var newUser = await _context.Users.FirstOrDefaultAsync(u => u.UserID == "new");
        Assert.That(newUser, Is.Not.Null);
        Assert.That(newUser.UserID, Is.EqualTo("new"));
    }
}