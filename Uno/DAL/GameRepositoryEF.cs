using Entities;
using System.Text.Json;
using Entities.Database;

namespace DAL;

public class GameRepositoryEF
{
    private readonly UnoDbContext _ctx;

    public GameRepositoryEF(UnoDbContext ctx)
    {
        _ctx = ctx;
    }

    public void Save(Guid id, GameState state)
    {
        //check if game already exists, if yes just update it, if no, null
        var game = _ctx.Games.FirstOrDefault(g => g.Id == state.Id);

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        options.Converters.Add(new JsonConverterUno());
        
        if (game == null)
        {
            game = new Game()
            {
                Id = state.Id,
                GameStateJson = JsonSerializer.Serialize(state, options),
            };
            _ctx.Games.Add(game);
        }
        else
        {
            game.StartedAt = DateTime.Now;
            game.GameStateJson = JsonSerializer.Serialize(state, options);
        }

        
        var changeCount = _ctx.SaveChanges();
        //Console.WriteLine("SaveChanges: " + changeCount);
    }

    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        return _ctx.Games
            .OrderByDescending(g => g.StartedAt)
            .ToList()
            .Select(g => (g.Id, g.StartedAt))
            .ToList();
    }

    public GameState LoadGame(Guid? id)
    {
        var game = _ctx.Games.First(g => g.Id == id);
        
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        options.Converters.Add(new JsonConverterUno());
            
        return JsonSerializer.Deserialize<GameState>(game.GameStateJson, options)!;
    }

}