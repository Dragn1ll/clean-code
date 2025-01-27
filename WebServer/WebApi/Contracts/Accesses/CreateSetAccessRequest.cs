using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Accesses;

public record CreateSetAccessRequest(
    [Required] Guid DocumentId,
    [Required] string UserEmail,
    [Required] int PermissionId
    );