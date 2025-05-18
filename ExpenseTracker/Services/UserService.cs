namespace ExpenseTracker.Services;

using ExpenseTracker.Interfaces;
using ExpenseTracker.Models;
using ExpenseTracker.DTO;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public User Login(LoginDTO loginDTO)
    {
        if (string.IsNullOrWhiteSpace(loginDTO.Name))
        {
            throw new ArgumentException("Name cannot be empty");
        }

        var user = _context.Users.FirstOrDefault(u => u.Name == loginDTO.Name);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return user;
    }

    public User Register(CreateUserDTO createUserDTO)
    {
        if (string.IsNullOrWhiteSpace(createUserDTO.Name))
        {
            throw new ArgumentException("Name cannot be empty");
        }

        var existingUser = _context.Users.FirstOrDefault(u => u.Name == createUserDTO.Name);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with that name already exists");
        }

        User user = new User
        {
            Name = createUserDTO.Name
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    public List<ListUserDTO> GetAllUsers()
    {
        return _context.Users.Select(u => new ListUserDTO
        {
            Id = u.Id,
            Name = u.Name
        }).ToList();
    }
}