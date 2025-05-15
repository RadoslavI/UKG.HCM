using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using UKG.HCM.UI.JWT;
using UKG.HCM.UI.Services;
using UKG.HCM.UI.Services.Interfaces;

namespace UKG.HCM.UI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        
        // Configure Authentication
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

        // Configure HttpClients from appsettings.json
        var authApiBaseUrl = builder.Configuration["ApiEndpoints:AuthenticationApi:BaseUrl"];
        var peopleApiBaseUrl = builder.Configuration["ApiEndpoints:PeopleManagementApi:BaseUrl"];

        // Register named HttpClients
        builder.Services.AddHttpClient("AuthApi", client =>
        {
            if (!string.IsNullOrEmpty(authApiBaseUrl))
                client.BaseAddress = new Uri(authApiBaseUrl);
        });

        builder.Services.AddHttpClient("PeopleApi", client =>
        {
            if (!string.IsNullOrEmpty(peopleApiBaseUrl))
                client.BaseAddress = new Uri(peopleApiBaseUrl);
        });
            
        // Register services
        builder.Services.AddSingleton<JwtTokenStore>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IPeopleService, PeopleService>();
        builder.Services.AddHttpContextAccessor();
        
        // Configure JSON serialization options
        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
        });
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}