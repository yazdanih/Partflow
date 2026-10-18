
using Microsoft.EntityFrameworkCore;
using PartFlow.Api.Models;

namespace PartFlow.Api.Data;

public class PartFlowDbContext : DbContext
{
    public PartFlowDbContext(DbContextOptions<PartFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Part> Parts => Set<Part>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.PartNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(p => p.PartNumber)
                .IsUnique();

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Status)
                .HasConversion<string>();
        });
    }
}
