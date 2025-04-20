using System.ComponentModel.DataAnnotations;
using TaskTransaction.Models.Transaction.Enum;

namespace TaskTransaction.Models.Transaction;

public class Transaction
{
    [Key] public long TransactionID { get; set; }
    [Required] public required string UserID { get; set; }
    [Required] public required decimal Amount { get; set; }
    [Required] public required TransactionType TransactionType { get; set; }
    [Required] public required DateTime CreatedTime { get; set; }

    public User.User User { get; set; }
}