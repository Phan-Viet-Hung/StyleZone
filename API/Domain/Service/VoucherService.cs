using API.Domain.DTOs;
using API.Domain.Request.VoucherRequest;
using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;
using StyleZone_API.Domain.Request.VoucherRequest;
using StyleZone_API.Domain.Service.IService;

public class VoucherService : IVoucherService
{
    private readonly DbContextApp _context;
    private readonly IWebHostEnvironment _env;

    public VoucherService(DbContextApp context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<VoucherDto> CreateAsync(CreateVoucherRequest request)
    {
        // Kiểm tra code trùng
        if (await _context.Vouchers.AnyAsync(v => v.Code == request.Code))
            throw new Exception("Mã voucher đã tồn tại.");

        ValidateVoucherData(request);

        var voucherId = Guid.NewGuid();
        string imagePath = await SaveImageAsync(request.ImageFile, request.ImageUrl, voucherId);

        var voucher = new Voucher
        {
            Id = voucherId,
            Code = request.Code,
            Description = request.Description,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            MinOrderAmount = request.MinOrderAmount,
            MaxUsagePerCustomer = request.MaxUsagePerCustomer ?? 0,
            TotalUsageLimit = request.TotalUsageLimit ?? 0,
            Status = request.Status,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAt = DateTime.Now,
            ImageUrl = imagePath
        };

        _context.Vouchers.Add(voucher);
        await _context.SaveChangesAsync();

        return voucher.ToDto();
    }

    public async Task<VoucherDto> UpdateAsync(UpdateVoucherRequest request)
    {
        var voucher = await _context.Vouchers.FindAsync(request.Id);
        if (voucher == null) throw new Exception("Voucher không tồn tại.");

        ValidateVoucherData(request);

        string imagePath = await SaveImageAsync(request.ImageFile, request.ImageUrl, request.Id, isUpdate: true);

        voucher.Code = request.Code;
        voucher.Description = request.Description;
        voucher.DiscountType = request.DiscountType;
        voucher.DiscountValue = request.DiscountValue;
        voucher.MinOrderAmount = request.MinOrderAmount;
        voucher.MaxUsagePerCustomer = request.MaxUsagePerCustomer ?? 0;
        voucher.TotalUsageLimit = request.TotalUsageLimit ?? 0;
        voucher.Status = request.Status;
        voucher.StartDate = request.StartDate;
        voucher.EndDate = request.EndDate;
        voucher.UpdatedAt = DateTime.Now;
        voucher.ImageUrl = imagePath;

        await _context.SaveChangesAsync();

        return voucher.ToDto();
    }

    private void ValidateVoucherData(CreateVoucherRequest request)
    {
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

        if (!string.IsNullOrEmpty(request.ImageUrl) && request.ImageFile != null)
            throw new Exception("Chỉ được chọn 1 trong 2: file ảnh hoặc link ảnh.");
    }

    private async Task<string> SaveImageAsync(IFormFile? file, string? imageUrl, Guid voucherId, bool isUpdate = false)
    {
        if (!string.IsNullOrEmpty(imageUrl))
            return imageUrl;

        if (file == null || file.Length == 0)
            throw new Exception("Ảnh không hợp lệ.");

        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "vouchers");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        string filePath = Path.Combine(uploadsFolder, $"{voucherId}{Path.GetExtension(file.FileName)}");

        if (isUpdate && File.Exists(filePath))
            File.Delete(filePath);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Trả về đường dẫn tương đối
        return $"/uploads/vouchers/{voucherId}{Path.GetExtension(file.FileName)}";
    }

    public async Task<VoucherDto?> GetByIdAsync(Guid id)
    {
        var v = await _context.Vouchers.FindAsync(id);
        return v?.ToDto();
    }

    public async Task<List<VoucherDto>> GetAllAsync()
    {
        var vouchers = await _context.Vouchers.ToListAsync();
        return vouchers.Select(v => v.ToDto()).ToList();
    }
    //public async Task<bool> ToggleStatusAsync(Guid id)
    //{
    //    var voucher = await _context.Vouchers.FindAsync(id);
    //    if (voucher == null)
    //        throw new Exception("Voucher không tồn tại.");

    //    // Chuyển đổi trạng thái giữa Active/InActive
    //    if (voucher.Status == VoucherStatus.Active)
    //        voucher.Status = VoucherStatus.Inactive;
    //    else if (voucher.Status == VoucherStatus.Inactive)
    //        voucher.Status = VoucherStatus.Active;
    //    else
    //        throw new Exception("Không thể thay đổi trạng thái voucher này.");

    //    voucher.UpdatedAt = DateTime.Now;
    //    await _context.SaveChangesAsync();

    //    return true;
    //}

    public async Task<bool> BulkChangeStatusAsync(BulkStatusChangeRequest request)
    {
        var newStatus = request.Status;

        var vouchers = await _context.Vouchers
            .Where(v => request.Ids.Contains(v.Id))
            .ToListAsync();

        foreach (var v in vouchers)
        {
            v.Status = newStatus;
            v.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeStatusAsync(Guid id, VoucherStatus status)
    {
        var voucher = await _context.Vouchers.FindAsync(id);
        if (voucher == null)
            throw new Exception("Voucher không tồn tại.");

        voucher.Status = status;
        voucher.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return true;
    }




}
