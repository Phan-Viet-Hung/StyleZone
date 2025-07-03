using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL_Empty.Models
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Thành phố là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên thành phố không được vượt quá 100 ký tự")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Địa chỉ đường phố là bắt buộc")]
        [StringLength(500, ErrorMessage = "Địa chỉ đường phố không được vượt quá 500 ký tự")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "Tỉnh/Bang là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên tỉnh/bang không được vượt quá 100 ký tự")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên quốc gia không được vượt quá 100 ký tự")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "ID khách hàng là bắt buộc")]
        public Guid? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}
