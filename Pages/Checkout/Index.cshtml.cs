using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CandyShopp.Data;
using CandyShopp.Models;
using CandyShopp.Services;
using System.ComponentModel.DataAnnotations;

namespace CandyShopp.Pages.Checkout
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;

        public IndexModel(ApplicationDbContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Името е задължително")]
            [Display(Name = "Име и фамилия")]
            public string CustomerName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Телефонният номер е задължителен")]
            [Phone(ErrorMessage = "Невалиден телефонен номер")]
            [Display(Name = "Телефон")]
            public string PhoneNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Адресът за доставка е задължителен")]
            [Display(Name = "Адрес за доставка")]
            public string DeliveryAddress { get; set; } = string.Empty;

            [Display(Name = "Бележка към поръчката")]
            public string? Note { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            CartItems = await _cartService.GetCartItemsAsync(userId);
            if (!CartItems.Any())
            {
                return RedirectToPage("/Cart/Index");
            }

            CartTotal = await _cartService.GetCartTotalAsync(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (!ModelState.IsValid)
            {
                CartItems = await _cartService.GetCartItemsAsync(userId);
                CartTotal = await _cartService.GetCartTotalAsync(userId);
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    CustomerName = Input.CustomerName,
                    PhoneNumber = Input.PhoneNumber,
                    DeliveryAddress = Input.DeliveryAddress,
                    Note = Input.Note,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending
                };

                var cartItems = await _cartService.GetCartItemsAsync(userId);
                foreach (var cartItem in cartItems)
                {
                    var product = await _context.Products.FindAsync(cartItem.ProductId);
                    if (product == null || product.StockQuantity < cartItem.Quantity)
                    {
                        ModelState.AddModelError(string.Empty, $"Няма достатъчно количество от {cartItem.Product.Name}");
                        await transaction.RollbackAsync();
                        CartItems = cartItems;
                        CartTotal = await _cartService.GetCartTotalAsync(userId);
                        return Page();
                    }

                    product.StockQuantity -= cartItem.Quantity;

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Product.Price
                    });
                }

                order.TotalAmount = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await _cartService.ClearCartAsync(userId);

                await transaction.CommitAsync();

                return RedirectToPage("/Checkout/Success", new { orderId = order.Id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Възникна грешка при обработката на поръчката. Моля, опитайте отново.");
                CartItems = await _cartService.GetCartItemsAsync(userId);
                CartTotal = await _cartService.GetCartTotalAsync(userId);
                return Page();
            }
        }
    }
} 