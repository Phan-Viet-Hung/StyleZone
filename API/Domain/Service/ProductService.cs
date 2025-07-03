using API.Domain.Extentions;
using API.Domain.Request.ProductDetaiRequest;
using API.Domain.Request.ProductRequest;
using API.Domain.Service.IService;
using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Domain.Service
{
    public class ProductService : IProductService
    {
        private readonly DbContextApp _context;
        public ProductService(DbContextApp context)
        {
            _context = context;
        }

        public async Task<ProductDto> CreateAsync(CreateProductRequest request, Guid createdBy)
        {

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.ToDto();
        }

        public async Task<ProductDto> UpdateAsync(UpdateProductRequest request, Guid updatedBy)
        {
            var product = await _context.Products.FindAsync(request.Id);
            if (product == null)
                throw new Exception("Sản phẩm không tồn tại.");

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.UpdatedAt = DateTime.Now;
            product.UpdatedBy = updatedBy;

            await _context.SaveChangesAsync();
            return product.ToDto();
        }


        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var p = await _context.Products.FindAsync(id);
            return p?.ToDto();
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products.Select(p => p.ToDto()).ToList();
        }
    }
}
