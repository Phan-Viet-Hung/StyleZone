using StyleZone_API.Domain.DTOs;
using StyleZone_API.Domain.Request.VoucherRequest;

namespace StyleZone_API.Domain.Service.IService
{
    public interface IVoucherService
    {
        Task<List<VoucherDto>> GetAllAsync();
        Task<VoucherDto?> GetByIdAsync(Guid id);
        Task<VoucherDto?> CreateAsync(CreateVoucherRequest request);
        Task<VoucherDto?> UpdateAsync(UpdateVoucherRequest request);
    }
}
