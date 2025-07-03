using System;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.Request.ProductDetaiRequest
{
    public class CreateProductDetailRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int Quantity { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        public Guid? ColorId { get; set; }

        public Guid? SizeId { get; set; }

        public Guid? MaterialId { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? BrandId { get; set; }

        public Guid? OriginId { get; set; }

        public Guid? SupplierId { get; set; }
    }
}
