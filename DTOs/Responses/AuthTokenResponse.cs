
using TestMaster.Entities;

namespace TestMaster.DTOs.Responses;

public class AuthTokenResponse
{
    public string? AccessToken { get; set; }
    public DateTime Expired { get; set; }
    public string? RefreshToken { get; set; }

    public static AuthTokenResponse FromEntity(User user, string Key)
    {
        return new()
        {
            RefreshToken = CryptographyData.Decrypt(user.RefreshToken!, Key),
            Expired = user.RefreshTokenExpiryTime
        };
    }
}