using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMaster.Entities;

namespace TestMaster.Data.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.ToTable("Tests");

        builder.HasKey("Id");

        builder.Property(p => p.Title)
               .HasColumnType("NVARCHAR(50)")
               .IsRequired();

        builder.Property(p => p.Description)
               .IsRequired();

        builder.Property(p => p.IsPublished)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(p => p.Duration)
               .IsRequired();

        builder.HasOne<User>(t => t.CreatedByUser)
               .WithMany(u => u.Tests)
               .HasForeignKey(t => t.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.QuestionBankId)
               .IsUnique();

    }
}
