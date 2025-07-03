using System.ComponentModel.DataAnnotations;

namespace DAL_Empty.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Tên danh mục không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string? Name { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
    }
}
