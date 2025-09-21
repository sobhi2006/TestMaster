using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestMaster.Entities;

namespace TestMaster.Data.Configurations;

public class QuestionBankQuestionConfiguration : IEntityTypeConfiguration<QuestionBankQuestion>
{
    public void Configure(EntityTypeBuilder<QuestionBankQuestion> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne<QuestionBank>(qbq => qbq.QuestionBank)
               .WithMany(qb => qb.QuestionBankQuestions)
               .HasForeignKey(qbq => qbq.QuestionBankId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Question>(qbq => qbq.Question)
               .WithMany(qb => qb.QuestionBankQuestions)
               .HasForeignKey(qbq => qbq.QuestionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}