namespace API.Domain.DTOs
{
    public class VoucherDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public string DiscountType { get; set; }
        public string DiscountTypeText { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; } = 1000;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxUsagePerCustomer { get; set; } = 1;
        public int? TotalUsageLimit { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
