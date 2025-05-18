namespace ExpenseTracker.Models;

public class GroupMember
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public bool IsSettled { get; set; }
    public decimal Balance { get; set; }
    
    public User User { get; set; }
    public Group Group { get; set; }

    public GroupMember(){}
    public GroupMember(User user, Group group)
    {
        User = user;
        Group = group;
        UserId = user.Id;
        GroupId = group.Id;
        IsSettled = true;
        Balance = 0.0m;
    }
}