using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CandyShopp.Models;
using CandyShopp.Services;

namespace CandyShopp.Pages.Cart
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CartService _cartService;

        public IndexModel(CartService cartService)
        {
            _cartService = cartService;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                CartItems = await _cartService.GetCartItemsAsync(userId);
                CartTotal = await _cartService.GetCartTotalAsync(userId);
            }
        }

        public async Task<IActionResult> OnPostAsync(int productId, int quantity)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _cartService.UpdateCartItemQuantityAsync(userId, productId, quantity);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int productId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _cartService.RemoveFromCartAsync(userId, productId);
            }
            return RedirectToPage();
        }
    }
} 