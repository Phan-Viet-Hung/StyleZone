using DAL_Empty.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.Request.ProductDetaiRequest
{
    public class CreateProductRequest
    {

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
