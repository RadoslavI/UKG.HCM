using Microsoft.EntityFrameworkCore;
using UKG.HCM.AuthenticationApi.Data.Entities;

namespace UKG.HCM.AuthenticationApi.Data;

public class AuthContext : DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
}
