using System.ComponentModel.DataAnnotations;

namespace CandyShopp.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        [Display(Name = "Име")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Телефонът е задължителен")]
        [Phone(ErrorMessage = "Невалиден телефонен номер")]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Адресът е задължителен")]
        [Display(Name = "Адрес за доставка")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Display(Name = "Бележка")]
        public string? Note { get; set; }

        [Display(Name = "Дата на поръчка")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Обща сума")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Статус")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
} 