using DAL_Empty.Models;
using System.ComponentModel.DataAnnotations;

namespace StyleZone_API.Domain.Request.VoucherRequest
{
    public class CreateVoucherRequest
    {
        // Ưu tiên chọn 1 trong 2
        public IFormFile? ImageFile { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        [Required(ErrorMessage = "Mã voucher là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Mã voucher không được vượt quá 50 ký tự")]
        public string Code { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        public DiscountType DiscountType { get; set; }

        [Required(ErrorMessage = "Giá trị giảm giá là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải lớn hơn 0")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Số tiền đơn hàng tối thiểu không được âm")]
        public decimal? MinOrderAmount { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lần sử dụng tối đa mỗi khách hàng phải lớn hơn 0")]
        public int? MaxUsagePerCustomer { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Tổng số lần sử dụng phải lớn hơn 0")]
        public int? TotalUsageLimit { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public VoucherStatus Status { get; set; } = VoucherStatus.Active;
    }
}
