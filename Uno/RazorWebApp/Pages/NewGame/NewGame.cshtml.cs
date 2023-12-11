using Entities.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RazorWebApp.Pages.NewGame;

public class NewGame : PageModel
{
    private readonly DAL.UnoDbContext _context;

    public NewGame(DAL.UnoDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        ViewData["GameID"] = new SelectList(_context.Games, "ID", "State");
        return Page();
    }
    
    [BindProperty]
    public int PlyrNumber { get; set; } = default!;
    public Player[] Players { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        foreach (Player p in Players)
        {
            _context.Players.Add(p);
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");

    }
    
}