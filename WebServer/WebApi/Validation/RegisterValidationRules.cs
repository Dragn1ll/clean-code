using FluentValidation;
using WebApi.Contracts.Users;

namespace WebApi.Validation;

public class RegisterValidationRules : AbstractValidator<RegisterUserRequest>
{
    public RegisterValidationRules()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Имя обязательно!")
            .MinimumLength(4).WithMessage("Имя минимум 4 символов!")
            .MaximumLength(20).WithMessage("Имя максимум 20 символов!");
        
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Электронная почта обязательна!")
            .EmailAddress().WithMessage("Нарушен формат электронной почты!");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Пароль обязателен!")
            .MinimumLength(8).WithMessage("Пароль должен быть минимум 8 символов!")
            .MaximumLength(25).WithMessage("Пароль должен быть максимум 25 символов!");
    }
}