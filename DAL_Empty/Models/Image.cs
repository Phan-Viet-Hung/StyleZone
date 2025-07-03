using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL_Empty.Models
{
    public class Image
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "URL hình ảnh không được để trống.")]
        [Url(ErrorMessage = "URL hình ảnh không đúng định dạng.")]
        [MaxLength(500, ErrorMessage = "URL hình ảnh không được vượt quá 500 ký tự.")]
        public string? Url { get; set; }
        [Required(ErrorMessage = "ProductDetailId không được để trống.")]
        [ForeignKey("ProductDetail")]
        public Guid? ProductDetailId { get; set; }

        public virtual ProductDetail? ProductDetail { get; set; }
    }
}
