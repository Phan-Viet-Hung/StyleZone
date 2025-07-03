using API.Domain.Request.ProductDetaiRequest;
using API.Domain.Request.ProductRequest;

namespace API.Domain.Service.IService
{
    public interface IProductService
    {
        Task<ProductDto> CreateAsync(CreateProductRequest request, Guid createdBy);
        Task<ProductDto> UpdateAsync(UpdateProductRequest request, Guid updatedBy);
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<List<ProductDto>> GetAllAsync();
    }
}
