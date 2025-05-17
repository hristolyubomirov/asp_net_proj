using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandyShopp.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        [NotMapped]
        public decimal TotalPrice => Product?.Price * Quantity ?? 0;
    }
} 