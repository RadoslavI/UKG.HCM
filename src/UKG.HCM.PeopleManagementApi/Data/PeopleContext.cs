using Microsoft.EntityFrameworkCore;
using UKG.HCM.PeopleManagementApi.Models;

namespace UKG.HCM.PeopleManagementApi.Data;

public class PeopleContext(DbContextOptions<PeopleContext> options) : DbContext(options)
{
    public DbSet<Person> People => Set<Person>();
}