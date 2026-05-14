using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Infrastructure.Options;
using Infrastructure.RepositoryImplementations;
using Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MongoDbContext>((sp, options) =>
        {
            var mongoOptions = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            options.UseMongoDB(mongoOptions.ConnectionString, mongoOptions.DatabaseName);
        });
        services.AddScoped<IStadiumRepository, StadiumRepository>();
        services.AddScoped<StadiumSeeder>();
        return services;
    }
}
