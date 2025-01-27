using Core.Enum;

namespace Application.Dto;

public class AccessDto(Guid userId, string email, string permission)
{
    public Guid UserId { get; } = userId;
    public string Email { get; } = email;
    public string Permission { get; } = permission;
}