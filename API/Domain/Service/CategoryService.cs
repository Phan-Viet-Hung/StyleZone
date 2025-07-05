using API.Domain.DTOs;
using API.Domain.Request.CategoryRequest;
using API.Domain.Service.IService;
using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Domain.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly DbContextApp _context;

        public CategoryService(DbContextApp context)
        {
            _context = context;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
        {
            if (await _context.Categories.AnyAsync(c => c.Name == request.Name))
                throw new Exception("Tên danh mục đã tồn tại.");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.Now
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<CategoryDto> UpdateAsync(UpdateCategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(request.Id);
            if (category == null)
                throw new Exception("Danh mục không tồn tại.");

            if (await _context.Categories.AnyAsync(c => c.Name == request.Name && c.Id != request.Id))
                throw new Exception("Tên danh mục đã được sử dụng.");

            category.Name = request.Name;
            category.Description = request.Description;
            category.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var c = await _context.Categories.FindAsync(id);
            return c == null ? null : new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            };
        }
    }
}
