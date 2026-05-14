using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Infrastructure.Data;

public class MongoDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Stadium> Stadiums { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Stadium>().ToCollection("stadiums");
    }
}
