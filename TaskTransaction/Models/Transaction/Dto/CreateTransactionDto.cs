using TaskTransaction.Models.Transaction.Enum;

namespace TaskTransaction.Models.Transaction.Dto;

public class CreateTransactionDto
{
    public string UserID { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
}