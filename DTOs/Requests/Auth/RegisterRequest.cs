namespace TestMaster.DTOs.Requests.Auth;

public class RegisterRequest
{
    public string? FName { get; set; }
    public string? LName { get; set; }
    public int? Age { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}