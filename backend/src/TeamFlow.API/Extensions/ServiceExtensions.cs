using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeamFlow.API.Options;
using TeamFlow.Infrastructure.Identity;
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
    
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 5;
                o.User.RequireUniqueEmail = true;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }
    
    public static void ConfigureCors(this IServiceCollection services) => services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", builder =>
            builder.WithOrigins("https://localhost:3000")
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("X-Pagination"));
    });
    
    public static void ConfigureGlobalException(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
    
    public static void ConfigureCookie(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }
}
