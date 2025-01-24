using System.ComponentModel.DataAnnotations;

namespace WebServer.Contracts.Users;

public record LoginUserRequest(
    [Required] string Email,
    [Required] string Password);