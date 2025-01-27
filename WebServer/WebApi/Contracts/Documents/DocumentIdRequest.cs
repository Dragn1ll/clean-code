using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Documents;

public record DocumentIdRequest(
    [Required] Guid DocumentId
    );