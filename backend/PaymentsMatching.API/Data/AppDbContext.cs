using Microsoft.EntityFrameworkCore;
using PaymentsMatching.API.Entities;

namespace PaymentsMatching.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MatchResult> MatchResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchResult>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OrderId)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(e => e.Currency)
                  .HasMaxLength(10)
                  .IsRequired();

            entity.Property(e => e.SystemAmount)
                  .HasPrecision(18, 2);

            entity.Property(e => e.ProviderAmount)
                  .HasPrecision(18, 2);

            entity.Property(e => e.Status)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(e => e.IsResolved)
                  .HasDefaultValue(false);

            entity.Property(e => e.ResolutionSide)
                  .HasMaxLength(20);

            entity.Property(e => e.CreatedDate)
                  .IsRequired();
        });
    }
}
