using FluentValidation;
using LdapConnector;

namespace AuthServer;

public class CredentialsValidator : AbstractValidator<CredentialsModel>
{
    public CredentialsValidator(ILdapRepository repository)
    {
        RuleFor(m => m.Login)
            .Cascade(CascadeMode.Stop)
            .Must(m => !m.Contains(' '))
            .WithMessage("Имя не должно содержать пробелы")
            .MaximumLength(80)
            .WithMessage("Максимальная длина имени 80 символов")
            .Must((cm, _) => repository.GetUser(cm.Login, cm.Password) != null)
            .When(m => m.Password.Length <= 80 && !m.Password.Contains(' '))
            .WithMessage("Пользователь с таким логином/паролем не найден")
            .OverridePropertyName(nameof(CredentialsModel.Login));

        RuleFor(m => m.Password)
            .Must(m => !m.Contains(' '))
            .WithMessage("Пароль не должен содержать пробелы")
            .MaximumLength(80)
            .WithMessage("Максимальная длина пароля 80 символов")
            .OverridePropertyName(nameof(CredentialsModel.Password));
    }
}