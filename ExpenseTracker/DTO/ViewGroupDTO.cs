namespace ExpenseTracker.DTO;

public class ViewGroupDTO
{
    public int Id { get; set; }
    public string Title { get; set; }

    public List<GroupMemberDTO> Members {get; set;}
    public List<TransactionDTO> Transactions {get; set;}
}