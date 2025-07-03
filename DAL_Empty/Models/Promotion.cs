using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL_Empty.Models
{
    public class Promotion
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Tên khuyến mãi là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên khuyến mãi không được vượt quá 200 ký tự")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        [StringLength(20, ErrorMessage = "Loại giảm giá không được vượt quá 20 ký tự")]
        [RegularExpression("^(PERCENTAGE|AMOUNT)$", ErrorMessage = "Loại giảm giá phải là PERCENTAGE hoặc AMOUNT")]
        public string? DiscountType { get; set; }
        [Required(ErrorMessage = "Giá trị giảm giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountValue { get; set; }
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [Column(TypeName = "datetime2")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [Column(TypeName = "datetime2")]
        public DateTime? EndDate { get; set; }
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]

        public string? Description { get; set; }
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [StringLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự")]
        [RegularExpression("^(ACTIVE|INACTIVE|EXPIRED)$", ErrorMessage = "Trạng thái phải là ACTIVE, INACTIVE hoặc EXPIRED")]
        public string? Status { get; set; }

        public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
    
}
}
