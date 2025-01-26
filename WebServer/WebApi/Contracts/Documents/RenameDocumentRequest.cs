using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Documents;

public record RenameDocumentRequest(
    [Required] Guid DocumentId,
    [Required] string NewName
    );