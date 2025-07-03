using System.ComponentModel.DataAnnotations;

namespace API.Domain.Request.ProductDetaiRequest
{
    public class UpdateProductDetailRequest : CreateProductDetailRequest
    {
        public Guid Id { get; set; } 
    }
}
