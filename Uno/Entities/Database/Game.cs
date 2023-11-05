namespace Entities.Database;

public class Game : BaseEntity
{
    public string GameStateJson { get; set; } = default!;
    public DateTime StartedAt { get; set; } = DateTime.Now;
}