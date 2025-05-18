namespace ExpenseTracker.DTO;

public class CreateUserDTO
{
    public string Name { get; set; }
}

public class LoginDTO
{
    public string Name { get; set; }
}

public class ListUserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
}