namespace ExpenseTracker.DTO;

public class TransactionDTO
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int GroupId { get; set; }
}