using TaskTransaction.Models.Transaction.Enum;

namespace TaskTransaction.Models.Transaction.Dto;

public class TransactionDto
{
    public long TransactionID { get; set; }
    public string UserID { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public DateTime CreatedTime { get; set; }
}