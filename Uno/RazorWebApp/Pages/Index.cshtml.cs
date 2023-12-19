using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorWebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly UnoDbContext _ctx;
    private readonly GameRepositoryEF _gameRepository;
    public IndexModel(ILogger<IndexModel> logger, UnoDbContext context)
    {
        _logger = logger;
        _ctx = context;
        _gameRepository = new GameRepositoryEF(_ctx);
    }

    public void OnGet()
    {
    }
}