using System.Text.Json;
using Domain;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository<string>
{
    private readonly string _filePrefix = "." + System.IO.Path.PathSeparator;

    public string SaveGame(object? id, GameState game)
    {
        var fileName = (string?) id ?? "durak-" + DateTime.Now.ToString() + ".json";
        File.WriteAllText(_filePrefix + fileName, JsonSerializer.Serialize(game));
        return fileName;
    }

    public GameState LoadGame(string id)
    {
        return JsonSerializer.Deserialize<GameState>(
            System.IO.File.ReadAllText(_filePrefix + id)
        )!;
    }
}