namespace Application.Dto;

public class UserDto(string name, string email)
{
    public string Name { get; } = name;
    public string Email { get; } = email;
}