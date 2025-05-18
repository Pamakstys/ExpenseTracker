namespace ExpenseTracker.Models;

public class Transaction
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string SplitType { get; set; }
    public Group Group { get; set; }
    public User User { get; set; }

    public List<TransactionSplit> TransactionSplits { get; set; }

    public Transaction()
    {
        TransactionSplits = new List<TransactionSplit>();
    }
}