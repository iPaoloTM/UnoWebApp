using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL;
using Microsoft.AspNetCore.Mvc;

namespace RazorWebApp.Pages.LoadGame
{
    public class LoadGameModel : PageModel
    {
        private readonly GameRepositoryEF _gameRepository;
        public List<(Guid id, DateTime startedAt)> Games { get; set; }
        
        public LoadGameModel(GameRepositoryEF gameRepository)
        {
            _gameRepository = gameRepository;
            Games = new List<(Guid id, DateTime startedAt)>(); // Initialize to an empty list
        }

        public void OnGet()
        {
            Games = _gameRepository.GetSaveGames() ?? new List<(Guid id, DateTime startedAt)>(); // Ensure this is never null
        }
        
        public IActionResult OnPost(Guid gameId)
        {
            return RedirectToPage("../Game/ChoosePlayer", new { GameId = gameId});
        }
    }
    
}