
using DAL_Empty.Models;
using StyleZone_API.Domain.DTOs;
using StyleZone_API.Domain.Extensions;

namespace API.Domain.Extentions
{
    public static class ProductExtensions
    {
        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                CreatedBy = product.CreatedBy,
                UpdatedBy = product.UpdatedBy,
            };
        }
    }
}
