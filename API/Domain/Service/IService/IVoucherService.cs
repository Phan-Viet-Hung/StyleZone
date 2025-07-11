
using API.Domain.DTOs;
using API.Domain.Request.VoucherRequest;
using DAL_Empty.Models;
using StyleZone_API.Domain.Request.VoucherRequest;

namespace StyleZone_API.Domain.Service.IService
{
    public interface IVoucherService
    {
        Task<List<VoucherDto>> GetAllAsync();
        Task<VoucherDto?> GetByIdAsync(Guid id);
        Task<VoucherDto?> CreateAsync(CreateVoucherRequest request);
        Task<VoucherDto?> UpdateAsync(UpdateVoucherRequest request);
        //Task<bool> ToggleStatusAsync(Guid id);
        Task<bool> BulkChangeStatusAsync(BulkStatusChangeRequest request);
        Task<bool> ChangeStatusAsync(Guid id, VoucherStatus newStatus);

    }
}
