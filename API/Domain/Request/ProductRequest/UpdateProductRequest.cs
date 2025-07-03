using API.Domain.Request.ProductDetaiRequest;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.Request.ProductRequest
{
    public class UpdateProductRequest : CreateProductRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
