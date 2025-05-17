using System.ComponentModel.DataAnnotations;

namespace CandyShopp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Име")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цената трябва да е по-голяма от 0")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Количество на стоката")]
        public int StockQuantity { get; set; }

        [Display(Name = "Изображение")]
        public string ImageUrl { get; set; } = string.Empty;

        public List<string> AdditionalImages { get; set; } = new List<string>();
    }
} 