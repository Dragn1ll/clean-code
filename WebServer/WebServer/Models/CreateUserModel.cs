using System.ComponentModel.DataAnnotations;

namespace WebServer.Models;

public record CreateUserModel(
    [Required] string Username,
    [Required] string Email,
    [Required] string Password
    );