using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UKG.HCM.AuthenticationApi.Data;
using UKG.HCM.AuthenticationApi.Services;
using UKG.HCM.AuthenticationApi.Services.Interfaces;
using UKG.HCM.Shared.Configuration;
using UKG.HCM.Shared.Constants;

namespace UKG.HCM.AuthenticationApi;

public class Program
{
    private const string AuthDbName = "AuthDb";
    
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
        // Configure EF Core with in-memory database
        builder.Services.AddDbContext<AuthContext>(options => 
            options.UseInMemoryDatabase(AuthDbName));
        
        // Add configuration
        builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWT"));
        
        // Add core services
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.RequireHRAdmin, 
                policy => policy.RequireRole(ApplicationRoles.HRAdmin));
            
            options.AddPolicy(PolicyNames.RequireManagerOrAbove, 
                policy => policy.RequireRole(ApplicationRoles.HRAdmin, ApplicationRoles.Manager));
            
            options.AddPolicy(PolicyNames.RequireAuthenticatedUser,
                policy => policy.RequireAuthenticatedUser());
        });
        
        builder.Services.AddControllers();
        
        // Add application services
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        
        // Add Swagger for API documentation
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }
    
    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        var jwtConfig = builder.Configuration.GetSection("JWT").Get<JwtConfig>();
        if (jwtConfig?.Key == null)
        {
            throw new InvalidOperationException("JWT configuration is missing or invalid");
        }

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });
    }
    
    private static void ConfigureMiddleware(WebApplication app)
    {
        // Configure HTTP pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
    
    private static void SeedDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
        DbSeeder.Seed(context);
    }
}