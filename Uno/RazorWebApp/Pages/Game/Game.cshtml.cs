using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorWebApp.Pages.Game;

public class Game : PageModel
{
    private readonly DAL.UnoDbContext _context;

    private readonly GameRepositoryEF _gameRepository = default!;

    public Game(UnoDbContext context)
    {
        _context = context;
        _gameRepository = new GameRepositoryEF(_context);
    }
    
    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public Guid PlayerId { get; set; }
    
    public void OnGet()
    {
        
    }
}