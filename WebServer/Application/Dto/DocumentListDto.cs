using System.ComponentModel.DataAnnotations;

namespace Application.Dto;

public class DocumentListDto
{
    [Required] public Guid DocumentId { get; set; }
    [Required] public string Title { get; set; }
    [Required] public DateTime Created { get; set; }
}