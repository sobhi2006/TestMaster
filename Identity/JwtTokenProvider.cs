using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TestMaster.DTOs.Responses;
using TestMaster.Entities;

namespace TestMaster.Identity;

public class JwtTokenProvider(IConfiguration configuration)
{
    public AuthTokenResponse GenerateToken(User User)
    {
        var JwtSettings = configuration.GetSection("JWT");

        var Claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, User!.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, User.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, User.FName!),
            new Claim(JwtRegisteredClaimNames.FamilyName, User.LName!),
            new Claim("Activate", User.IsActive.ToString().ToLower()!),
            new Claim(ClaimTypes.Role, User.Role.ToString())
        };

        var expired = DateTime.UtcNow.AddMinutes(int.Parse(JwtSettings["ExpiredInMinutes"]!));

        var Descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(Claims),
            Expires = expired,
            Issuer = JwtSettings["Issuer"],
            Audience = JwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings["Key"]!))
                        , SecurityAlgorithms.HmacSha256)
        };

        var TokenHandler = new JwtSecurityTokenHandler();
        var Token = TokenHandler.CreateToken(Descriptor);

        return new AuthTokenResponse
        {
            AccessToken = TokenHandler.WriteToken(Token),
            Expired = expired,
            RefreshToken = GenerateRefreshToken()
        };
    }

    public string GenerateRefreshToken()
    {
        var RandomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(RandomBytes);

        return Convert.ToBase64String(RandomBytes);
    }

    public ClaimsPrincipal? GetClaimsPrincipalFromExpiredToken(string token)
    {
        var TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = configuration["JWT:Issuer"],
            ValidAudience = configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!.ToString())),
            ClockSkew = TimeSpan.Zero
        };

        var TokenHandler = new JwtSecurityTokenHandler();
        var principle = TokenHandler.ValidateToken(token, TokenValidationParameters, out _);

        return principle;
    }
}