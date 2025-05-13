namespace ExpenseTracker.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/group")]
public class GroupController : ControllerBase
{
    private readonly ILogger<GroupController> _logger;

    public GroupController(ILogger<GroupController> logger)
    {
        _logger = logger;
    }

    [HttpGet("test")]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }
}