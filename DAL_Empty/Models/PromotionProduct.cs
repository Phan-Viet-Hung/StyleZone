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
        public Guid? PromotionId { get; set; }

        [Required(ErrorMessage = "Product Detail ID là bắt buộc")]
        [ForeignKey("ProductDetail")]
        public Guid? ProductDetailId { get; set; }

        public virtual ProductDetail? ProductDetail { get; set; }

        public virtual Promotion? Promotion { get; set; }
    }
}
