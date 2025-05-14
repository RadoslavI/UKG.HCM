using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UKG.HCM.PeopleManagementApi.Configuration;
using UKG.HCM.PeopleManagementApi.Data;
using FluentValidation.AspNetCore;
using UKG.HCM.PeopleManagementApi.Services;
using UKG.HCM.PeopleManagementApi.Services.Interfaces;

namespace UKG.HCM.PeopleManagementApi;

/// <summary>
/// Application entry point and configuration
/// </summary>
public class Program
{
    private const string BearerSecurityScheme = "Bearer";
    private const string PeopleDbName = "PeopleDb";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);
        ConfigureAuthentication(builder);

        var app = builder.Build();
        ConfigureMiddleware(app);
        SeedDatabase(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<PeopleContext>(options =>
            options.UseInMemoryDatabase(PeopleDbName));

        // Core services
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        // Application services
        builder.Services.AddScoped<IPeopleService, PeopleService>();
        builder.Services.AddHttpClient<IAuthService, AuthService>();

        // Validation
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        // API Documentation
        ConfigureSwagger(builder);
    }

    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition(BearerSecurityScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter JWT token",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = BearerSecurityScheme
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = BearerSecurityScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        var jwtConfig = builder.Configuration.GetSection("JWT").Get<JwtConfig>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static void SeedDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeopleContext>();
        DbSeeder.Seed(db);
    }
}