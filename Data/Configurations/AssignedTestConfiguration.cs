using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMaster.Entities;

namespace TestMaster.Data.Configurations;

public class AssignedTestConfiguration : IEntityTypeConfiguration<AssignedTest>
{
    public void Configure(EntityTypeBuilder<AssignedTest> builder)
    {
        builder.ToTable("AssignedTests");

        builder.HasOne<User>(e => e.CreatedByUser)
               .WithMany(u => u.AssignedTests)
               .HasForeignKey(e => e.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Test>(e => e.Test)
               .WithMany(t => t.AssignedTests)
               .HasForeignKey(e => e.TestId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
