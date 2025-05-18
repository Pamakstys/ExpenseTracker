namespace ExpenseTracker.Models;

public class TransactionSplit
{
    public int Id { get; set; }
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public bool IsSettled { get; set; }
    
    public Transaction Transaction { get; set; }
    public User User { get; set; }

    public TransactionSplit() { }
}