namespace ExpenseTracker.Services;

using ExpenseTracker.Interfaces;
using ExpenseTracker.Models;
using ExpenseTracker.DTO;
using Microsoft.EntityFrameworkCore;

public class GroupMemberService : IGroupMemberService
{
    private readonly AppDbContext _context;

    public GroupMemberService(AppDbContext context)
    {
        _context = context;
    }

    public List<GroupMemberDTO> GetGroupMembers(int groupId)
    {
        var group = _context.Groups.Include(g => g.Members).FirstOrDefault(g => g.Id == groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found");
        }

        List<GroupMember> groupMembers = group.Members;
        List<GroupMemberDTO> groupMemberDTOs = new List<GroupMemberDTO>();
        foreach(GroupMember groupMember in groupMembers){
            var user = _context.Users.FirstOrDefault(u => u.Id == groupMember.UserId);
            if (user == null) continue;
            GroupMemberDTO groupMemberDTO = new GroupMemberDTO
            {
                Id = user.Id,
                Name = user.Name,
                Balance = groupMember.Balance
            };
            groupMemberDTOs.Add(groupMemberDTO);
        }   
        return groupMemberDTOs;
    }
}