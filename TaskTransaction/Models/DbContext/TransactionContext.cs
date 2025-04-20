using Microsoft.EntityFrameworkCore;

namespace TaskTransaction.Models.DbContext;

public class TransactionContext(DbContextOptions<TransactionContext> options)
    : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<User.User> Users { get; set; }
    public DbSet<Transaction.Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapTables(modelBuilder);
        MapUserAndTransactionRelations(modelBuilder);

        modelBuilder.Entity<Transaction.Transaction>()
            .Property(t => t.CreatedTime)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

    private void MapTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User.User>().ToTable("User");
        modelBuilder.Entity<Transaction.Transaction>().ToTable("Transaction");
    }

    private void MapUserAndTransactionRelations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction.Transaction>()
            .HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserID);
    }
}