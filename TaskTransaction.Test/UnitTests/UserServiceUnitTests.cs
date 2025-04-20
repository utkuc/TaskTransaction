using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskTransaction.Models.DbContext;
using TaskTransaction.Models.Transaction;
using TaskTransaction.Models.Transaction.Enum;
using TaskTransaction.Models.User;
using TaskTransaction.Models.User.Repository;
using TaskTransaction.Services;

namespace TaskTransaction.Test.UnitTests;

[TestFixture]
public class Tests
{
    private Mock<IUserRepository> _userRepoMock;
    private ILogger<UserService> _logger;
    private TransactionContext _context;
    private UserService _userService;


    [SetUp]
    public void Setup()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _logger = new LoggerFactory().CreateLogger<UserService>();

        var options = new DbContextOptionsBuilder<TransactionContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new TransactionContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();
        //disable fk for testing transactions on their own.
        _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
        _userService = new UserService(_logger, _userRepoMock.Object, _context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetUserByIdAsync_ReturnsUser()
    {
        var user = new User { UserID = "123" };
        _userRepoMock.Setup(userRepository => userRepository.GetByIdAsync("123")).ReturnsAsync(user);

        var result = await _userService.GetUserByIdAsync("123");

        Assert.That(result.UserID, Is.EqualTo("123"));
    }

    [Test]
    public async Task DeleteUserAsync_CallsRepository()
    {
        _userRepoMock.Setup(r => r.DeleteAsync("del")).ReturnsAsync(true);

        var result = await _userService.DeleteUserAsync("del");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task GetTotalTransactionAmountAsync_ReturnsSum()
    {
        _context.Transactions.AddRange(
            new Transaction
            {
                UserID = "123",
                Amount = 10,
                TransactionType = TransactionType.Debit,
                CreatedTime = default
            },
            new Transaction
            {
                UserID = "123",
                Amount = 20,
                TransactionType = TransactionType.Credit,
                CreatedTime = default
            }
        );
        await _context.SaveChangesAsync();

        var result = await _userService.GetTotalTransactionAmountAsync("123");

        Assert.That(result, Is.EqualTo(30));
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
            Amount = 10,
            TransactionType = TransactionType.Debit,
            CreatedTime = default
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        _userRepoMock.Setup(userRepository => userRepository.GetByIdAsync("old")).ReturnsAsync(oldUser);
        _userRepoMock.Setup(userRepository => userRepository.DeleteAsync("old")).ReturnsAsync(true);
        _userRepoMock.Setup(userRepository => userRepository.GetByIdAsync("new")).ReturnsAsync((User)null);
        _userRepoMock.Setup(userRepository => userRepository.AddAsync(It.Is<User>(u => u.UserID == "new")))
            .Returns(Task.CompletedTask);


        var result = await _userService.UpdateUserIdAsync("old", "new");

        var updatedTransaction = await _context.Transactions.FirstAsync();
        Assert.Multiple(() =>
        {
            Assert.That(updatedTransaction.UserID, Is.EqualTo("new"));
            Assert.That(result.UserID, Is.EqualTo("new"));
        });
    }
}