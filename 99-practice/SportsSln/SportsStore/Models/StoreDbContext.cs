using Microsoft.EntityFrameworkCore;

namespace SportsStore.Models;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
    {
        Console.WriteLine(Environment.GetEnvironmentVariable("POSTGRES_PASSWD"));
    }

    public DbSet<Product> Products => Set<Product>();
}