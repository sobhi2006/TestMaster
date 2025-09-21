using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMaster.Entities;

namespace TestMaster.Data.Configurations;

public class EvaluationConfiguration : IEntityTypeConfiguration<Evaluation>
{
    public void Configure(EntityTypeBuilder<Evaluation> builder)
    {
        builder.ToTable("Evaluations");

        builder.Property(p => p.Feedback)
               .HasColumnType("NVARCHAR(MAX)")
               .IsRequired();

       builder.HasOne<User>(e => e.CreatedByUser)
              .WithMany(u => u.Evaluations)
              .HasForeignKey(e => e.CreatedByUserId)
              .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne<Test>(e => e.Test)
              .WithMany(t => t.Evaluations)
              .HasForeignKey(e => e.TestId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
