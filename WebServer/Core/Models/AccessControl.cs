using Core.Enum;

namespace Core.Models;

public class AccessControl(Guid userId, Guid documentId, Permissions permissions)
{
    public Guid UserId { get; } = userId;
    public Guid DocumentId { get; } = documentId;
    public Permissions Permissions { get; } = permissions;
}