using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Users;

public record LoginUserRequest(
    [Required] string Email,
    [Required] string Password);