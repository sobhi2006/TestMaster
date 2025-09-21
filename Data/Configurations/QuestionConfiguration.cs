using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMaster.Entities;

namespace TestMaster.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
       builder.ToTable("Questions");

       builder.Property(p => p.QuestionText)
               .HasColumnType("NVARCHAR(MAX)")
               .IsRequired();

       builder.Property(p => p.Topic)
               .HasConversion<string>();

       builder.Property(p => p.Difficulty)
               .HasConversion<string>();

       builder.Property(p => p.Type)
               .HasConversion<string>();

       builder.HasOne<User>(q => q.CreatedByUser)
               .WithMany(u => u.Questions)
               .HasForeignKey(q => q.CreatedByUserId);
    }
}
