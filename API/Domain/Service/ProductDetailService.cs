using API.Domain.Extentions;
using API.Domain.Request.ProductDetaiRequest;
using API.Domain.Service.IService;
using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;
using StyleZone_API.Domain.Extensions;

namespace API.Domain.Service
{
    public class ProductDetailService : IProductDetailService
    {
        private readonly DbContextApp _context;

        public ProductDetailService(DbContextApp context)
        {
            _context = context;
        }

        public async Task<ProductDetailDto> CreateAsync(CreateProductDetailRequest request)
        {
            // Validate: product must exist
            if (!await _context.Products.AnyAsync(p => p.Id == request.ProductId))
                throw new Exception("Sản phẩm cha không tồn tại.");

            // Validate duplicate
            bool exists = await _context.ProductDetails.AnyAsync(x =>
                x.Name == request.Name &&
                (x.ColorId == request.ColorId || (x.ColorId == null && request.ColorId == null)) &&
                (x.SizeId == request.SizeId || (x.SizeId == null && request.SizeId == null)) &&
                (x.MaterialId == request.MaterialId || (x.MaterialId == null && request.MaterialId == null)) &&
                (x.CategoryId == request.CategoryId || (x.CategoryId == null && request.CategoryId == null)) &&
                (x.BrandId == request.BrandId || (x.BrandId == null && request.BrandId == null)) &&
                (x.OriginId == request.OriginId || (x.OriginId == null && request.OriginId == null)) &&
                (x.SupplierId == request.SupplierId || (x.SupplierId == null && request.SupplierId == null)));

            if (exists)
                throw new Exception("Chi tiết sản phẩm đã tồn tại với thông tin tương tự.");

            var detail = new ProductDetail
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                Quantity = request.Quantity,
                ProductId = request.ProductId,
                ColorId = request.ColorId,
                SizeId = request.SizeId,
                MaterialId = request.MaterialId,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                OriginId = request.OriginId,
                SupplierId = request.SupplierId,
                Status = ProductDetailStatus.Active
            };

            _context.ProductDetails.Add(detail);
            await _context.SaveChangesAsync();
            return detail.ToDto();
        }


        public async Task<ProductDetailDto> UpdateAsync(UpdateProductDetailRequest request)
        {
            var detail = await _context.ProductDetails.FindAsync(request.Id);
            if (detail == null)
                throw new Exception("Chi tiết sản phẩm không tồn tại.");

            detail.Name = request.Name;
            detail.Price = request.Price;
            detail.Quantity = request.Quantity;
            detail.ColorId = request.ColorId;
            detail.SizeId = request.SizeId;
            detail.MaterialId = request.MaterialId;
            detail.CategoryId = request.CategoryId;
            detail.BrandId = request.BrandId;
            detail.OriginId = request.OriginId;
            detail.SupplierId = request.SupplierId;

            await _context.SaveChangesAsync();
            return detail.ToDto();
        }

        public async Task<ProductDetailDto?> GetByIdAsync(Guid id)
        {
            var detail = await _context.ProductDetails
                .Include(p => p.Brand)
                .Include(p => p.Color)
                .Include(p => p.Size)
                .Include(p => p.Material)
                .Include(p => p.Category)
                .Include(p => p.Origin)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);

            return detail?.ToDto();
        }

        public async Task<List<ProductDetailDto>> GetAllAsync()
        {
            var list = await _context.ProductDetails
                .Include(p => p.Brand)
                .Include(p => p.Color)
                .Include(p => p.Size)
                .Include(p => p.Material)
                .Include(p => p.Category)
                .Include(p => p.Origin)
                .Include(p => p.Supplier)
                .ToListAsync();

            return list.Select(p => p.ToDto()).ToList();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var detail = await _context.ProductDetails.FindAsync(id);
            if (detail == null)
                throw new Exception("Chi tiết sản phẩm không tồn tại.");

            _context.ProductDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
