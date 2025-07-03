using System.ComponentModel.DataAnnotations;

namespace DAL_Empty.Models
{
    public class OrderHistory
    {
        public Guid Id { get; set; }

            
        public Guid? BillId { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        public virtual OrderInfo? Bill { get; set; }
    }
}
