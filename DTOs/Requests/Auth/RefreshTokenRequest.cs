namespace TestMaster.DTOs.Requests.Auth;

public class RefreshTokenRequest
{
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
}