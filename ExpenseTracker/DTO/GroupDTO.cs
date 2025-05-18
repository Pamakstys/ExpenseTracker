namespace ExpenseTracker.DTO;

public class CreateGroupDTO
{
    public string GroupName { get; set; }
    public int UserId { get; set; }
}

public class ListGroupDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Balance { get; set; }
}

public class ViewGroupDTO
{
    public int Id { get; set; }
    public string Title { get; set; }

    public List<GroupMemberDTO> Members { get; set; }
    public List<TransactionDTO> Transactions { get; set; }
}

public class UserGroupIdDTO
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
}

public class GroupMemberDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
}