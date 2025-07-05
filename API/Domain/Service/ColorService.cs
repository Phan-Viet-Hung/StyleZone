using API.Domain.Service.IService;
using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Domain.Service
{
    public class ColorService : IColorService
    {
        private readonly DbContextApp _context;

        public ColorService(DbContextApp context)
        {
            _context = context;
        }

        public async Task<ColorDto> CreateAsync(CreateColorRequest request)
        {
            string name = request.Name.Trim();
            string? code = request.Code?.Trim();

            if (await _context.Colors.AnyAsync(c => c.Name == name || (code != null && c.Code == code)))
                throw new Exception("Màu đã tồn tại với tên hoặc mã trùng.");

            var color = new Color
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code,
                CreatedAt = DateTime.Now
            };

            _context.Colors.Add(color);
            await _context.SaveChangesAsync();

            return color.ToDto();
        }

        public async Task<ColorDto> UpdateAsync(UpdateColorRequest request)
        {
            var color = await _context.Colors.FindAsync(request.Id);
            if (color == null)
                throw new Exception("Không tìm thấy màu cần cập nhật.");

            string newName = request.Name.Trim();
            string? newCode = request.Code?.Trim();

            // Kiểm tra trùng mã/tên với màu khác
            bool isDuplicate = await _context.Colors.AnyAsync(c =>
                c.Id != request.Id && (c.Name == newName || (newCode != null && c.Code == newCode)));

            if (isDuplicate)
                throw new Exception("Đã tồn tại màu khác với mã hoặc tên trùng.");

            color.Name = newName;
            color.Code = newCode;
            color.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return color.ToDto();
        }

        public async Task<List<ColorDto>> GetAllAsync()
        {
            var colors = await _context.Colors.ToListAsync();
            return colors.Select(c => c.ToDto()).ToList();
        }

        public async Task<ColorDto?> GetByIdAsync(Guid id)
        {
            var color = await _context.Colors.FindAsync(id);
            return color?.ToDto();
        }
    }
}
