using Demonstration.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demonstration.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<WorkTask> Tasks { get; set; }
        public DbSet<EmployeeTask> UserTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
            });

            modelBuilder.Entity<WorkTask>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(w => w.MaximumParticipants)
                        .HasDefaultValue(10);
                entity.Property(w => w.EnrolledParticipants)
                        .HasDefaultValue(0);
            });

            modelBuilder.Entity<EmployeeTask>(entity =>
            {
                entity.HasKey(et => new { et.UserId, et.TaskId });

                entity.HasOne(et => et.User)
                        .WithMany(e => e.EmployeeTasks)
                        .HasForeignKey(et => et.UserId);

                entity.HasOne(et => et.Task)
                        .WithMany(e => e.EmployeeTasks)
                        .HasForeignKey(et => et.TaskId);
            });
        }
    }
}
