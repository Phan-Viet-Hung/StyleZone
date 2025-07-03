using DAL_Empty.Models;

namespace API.Services
{
    public interface ISupplierService
    {
        Task<List<Supplier>> GetAllAsync();
        Task<Supplier> GetByIdAsync(Guid id);
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
    }
}
