namespace ExpenseTracker.Interfaces;

using ExpenseTracker.DTO;
using ExpenseTracker.Models;

public interface IGroupMemberService
{
    void AddGroupMember(int groupId, int userId);
    void RemoveGroupMember(int groupId, int userId);
    List<GroupMemberDTO> GetGroupMembers(int groupId);
    GroupMemberDTO GetGroupMember(int groupId, int userId);
}