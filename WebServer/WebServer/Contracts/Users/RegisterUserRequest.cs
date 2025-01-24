using System.ComponentModel.DataAnnotations;

namespace WebServer.Contracts.Users;

public record RegisterUserRequest(
    [Required] string Name,
    [Required] string Email,
    [Required] string Password);