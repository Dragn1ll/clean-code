using System.ComponentModel.DataAnnotations;

namespace WebServer.Models;

public record LoginUserModel(
    [Required] string Email,
    [Required] string Password
    );