using Microsoft.EntityFrameworkCore;
using UKG.HCM.AuthenticationApi.Models;

namespace UKG.HCM.AuthenticationApi.Data;

public class AuthContext : DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
}
