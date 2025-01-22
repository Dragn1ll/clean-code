using Core.Enum;

namespace Application.Dto;

public class AccessDto(Guid userId, string userName, Permissions permission)
{
    public Guid UserId { get; } = userId;
    public string UserName { get; } = userName;
    public Permissions Permissions { get; } = permission;
}