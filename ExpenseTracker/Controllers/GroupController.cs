namespace ExpenseTracker.Controllers;

using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.DTO;
using ExpenseTracker.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly ILogger<GroupController> _logger;
    private readonly IGroupService _groupService;

    public GroupController(ILogger<GroupController> logger, IGroupService groupService)
    {
        _logger = logger;
        _groupService = groupService;
    }

    [HttpPost("create")]
    public IActionResult CreateGroup([FromBody] CreateGroupDTO createGroupDTO)
    {
        var response = new Dictionary<string, object>();
        try
        {
            var group = _groupService.CreateGroup(createGroupDTO);
            response.Add("id", group.Id);
            return Ok(response);
        }
        catch (ArgumentException ae)
        {
            response.Add("error", ae.Message);
            return BadRequest(response);
        }
    }

    [HttpGet("getByUser/{userId}")]
    public IActionResult GetGroupsByUserId(int userId)
    {
        var response = new Dictionary<string, object>();
        try
        {
            var groups = _groupService.GetGroupsByUserId(userId);
            response.Add("groups", groups);
            return Ok(response);
        }
        catch (ArgumentException ae)
        {
            response.Add("error", ae.Message);
            return BadRequest(response);
        }
    }

    [HttpPost("view")]
    public IActionResult ViewGroup([FromBody] UserGroupIdDTO groupViewDTO)
    {
        var response = new Dictionary<string, object>();
        try
        {
            var group = _groupService.ViewGroup(groupViewDTO);
            return Ok(group);
        }
        catch (ArgumentException ae)
        {
            response.Add("error", ae.Message);
            return BadRequest(response);
        }
    }

    [HttpPost("addMember")]
    public IActionResult AddMember([FromBody] UserGroupIdDTO addGroupMemberDTO)
    {
        var response = new Dictionary<string, object>();
        try
        {
            var groupMember = _groupService.AddGroupMember(addGroupMemberDTO);
            response.Add("id", groupMember.Id);
            return Ok(response);
        }
        catch (ArgumentException ae)
        {
            response.Add("error", ae.Message);
            return BadRequest(response);
        }
    }

    [HttpPost("removeMember")]
    public IActionResult RemoveMember([FromBody] UserGroupIdDTO removeGroupMemberDTO)
    {
        var response = new Dictionary<string, object>();
        try
        {
            _groupService.RemoveGroupMember(removeGroupMemberDTO);
            return Ok();
        }
        catch (ArgumentException ae)
        {
            response.Add("error", ae.Message);
            return BadRequest(response);
        }
    }

}