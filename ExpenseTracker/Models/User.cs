namespace ExpenseTracker.Models;

public class User {
    public int Id {get; set;}
    public string Name {get; set;}

    public List<GroupMember> Groups { get; set; }

    public User() {
        Groups = new List<GroupMember>();
    }
}