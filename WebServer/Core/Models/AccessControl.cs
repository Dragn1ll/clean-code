using Core.Enum;

namespace Core.Models;

public class AccessControl(Guid userId, Guid documentId, Permissions permission)
{
    public Guid UserId { get; } = userId;
    public Guid DocumentId { get; } = documentId;
    public Permissions Permission { get; } = permission;
}