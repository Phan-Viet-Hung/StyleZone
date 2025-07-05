using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;
using StyleZone_API.Domain.DTOs;
using StyleZone_API.Domain.Extensions;
using StyleZone_API.Domain.Request.VoucherRequest;
using StyleZone_API.Domain.Service.IService;

namespace StyleZone_API.Domain.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly DbContextApp _context;

        public VoucherService(DbContextApp context)
        {
            _context = context;
        }

        public async Task<VoucherDto> CreateAsync(CreateVoucherRequest request)
        {
            if (await _context.Vouchers.AnyAsync(v => v.Code == request.Code))
                throw new Exception("Mã voucher đã tồn tại.");

            if (request.StartDate >= request.EndDate)
                throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");

            if (request.EndDate < DateTime.Now)
                throw new Exception("Ngày kết thúc phải nằm trong tương lai.");

            if (request.StartDate < DateTime.Now.Date)
                throw new Exception("Ngày bắt đầu không được nhỏ hơn ngày hiện tại.");

            if (!Enum.IsDefined(typeof(DiscountType), request.DiscountType))
                throw new Exception("Loại giảm giá không hợp lệ.");

            if (!Enum.IsDefined(typeof(VoucherStatus), request.Status))
                throw new Exception("Trạng thái không hợp lệ.");

            if (request.DiscountType == DiscountType.Percentage && (request.DiscountValue <= 0 || request.DiscountValue > 100))
                throw new Exception("Giá trị phần trăm giảm giá phải trong khoảng từ 0 đến 100.");
            var voucher = new Voucher
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Description = request.Description,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MinOrderAmount = request.MinOrderAmount,
                MaxUsagePerCustomer = request.MaxUsagePerCustomer,
                TotalUsageLimit = request.TotalUsageLimit,
                Status = request.Status,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedAt = DateTime.Now
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return voucher.ToDto();

        }

        public async Task<VoucherDto> UpdateAsync(UpdateVoucherRequest request)
        {
            var voucher = await _context.Vouchers.FindAsync(request.Id);
            if (voucher == null)
                throw new Exception("Voucher không tồn tại.");

            if (request.StartDate >= request.EndDate)
                throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");

            if (request.EndDate < DateTime.Now)
                throw new Exception("Ngày kết thúc phải nằm trong tương lai.");

            if (request.StartDate < DateTime.Now.Date)
                throw new Exception("Ngày bắt đầu không được nhỏ hơn ngày hiện tại.");

            if (!Enum.IsDefined(typeof(DiscountType), request.DiscountType))
                throw new Exception("Loại giảm giá không hợp lệ.");

            if (!Enum.IsDefined(typeof(VoucherStatus), request.Status))
                throw new Exception("Trạng thái không hợp lệ.");

            if (request.DiscountType == DiscountType.Percentage && (request.DiscountValue <= 0 || request.DiscountValue > 100))
                throw new Exception("Giá trị phần trăm giảm giá phải trong khoảng từ 0 đến 100.");

            voucher.Code = request.Code;
            voucher.Description = request.Description;
            voucher.DiscountType = request.DiscountType;
            voucher.DiscountValue = request.DiscountValue;
            voucher.MinOrderAmount = request.MinOrderAmount;
            voucher.MaxUsagePerCustomer = request.MaxUsagePerCustomer;
            voucher.TotalUsageLimit = request.TotalUsageLimit;
            voucher.Status = request.Status;
            voucher.StartDate = request.StartDate;
            voucher.EndDate = request.EndDate;
            voucher.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return voucher.ToDto();

        }

        public async Task<VoucherDto?> GetByIdAsync(Guid id)
        {
            var v = await _context.Vouchers.FindAsync(id);
            if (v == null) return null;
            return v.ToDto();

        }

        public async Task<List<VoucherDto>> GetAllAsync()
        {
            var vouchers = await _context.Vouchers.ToListAsync();

            return vouchers.Select(v => v.ToDto()).ToList();

        }

    }
}
