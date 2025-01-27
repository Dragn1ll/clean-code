using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Documents;

public record CreateDocumentRequest(
    [Required] string Title
    );