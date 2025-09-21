using FluentValidation;
using TestMaster.DTOs.Requests.Auth;

namespace TestMaster.Validations.Auth;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(p => p.RefreshToken).NotNull()
                                    .WithMessage("Refresh Token is required")
                                    .NotEmpty()
                                    .WithMessage("Refresh Token can't be empty");

        RuleFor(p => p.AccessToken).NotNull()
                                    .WithMessage("Access Token is required")
                                    .NotEmpty()
                                    .WithMessage("Access Token can't be empty");
    }
}