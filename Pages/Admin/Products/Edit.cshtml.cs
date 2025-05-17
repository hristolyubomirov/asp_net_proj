using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CandyShopp.Data;
using CandyShopp.Models;
using System.IO;

namespace CandyShopp.Pages.Admin.Products
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public List<(string Url, string DisplayName)> AvailableImages { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            Product = product;

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

            if (!string.IsNullOrEmpty(Product.ImageUrl) && Product.ImageUrl.StartsWith("data:image"))
            {
                var base64Data = Product.ImageUrl.Split(',')[1];
                var imageBytes = Convert.FromBase64String(base64Data);
                
                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = Path.Combine(_environment.WebRootPath, "images", "products", fileName);
                
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                
                Product.ImageUrl = $"/images/products/{fileName}";
            }

            _context.Attach(Product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
} 