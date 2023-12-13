using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL; // Ensure this namespace is correct
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
        
        public IActionResult OnPostLoadGame(Guid gameId)
        {
            var gameState = _gameRepository.LoadGame(gameId);
            // Logic to handle the loaded game
            // Redirect to the game page or display the game state

            return Page();
        }
    }
    
}