using API.Domain.DTOs;
using DAL_Empty.Models;

public static class VoucherExtensions
{
    public static VoucherDto ToDto(this Voucher v) => new()
    {
        Id = v.Id,
        Code = v.Code,
        Description = v.Description,
        ImageUrl = v.ImageUrl,
        DiscountType = v.DiscountType.ToString(),
        DiscountTypeText = v.DiscountType switch
        {
            DiscountType.Percentage => "Phần trăm",
            DiscountType.FixedAmount => "Số tiền cố định",
            _ => "Không xác định"
        },
        DiscountValue = v.DiscountValue,
        MinOrderAmount = v.MinOrderAmount = 1000,
        MaxUsagePerCustomer = v.MaxUsagePerCustomer = 1,
        TotalUsageLimit = v.TotalUsageLimit,
        Status = v.Status.ToString(),
        StatusText = v.Status switch
        {
            VoucherStatus.Active => "Đang áp dụng",
            VoucherStatus.Inactive => "Chưa áp dụng",
            VoucherStatus.Expired => "Hết hạn",
            _ => "Không xác định"
        },
        StartDate = v.StartDate,
        EndDate = v.EndDate,
        CreatedAt = v.CreatedAt,
        UpdatedAt = v.UpdatedAt
    };

}
