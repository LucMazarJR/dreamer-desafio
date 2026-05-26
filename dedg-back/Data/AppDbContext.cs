using dedg_back.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dedg_back.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<TimeEvent> TimeEvents { get; set; }
    public DbSet<MonthlyPeriod> MonthlyPeriods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User — auto-referência gestor/subordinado
        modelBuilder.Entity<User>()
            .HasOne(u => u.Manager)
            .WithMany(u => u.Subordinates)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // TimeEvent -> User
        modelBuilder.Entity<TimeEvent>()
            .HasOne(t => t.User)
            .WithMany(u => u.TimeEvents)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // MonthlyPeriod -> User (quem fechou)
        modelBuilder.Entity<MonthlyPeriod>()
            .HasOne(m => m.ClosedBy)
            .WithMany()
            .HasForeignKey(m => m.ClosedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Enums armazenados como string no banco
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<TimeEvent>()
            .Property(t => t.EventType)
            .HasConversion<string>();

        modelBuilder.Entity<MonthlyPeriod>()
            .Property(m => m.Status)
            .HasConversion<string>();
    }
}
