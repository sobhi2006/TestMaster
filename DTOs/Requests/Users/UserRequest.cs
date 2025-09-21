using TestMaster.Enums;

namespace TestMaster.DTOs.Requests.Users;

public class UserRequest
{
    public string? FName { get; set; }
    public string? LName { get; set; }
    public int? Age { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public UserRole? Role { get; set; }
    public bool? IsActive { get; set; }
}