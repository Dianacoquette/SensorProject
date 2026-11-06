using Microsoft.EntityFrameworkCore;
using SensoresApi.Models;

namespace SensoresApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Lectura> Lecturas => Set<Lectura>();
}