namespace ExpenseTracker.Services;

using ExpenseTracker.Interfaces;
using ExpenseTracker.Models;
using ExpenseTracker.DTO;
using Microsoft.EntityFrameworkCore;

public class GroupService : IGroupService
{
    private readonly AppDbContext _context;
    private readonly IGroupMemberService _groupMemberService;

    public GroupService(AppDbContext context, IGroupMemberService groupMemberService)
    {
        _context = context;
        _groupMemberService = groupMemberService;
    }

    public void RemoveGroupMember(UserGroupIdDTO RemoveGroupMemberDTO)
    {
        if (string.IsNullOrWhiteSpace(RemoveGroupMemberDTO.UserId.ToString()))
        {
            throw new ArgumentException("User ID cannot be empty");
        }
        if (string.IsNullOrWhiteSpace(RemoveGroupMemberDTO.GroupId.ToString()))
        {
            throw new ArgumentException("Group ID cannot be empty");
        }

        int userId = RemoveGroupMemberDTO.UserId;
        int groupId = RemoveGroupMemberDTO.GroupId;

        var group = _context.Groups
            .Include(g => g.Members)
            .FirstOrDefault(g => g.Id == groupId);

        if (group == null)
        {
            throw new ArgumentException("Group not found");
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        GroupMember groupMember = group.Members.FirstOrDefault(gm => gm.UserId == userId);
        if (groupMember != null)
        {
            if(groupMember.IsSettled == false)
            {
                throw new InvalidOperationException("Cannot remove member if not settled");
            }
            group.Members.Remove(groupMember);
            _context.GroupMembers.Remove(groupMember);
            _context.Groups.Update(group);
            _context.SaveChanges();
        }
    }

    public GroupMember AddGroupMember(UserGroupIdDTO addGroupMemberDTO)
    {
        if (string.IsNullOrWhiteSpace(addGroupMemberDTO.UserId.ToString()))
        {
            throw new ArgumentException("User ID cannot be empty");
        }
        if (string.IsNullOrWhiteSpace(addGroupMemberDTO.GroupId.ToString()))
        {
            throw new ArgumentException("Group ID cannot be empty");
        }

        int userId = addGroupMemberDTO.UserId;
        int groupId = addGroupMemberDTO.GroupId;

        var group = _context.Groups
            .Include(g => g.Members)
            .FirstOrDefault(g => g.Id == groupId);

        if (group == null)
        {
            throw new ArgumentException("Group not found");
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        GroupMember groupMember = new GroupMember(user, group);
        group.Members.Add(groupMember);
        _context.GroupMembers.Add(groupMember);
        _context.Groups.Update(group);
        _context.SaveChanges();

        return groupMember;
    }

    public ViewGroupDTO ViewGroup(UserGroupIdDTO groupViewDTO)
    {
        var group = _context.Groups
            .Include(g => g.Members)
            .FirstOrDefault(g => g.Id == groupViewDTO.GroupId);

        if (group == null)
        {
            throw new ArgumentException("Group not found");
        }
        var user = _context.Users.FirstOrDefault(u => u.Id == groupViewDTO.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        if (!group.Members.Any(gm => gm.UserId == user.Id))
        {
            throw new ArgumentException("User is not a member of the group");
        }
        ViewGroupDTO viewGroupDTO = new ViewGroupDTO();

        viewGroupDTO.Id = group.Id;
        viewGroupDTO.Title = group.Title;

        List<GroupMember> groupMembers = group.Members;
        List<GroupMemberDTO> groupMemberDTOs = _groupMemberService.GetGroupMembers(groupViewDTO.GroupId);
        viewGroupDTO.Members = groupMemberDTOs;

        List<TransactionDTO> transactionDTOs = new List<TransactionDTO>();
        List<Transaction> transactions = group.Transactions;
        foreach (Transaction transaction in transactions)
        {
            TransactionDTO transactionDTO = new TransactionDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                UserId = transaction.User.Id,
                UserName = transaction.User.Name
            };
            transactionDTOs.Add(transactionDTO);
        }
        viewGroupDTO.Transactions = transactionDTOs;
        return viewGroupDTO;
    }

    public Group CreateGroup(CreateGroupDTO createGroupDTO)
    {
        if (string.IsNullOrWhiteSpace(createGroupDTO.GroupName))
        {
            throw new ArgumentException("Group name cannot be empty");
        }
        var user = _context.Users.FirstOrDefault(u => u.Id == createGroupDTO.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        Group group = new Group
        {
            Title = createGroupDTO.GroupName
        };
        _context.Groups.Add(group);
        _context.SaveChanges();

        GroupMember groupMember = new GroupMember(user, group);
        group.Members.Add(groupMember);
        _context.Groups.Update(group);
        _context.GroupMembers.Add(groupMember);
        _context.SaveChanges();

        return group;
    }

    public List<ListGroupDTO> GetGroupsByUserId(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        List<GroupMember> groupMembers = _context.GroupMembers
            .Include(gm => gm.Group)
            .Where(gm => gm.UserId == userId)
            .ToList();

        List<ListGroupDTO> groups = new List<ListGroupDTO>();
        foreach (var groupMember in groupMembers)
        {
            ListGroupDTO groupDTO = new ListGroupDTO
            {
                Id = groupMember.Group.Id,
                Title = groupMember.Group.Title,
                Balance = groupMember.Balance
            };
            groups.Add(groupDTO);
        }

        return groups;
    }
    
}