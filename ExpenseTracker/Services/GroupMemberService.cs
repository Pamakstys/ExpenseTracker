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

    public GroupMemberDTO GetGroupMember(int groupId, int userId)
    {
        var group = _context.Groups.Include(g => g.Members).FirstOrDefault(g => g.Id == groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found");
        }

        var groupMember = group.Members.FirstOrDefault(m => m.UserId == userId);
        if (groupMember == null)
        {
            throw new ArgumentException("User is not a member of the group");
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        GroupMemberDTO groupMemberDTO = new GroupMemberDTO
        {
            Id = user.Id,
            Name = user.Name,
            Balance = groupMember.Balance
        };

        return groupMemberDTO;
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

    public void RemoveGroupMember(int groupId, int userId)
    {
        var group = _context.Groups.Include(g => g.Members).FirstOrDefault(g => g.Id == groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found");
        }

        var groupMember = group.Members.FirstOrDefault(m => m.UserId == userId);
        if (groupMember == null)
        {
            throw new ArgumentException("User is not a member of the group");
        }

        group.Members.Remove(groupMember);
        _context.Update(group);
        _context.GroupMembers.Remove(groupMember);
        _context.SaveChanges();
    }

    public void AddGroupMember(int groupId, int userId)
    {
        var group = _context.Groups.Include(g => g.Members).FirstOrDefault(g => g.Id == groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found");
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var groupMember = new GroupMember();
        groupMember.UserId = userId;
        groupMember.GroupId = groupId;
        groupMember.IsSettled = false;
        groupMember.Balance = 0.0m;
        groupMember.User = user;
        groupMember.Group = group;

        group.Members.Add(groupMember);
        _context.GroupMembers.Add(groupMember);
        _context.Update(group);
        _context.SaveChanges();
    }
}