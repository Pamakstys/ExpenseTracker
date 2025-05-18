namespace ExpenseTracker.Models;

public class Group
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<GroupMember> Members { get; set; }
    public List<Transaction> Transactions { get; set; }
    
    public Group()
    {
        Members = new List<GroupMember>();
        Transactions = new List<Transaction>();
    }
}

