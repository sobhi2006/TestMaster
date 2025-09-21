namespace TestMaster.Entities;

public class AssignedTest
{
    public Guid Id { get; set; }
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public Guid AssignedUserId { get; set; }
    public Guid TestId { get; set; }
    public Test Test { get; set; } = null!;
    public DateTime AssignedAt{ get; set; }
}