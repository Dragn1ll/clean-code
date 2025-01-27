using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Accesses;

public record DeleteAccessRequest(
    [Required] string UserEmail,
    [Required] Guid DocumentId
    );