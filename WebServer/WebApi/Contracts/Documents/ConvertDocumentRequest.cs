using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Documents;

public record ConvertDocumentRequest(
    [Required] Guid documentId,
    [Required] string content);