using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL_Empty.Models
{
    public class OrderPaymentMethod
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Order ID là bắt buộc")]
        [ForeignKey("Order")]
        public Guid? OrderId { get; set; }
        [Required(ErrorMessage = "Payment Method ID là bắt buộc")]
        [ForeignKey("PaymentMethod")]
        public Guid? PaymentMethodId { get; set; }

        public virtual OrderInfo? Order { get; set; }

        public virtual PaymentMethod? PaymentMethod { get; set; }
    }
}
