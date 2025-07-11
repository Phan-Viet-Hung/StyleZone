using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL_Empty.Models
{
    public class PromotionProduct
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Promotion ID là bắt buộc")]
        [ForeignKey("Promotion")]
        public Guid PromotionId { get; set; }

        [Required(ErrorMessage = "Product Detail ID là bắt buộc")]
        [ForeignKey("ProductDetail")]
        public Guid ProductDetailId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceBeforeDiscount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAfterDiscount { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public virtual ProductDetail? ProductDetail { get; set; }

        public virtual Promotion? Promotion { get; set; }
    }
}
