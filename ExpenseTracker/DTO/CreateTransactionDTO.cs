namespace ExpenseTracker.DTO;

public class CreateTransactionDTO
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public String SplitType { get; set; }
    public List<SplitDTO> Splits { get; set; }
}