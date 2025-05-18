namespace ExpenseTracker.Controllers;

using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;

[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;
    private readonly AppDbContext _context;

    public ExampleController(ILogger<ExampleController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("example")]
    public IActionResult GetExample()
    {
        _logger.LogInformation("GetExample method called");
        var result = new Dictionary<string, object>
        {
            { "message", "Example controller response" },
        };
        return Ok(result);
    }

}