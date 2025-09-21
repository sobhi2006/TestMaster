using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMaster.Entities;

namespace TestMaster.Data.Configurations;

public class QuestionBankConfiguration : IEntityTypeConfiguration<QuestionBank>
{
    public void Configure(EntityTypeBuilder<QuestionBank> builder)
    {
       builder.ToTable("QuestionBanks");

       builder.HasKey(p => p.Id);

       builder.Property(p => p.Title)
               .HasColumnType("NVARCHAR(20)")
               .IsRequired();

       builder.Property(p => p.Description)
               .HasColumnType("NVARCHAR(100)")
               .IsRequired();

       builder.Property(p => p.Description)
               .HasColumnType("NVARCHAR(100)")
               .IsRequired();

       builder.Property(p => p.Difficulty)
               .HasConversion<string>();

       builder.HasOne<User>(qb => qb.CreatedByUser)
              .WithMany(u => u.QuestionBank)
              .HasForeignKey(qb => qb.CreatedByUserId);
    }
}
