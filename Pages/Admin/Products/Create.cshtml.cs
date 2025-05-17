using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CandyShopp.Data;
using CandyShopp.Models;
using System.IO;

namespace CandyShopp.Pages.Admin.Products
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Product Product { get; set; } = new();

        public List<(string Url, string DisplayName)> AvailableImages { get; set; } = new();

        public IActionResult OnGet()
        {
            var imagesPath = Path.Combine(_environment.WebRootPath, "images", "products");
            if (Directory.Exists(imagesPath))
            {
                AvailableImages = Directory.GetFiles(imagesPath)
                    .Select(f => (
                        Url: "/images/products/" + Path.GetFileName(f),
                        DisplayName: Path.GetFileName(f)
                    ))
                    .ToList();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
} 