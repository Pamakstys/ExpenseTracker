namespace ExpenseTracker.Controllers;

using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.DTO;
using ExpenseTracker.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly ITransactionService _transactionService;

    public TransactionController(ILogger<TransactionController> logger, ITransactionService TranscationService)
    {
        _logger = logger;
        _transactionService = TranscationService;
    }

    [HttpPost("create")]
    public IActionResult CreateTransaction([FromBody] CreateTransactionDTO transactionDto)
    {
        var response = new Dictionary<string, object>();
        try
        {
            _transactionService.AddTransaction(transactionDto);
            response.Add("success", "Transaction created successfully");
            return Ok(response);
        }
        catch (Exception e)
        {
            response.Add("error", e.Message);
            return BadRequest(response);
        }
    }

    [HttpPost("get-splits")]
    public IActionResult GetSettles([FromBody] UserGroupIdDTO memberGroupUserDto)
    {
        var response = new Dictionary<string, object>();
        try
        {
            var transactionSplits = _transactionService.GetSettles(memberGroupUserDto);
            return Ok(transactionSplits);
        }
        catch (Exception e)
        {
            response.Add("error", e.Message);
            return BadRequest(response);
        }
    }

    [HttpPost("settle")]
    public IActionResult SettleTransactionSplit([FromBody] SettleUserIdDTO settleUserIdDto)
    {
        var response = new Dictionary<string, object>();
        try
        {
            _transactionService.SettleTransactionSplit(settleUserIdDto);
            response.Add("success", "Transaction settled successfully");
            return Ok(response);
        }
        catch (Exception e)
        {
            response.Add("error", e.Message);
            return BadRequest(response);
        }
    }
    [HttpPost("view")]
    public IActionResult GetTransaction([FromBody] UserTransactionIdDTO userTransactionDTO)
    {
        var response = new Dictionary<string, object>();
        try
        {
            var transaction = _transactionService.GetTransaction(userTransactionDTO);
            return Ok(transaction);
        }
        catch (Exception e)
        {
            response.Add("error", e.Message);
            return BadRequest(response);
        }
    }
}