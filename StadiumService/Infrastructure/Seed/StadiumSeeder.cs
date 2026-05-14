using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Seed;

public sealed class StadiumSeeder(
    MongoDbContext context,
    ILogger<StadiumSeeder> logger)
{
    public async Task SeedAsync()
    {
        if (await context.Stadiums.AnyAsync())
        {
            logger.LogInformation("Stadiums already seeded, skipping.");
            return;
        }

        var stadiums = new List<Stadium>
        {
            Stadium.Create("Lusail Iconic Stadium", "Lusail", 88966),
            Stadium.Create("Al Bayt Stadium", "Al Khor", 60000),
            Stadium.Create("Khalifa International Stadium", "Doha", 45416),
            Stadium.Create("Al Thumama Stadium", "Doha", 40000),
            Stadium.Create("Ahmad Bin Ali Stadium", "Al Rayyan", 44740),
            Stadium.Create("Al Janoub Stadium", "Al Wakrah", 40000),
            Stadium.Create("Education City Stadium", "Al Rayyan", 45350),
            Stadium.Create("Stadium 974", "Doha", 44089)
        };

        context.Stadiums.AddRange(stadiums);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} stadiums.", stadiums.Count);
    }
}
