using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Accesses;

public record GetAccessRequest(
    [Required] Guid DocumentId
    );