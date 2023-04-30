using System.Security.Claims;
using Domain.Enums;
using Domain.Models;
using DTO;
using LdapConnector;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

namespace AuthServer.Auth;

public class UserValidator : IResourceOwnerPasswordValidator
{
    private readonly CredentialsValidator _validator;
    private readonly ILdapRepository _repository;

    public UserValidator(CredentialsValidator validator, ILdapRepository repository)
    {
        _validator = validator;
        _repository = repository;
    }
    public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        
        var username = context.UserName;
        var password = context.Password;
        var result = _validator.Validate(new CredentialsModel()
        {
            Login = username,
            Password = password
        });

        if (result.IsValid)
        {
            var user = _repository.GetUser(username, password);
            var role = user.Role.ToString();
            context.Result = new GrantValidationResult(
                subject: username,
                authenticationMethod: "custom",
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.UserData, user.LdapId.ToString())
                });
        }
        else
        {
            var errors = result.Errors.Select(e => new ErrorDto
            {
                FieldName = e.PropertyName,
                Message = e.ErrorMessage
            });

            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                customResponse: new Dictionary<string, object>()
                {
                    {"Errors", errors}
                });
        }

        return Task.FromResult(0);
    }
}