using Microsoft.EntityFrameworkCore;
using TestMaster.Data.Configurations;
using TestMaster.Entities;

namespace TestMaster.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{

    public DbSet<User> Users => Set<User>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionBank> QuestionBanks => Set<QuestionBank>();
    public DbSet<QuestionBankQuestion> QuestionBanksQuestion => Set<QuestionBankQuestion>();
    public DbSet<Evaluation> Evaluations => Set<Evaluation>();
    public DbSet<AssignedTest> AssignedTests => Set<AssignedTest>();
    public DbSet<AnswerQuestion> Answers => Set<AnswerQuestion>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}