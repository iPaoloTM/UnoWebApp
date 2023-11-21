using Entities.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorWebApp.Pages.NewGame;

public class NewGame : PageModel
{
    [BindProperty]
    public int plyrNumber { get; set; } = default!;
    
    public void OnGet()
    {
        
    }
}