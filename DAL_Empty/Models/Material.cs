using System.ComponentModel.DataAnnotations;

namespace DAL_Empty.Models
{
    public class Material
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Tên chất liệu không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên chất liệu không được vượt quá 100 ký tự.")]
        public string? Name { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
    }
}
