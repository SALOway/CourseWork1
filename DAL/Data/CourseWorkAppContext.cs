using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DAL.Data;

public class CourseWorkAppContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<AnswerOption> AnswerOptions { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<TestAttempt> TestAttempts { get; set; }
    public DbSet<StudentGroup> StudentGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CourseWorkApp-1;Integrated Security=True;";
        optionsBuilder.UseSqlServer(connectionString);
        optionsBuilder.EnableSensitiveDataLogging(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<StudentGroup>()
            .HasIndex(g => g.Name)
            .IsUnique();
    }

    public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
    {
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.IsDeleted = true;
            baseEntity.DeletedAt = DateTime.UtcNow;

            Entry(entity).State = EntityState.Modified;

            return Entry(entity);
        }

        throw new ArgumentException($"All entites must be of type {typeof(BaseEntity).Name}");
    }

    public override void RemoveRange(IEnumerable<object> entities)
    {
        foreach (EntityEntry<object> entity in entities.Cast<EntityEntry<object>>())
        {
            Remove(entity);
        }
    }
}
