using UKG.HCM.UI.JWT;

namespace UKG.HCM.UI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        
        builder.Services.AddHttpClient("AuthApi", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7154/");
        });

        builder.Services.AddHttpClient("PeopleApi", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7069/");
        });
        
        builder.Services.AddSingleton<JwtTokenStore>();
        
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

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}