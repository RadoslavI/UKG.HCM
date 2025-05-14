using Microsoft.EntityFrameworkCore;
using UKG.HCM.PeopleManagementApi.Data.Entities;

namespace UKG.HCM.PeopleManagementApi.Data;

public class PeopleContext(DbContextOptions<PeopleContext> options) : DbContext(options)
{
    public DbSet<Person> People => Set<Person>();
}