using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMaster.Entities;

namespace TestMaster.Data.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<AnswerQuestion>
{
    public void Configure(EntityTypeBuilder<AnswerQuestion> builder)
    {
        builder.ToTable("Answers");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.IsCorrect)
               .HasDefaultValue(false);

        builder.Property(p => p.AnswerText)
               .IsRequired();

        builder.HasIndex(p => p.QuestionId)
               .IsUnique();

              builder.HasOne<Question>(a => a.Question)
                     .WithMany(q => q.Answers)
                     .HasForeignKey(a => a.QuestionId)
                     .OnDelete(DeleteBehavior.Restrict);
    }
}
