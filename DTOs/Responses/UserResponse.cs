using TestMaster.Entities;
using TestMaster.Enums;

namespace TestMaster.DTOs.Responses;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FName { get; set; } = "";
    public string LName { get; set; } = "";
    public string Email { get; set; } = "";
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }

    private UserResponse() { }

    public static UserResponse FromEntity(User user, string protectorKey)
    {
        return new()
        {
            Id = user.Id,
            FName = user.FName!,
            LName = user.LName!,
            Email = CryptographyData.Decrypt(user.Email!, protectorKey),
            Role = user.Role,
            IsActive = user.IsActive
        };
    }
}