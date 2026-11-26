using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using TeamFlow.API;
using TeamFlow.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(options =>
{
    // ASP.NET Core's hosting scope contains the raw request path.
    options.IncludeScopes = false;
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'Z' ";
    options.UseUtcTimestamp = true;
});
builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Warning);

// Add services to the container.

builder.Services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.ConfigureDatabase(builder.Configuration, builder.Environment);
builder.Services.ConfigureCors();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureGlobalException();
builder.Services.ConfigureCookie();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureAntiForgery(builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UsePathBase("/api");
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAntiforgery();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
