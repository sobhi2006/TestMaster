using TestMaster.Enums;

namespace TestMaster.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FName { get; set; } = string.Empty;
    public string LName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Test> Tests { get; set; } = [];
    public ICollection<Question> Questions { get; set; } = [];
    public ICollection<QuestionBank> QuestionBank { get; set; } = [];
    public ICollection<Evaluation> Evaluations { get; set; } = [];
    public ICollection<AssignedTest> AssignedTests { get; set; } = [];

    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public Guid? CreatedByUserId { get; set; }
}