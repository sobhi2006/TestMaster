using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TestMaster.Data;
using TestMaster.DTOs.Requests.Users;
using TestMaster.DTOs.Responses;
using TestMaster.Entities;
using TestMaster.Enums;
using TestMaster.Exceptions;

namespace TestMaster.Services.UserService;

public class UserService(AppDbContext context, IConfiguration configuration, IMemoryCache memoryCache) : IUserService
{
    private string _keyProtectionData => configuration["KeyDataProtection"]!;
    private string _keyCachingDataU => "Users";

    public async Task<UserResponse> AddNewUserAsync(UserRequest request, Guid? CurrentUserId)
    {
        if (await context.Users.AnyAsync(u => u.Email == CryptographyData.Encrypt(request.Email!, _keyProtectionData)))
            throw new BusinessRuleException("Invalid Email, it is used.", StatusCodes.Status400BadRequest);

        User user = new()
        {
            Id = Guid.NewGuid(),
            FName = request.FName!,
            LName = request.LName!,
            Email = CryptographyData.Encrypt(request.Email!, _keyProtectionData),
            Password = CryptographyData.Encrypt(request.Password!, _keyProtectionData),
            Role = (UserRole)request.Role!,
            RefreshToken = null,
            RefreshTokenExpiryTime = DateTime.MinValue,
            CreatedByUserId = CurrentUserId,
            Age = (int)request.Age!,
            IsActive = (bool)request.IsActive!,
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataU);

        return UserResponse.FromEntity(user, _keyProtectionData);
    }

    public async Task DeleteUserAsync(Guid UserId)
    {
        var User = await context.Users.FirstOrDefaultAsync(u => u.Id == UserId) ??
            throw new BusinessRuleException("User isn't found.", StatusCodes.Status404NotFound);

        context.Users.Remove(User);
        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataU);
    }

    public async Task<UserResponse> GetUserAsync(Guid UserId, UserRole UserRole)
    {
        User User = await context.Users
                            .Where(u => (UserRole.Instructor == UserRole) ? u.Role == UserRole.Student : true)
                            .FirstOrDefaultAsync(u => u.Id == UserId)
                        ?? throw new BusinessRuleException("User is not found", StatusCodes.Status404NotFound);
        return UserResponse.FromEntity(User, _keyProtectionData);
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync(UserRole UserRole, int Page, int PageSize)
    {
        return await memoryCache.GetOrCreate(_keyCachingDataU, async entry =>
        {
            System.Console.WriteLine("From Db :-)");
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.Size = 1;

            List<User> User = await context.Users
                                .Where(u => (UserRole.Instructor == UserRole) ? u.Role == UserRole.Student : true)
                                .Skip((Page - 1) * PageSize).Take(PageSize)
                                .ToListAsync();
            return User.Select(u => UserResponse.FromEntity(u, _keyProtectionData));
        })!;
    }

    public async Task<User> GetUserByEmailPasswordAsync(string Email, string Password)
    {
        var EmailEn = CryptographyData.Encrypt(Email, _keyProtectionData);
        var PasswordEn = CryptographyData.Encrypt(Password, _keyProtectionData);

        User User = await context.Users
                            .FirstOrDefaultAsync(u => u.Email == EmailEn
                                                && u.Password == PasswordEn)
                        ?? throw new BusinessRuleException("User is not found", StatusCodes.Status404NotFound);

        User.Email = CryptographyData.Decrypt(User.Email!, _keyProtectionData);
        User.RefreshToken = User.RefreshToken is not null ? CryptographyData.Decrypt(User.RefreshToken!, _keyProtectionData) : null;
        User.Password = CryptographyData.Decrypt(User.Password!, _keyProtectionData);

        return User;
    }

    public async Task<User> GetUserAsync(Guid UserId)
    {
        var User = await context.Users.FirstOrDefaultAsync(u => u.Id == UserId) ??
            throw new BusinessRuleException("User isn't found.", StatusCodes.Status404NotFound);

        User.Email = CryptographyData.Decrypt(User.Email!, _keyProtectionData);
        User.RefreshToken = User.RefreshToken is not null ? CryptographyData.Decrypt(User.RefreshToken!, _keyProtectionData) : null;
        User.Password = CryptographyData.Decrypt(User.Password!, _keyProtectionData);

        return User;
    }

    public async Task<UserResponse> UpdateUserAsync( User request, Guid? CurrentUserId)
    {
        var User = await context.Users.FirstOrDefaultAsync(u => u.Id == request.Id) ??
            throw new BusinessRuleException("User isn't found.", StatusCodes.Status404NotFound);

        var EmailEn = CryptographyData.Encrypt(request.Email!, _keyProtectionData);

         if (await context.Users.AnyAsync(u => u.Email == EmailEn && u.Id != request.Id))
            throw new BusinessRuleException("Invalid Email, it is used.", StatusCodes.Status400BadRequest);

        User.FName = request.FName;
        User.LName = request.LName;
        User.Email = EmailEn;
        User.Role = (UserRole)request.Role!;

        User.IsActive = request.IsActive!;

        User.Password = CryptographyData.Encrypt(request.Password!, _keyProtectionData);
        User.RefreshToken = (request.RefreshToken != null)? CryptographyData.Encrypt(User.RefreshToken!, _keyProtectionData) : null;
        User.RefreshTokenExpiryTime = request.RefreshTokenExpiryTime;
        User.CreatedByUserId = CurrentUserId;
        User.Age = request.Age!;


        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataU);

        return UserResponse.FromEntity(User, _keyProtectionData);
    }
}