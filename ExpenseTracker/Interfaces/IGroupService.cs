namespace ExpenseTracker.Interfaces;

using ExpenseTracker.Models;
using ExpenseTracker.DTO;

public interface IGroupService
{
    List<ListGroupDTO> GetGroupsByUserId(int userId);
    Group CreateGroup(CreateGroupDTO createGroupDTO);
    ViewGroupDTO ViewGroup(UserGroupIdDTO groupViewDTO);

    GroupMember AddGroupMember(UserGroupIdDTO addGroupMemberDTO);
    void RemoveGroupMember(UserGroupIdDTO removeGroupMemberDTO);
}