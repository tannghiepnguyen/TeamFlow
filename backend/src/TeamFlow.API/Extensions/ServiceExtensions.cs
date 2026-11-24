using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeamFlow.API.Options;
using TeamFlow.Infrastructure.Persistence;
using TeamFlow.Infrastructure.Persistence.Configurations;

namespace TeamFlow.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddOptions<ConnectionStringOptions>()
            .Bind(configuration.GetSection("ConnectionStrings"))
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.ConnectionString),
                "Set ConnectionStrings:TeamFlow or ConnectionStrings:DefaultConnection to connect to PostgreSQL.");

        services.AddOptions<PersistenceOptions>()
            .Bind(configuration.GetSection("Persistence"));

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var connection = serviceProvider.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;
            var persistence = serviceProvider.GetRequiredService<IOptions<PersistenceOptions>>().Value;
            options.UseNpgsql(connection.ConnectionString);

            if (environment.IsDevelopment() && persistence.SeedDemoData)
            {
                DemoDataConfiguration.Configure(options);
            }
        });
    }
}
