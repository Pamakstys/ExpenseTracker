namespace ExpenseTracker.Interfaces;

using ExpenseTracker.Models;
using ExpenseTracker.DTO;

public interface IUserService
{
    User Register(CreateUserDTO createUserDTO);
    User Login(LoginDTO loginDTO);
    List<ListUserDTO> GetAllUsers();
}