using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DAL;

public class UnoDbContextFactory : IDesignTimeDbContextFactory<UnoDbContext>
{
    public UnoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UnoDbContext>();
        optionsBuilder.UseSqlite("");
        
        return new UnoDbContext(optionsBuilder.Options);
    }
}