using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTransaction.Models.User;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string UserID { get; set; } = Guid.NewGuid().ToString();

    public List<Transaction.Transaction> Transactions { get; set; }
}