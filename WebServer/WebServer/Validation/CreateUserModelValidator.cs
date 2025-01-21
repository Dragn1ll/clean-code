using FluentValidation;
using WebServer.Models;

namespace WebServer.Validation;

public class CreateUserModelValidator : AbstractValidator<CreateUserModel>
{
    public CreateUserModelValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is required")
            .MinimumLength(8).WithMessage("Email length must be between 0 and 30 characters")
            .MaximumLength(30).WithMessage("Email length must be between 0 and 30 characters");
        
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password length must be between 8 and 20 characters")
            .MaximumLength(20).WithMessage("Password length must be between 8 and 20 characters")
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one digit, special symbol and upper case letter");

        RuleFor(user => user.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(1).WithMessage("Username length must be between 1 and 25 characters")
            .MaximumLength(25).WithMessage("Username length must be between 1 and 25 characters");
    }
}