namespace ExpenseTracker.Interfaces;

using ExpenseTracker.DTO;
using ExpenseTracker.Models;

public interface IGroupMemberService
{
    List<GroupMemberDTO> GetGroupMembers(int groupId);
}