using Microsoft.EntityFrameworkCore;

namespace DAL;

public class UnoDbContext: DbContext
{
    public DbSet<Entities.Database.Game> Games { get; set; } = default!;
    public DbSet<Entities.Database.Player> Players { get; set; } = default!;

    public UnoDbContext(DbContextOptions options) : base(options)
    {
    }

}