namespace ExpenseTracker.Controllers;

using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.DTO;
using ExpenseTracker.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }


    [HttpPost("register")]
    public IActionResult Register([FromBody] CreateUserDTO createUserDTO)
    {
        var response = new Dictionary<string, object>();
        try
        {
            var user = _userService.Register(createUserDTO);
            response.Add("id", user.Id);
            response.Add("name", user.Name);
            return Ok(response);
        }
        catch (ArgumentException ae)
        {
            response.Add("error", ae.Message);
            return BadRequest(response);
        }
        catch (InvalidOperationException ioe)
        {
            response.Add("error", ioe.Message);
            return Conflict(response);
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO loginDTO)
    {
        var response = new Dictionary<string, object>();
        try
        {
            var user = _userService.Login(loginDTO);
            response.Add("id", user.Id);
            response.Add("name", user.Name);
            return Ok(response);
        }
        catch (ArgumentException ae)
        {
            response.Add("error", ae.Message);
            return BadRequest(response);
        }
        catch (InvalidOperationException ioe)
        {
            response.Add("error", ioe.Message);
            return NotFound(response);
        }
    }

    [HttpGet("all")]
    public IActionResult GetAllUsers()
    {
        var response = new Dictionary<string, object>();
        try
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }
        catch (Exception e)
        {
            response.Add("error", e.Message);
            return BadRequest(response);
        }
    }
}