using Microsoft.EntityFrameworkCore;

namespace ResumeHxApp.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Resume> Resumes { get; set; }
    public DbSet<JobHistory> JobHistories { get; set; }
    public DbSet<JobResponsibility> JobResponsibilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.LinkedInProfile).HasMaxLength(500);
            entity.Property(e => e.Summary).HasMaxLength(5000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<JobHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Location).HasMaxLength(150);
            entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TechStack).HasMaxLength(1000);
            entity.Property(e => e.Summary).HasMaxLength(2000);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne<Resume>()
                  .WithMany(r => r.JobHistories)
                  .HasForeignKey(e => e.ResumeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobResponsibility>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);

            entity.HasOne<JobHistory>()
                  .WithMany(j => j.Responsibilities)
                  .HasForeignKey(e => e.JobHistoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

    }
}
