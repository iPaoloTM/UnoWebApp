using Entities;
using Microsoft.EntityFrameworkCore;
using Player = Entities.Database.Player;

namespace DAL;

public class TestDB
{
    public static void testingdb()
    {
        var contextOptions = new DbContextOptionsBuilder<UnoDbContext>()
            .UseSqlite("Data Source=app.db")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

            using var db = new UnoDbContext(contextOptions);
            db.Database.Migrate();
        
            db.Players.Add(new Player() { Nickname = "Andres", Type = EPlayerType.Human });
            var count = db.SaveChanges();
            Console.WriteLine("{0} records saved to database", count);

            Console.WriteLine();
            Console.WriteLine("All entries in database:");
            foreach (var entry in db.Players)
            {
            Console.WriteLine($" - {entry.Nickname} {entry.Type}");
            }
    }
}