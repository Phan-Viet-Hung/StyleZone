using API.Domain.Request.ProductDetaiRequest;

namespace API.Domain.Service.IService
{
    public interface IProductDetailService
    {
        Task<ProductDetailDto> CreateAsync(CreateProductDetailRequest request);
        Task<ProductDetailDto> UpdateAsync(UpdateProductDetailRequest request);
        Task<ProductDetailDto?> GetByIdAsync(Guid id);
        Task<List<ProductDetailDto>> GetAllAsync(Guid? productId = null);
        Task<bool> DeleteAsync(Guid id);
    }
}
