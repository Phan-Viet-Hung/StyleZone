using DAL_Empty.Models;
using StyleZone_API.Domain.DTOs;

namespace StyleZone_API.Domain.Extensions
{
    public static class VoucherExtensions
    {
        public static VoucherDto ToDto(this Voucher v) => new()
        {
            Id = v.Id,
            Description = v.Description,
            DiscountType = v.DiscountType.ToString(),
            DiscountValue = v.DiscountValue,
            MinOrderAmount = v.MinOrderAmount,
            MaxUsagePerCustomer = v.MaxUsagePerCustomer,
            TotalUsageLimit = v.TotalUsageLimit,
            Status = v.Status.ToString(),
            StartDate = v.StartDate,
            EndDate = v.EndDate,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt
        };
    }
}
