using ExpenseTracker.Models;

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

public class CreateTransactionDTO
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public String SplitType { get; set; }
    public List<SplitDTO> Splits { get; set; }
}

public class SplitDTO
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}

public class UserTransactionIdDTO
{
    public int UserId { get; set; }
    public int TransactionId { get; set; }
}

public class SettleDTO
{
    public int SplitId { get; set; }
    public string UserName { get; set; }
    public decimal Amount { get; set; }
}

public class SettleUserIdDTO
{
    public int UserId { get; set; }
    public int SplitId { get; set; }
}