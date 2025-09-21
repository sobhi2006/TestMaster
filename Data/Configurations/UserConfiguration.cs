using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMaster.Entities;
using TestMaster.Enums;

namespace TestMaster.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
       public void Configure(EntityTypeBuilder<User> builder)
       {
              builder.ToTable("Users");

              builder.HasKey("Id");

              builder.Property(p => p.FName)
                     .HasColumnType("NVARCHAR(100)")
                     .IsRequired();

              builder.Property(p => p.LName)
                     .IsRequired()
                     .HasColumnType("NVARCHAR(100)");

              builder.Property(p => p.Email)
                     .IsRequired()
                     .HasColumnType("NVARCHAR(300)");

              builder.Property(p => p.Password)
                     .IsRequired()
                     .HasColumnType("NVARCHAR(300)");
              builder.Property(p => p.RefreshToken)
                     .IsRequired(false)
                     .HasColumnType("NVARCHAR(300)");
              builder.Property(p => p.IsActive)
                     .IsRequired()
                     .HasDefaultValue(false);

              builder.Property(p => p.Role)
                     .HasConversion<string>()
                     .HasDefaultValue(UserRole.Student);

              builder.Property(p => p.CreatedByUserId)
                     .IsRequired(false);

              builder.HasIndex(p => p.Email)
                     .IsUnique();
       }
}
