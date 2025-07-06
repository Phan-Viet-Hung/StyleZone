using API.Domain.DTOs;
using API.Domain.Request.ImageRequest;
using API.Domain.Service.IService;
using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Domain.Service
{
    public class ImageService : IImageService
    {
        private readonly DbContextApp _context;
        private readonly IWebHostEnvironment _env;

        public ImageService(DbContextApp context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<ImageDto>> UploadMultipleImagesAsync(UploadMultipleImagesRequest request)
        {
            var result = new List<Image>();
            int total = (request.Files?.Count ?? 0) + (request.Urls?.Count ?? 0);

            if (request.MainImageIndex is not null && (request.MainImageIndex < 0 || request.MainImageIndex >= total))
                throw new Exception("Chỉ số ảnh chính không hợp lệ.");

            int index = 0;

            // Xử lý ảnh từ URL
            if (request.Urls != null)
            {
                foreach (var url in request.Urls)
                {
                    var image = new Image
                    {
                        Id = Guid.NewGuid(),
                        Url = url,
                        FileName = Path.GetFileName(url),
                        ProductDetailId = request.ProductDetailId,
                        IsMainImage = index == request.MainImageIndex,
                        AltText = request.AltTexts?.ElementAtOrDefault(index),
                        CreatedAt = DateTime.Now
                    };
                    result.Add(image);
                    index++;
                }
            }

            // Xử lý ảnh từ File upload
            if (request.Files != null && request.Files.Any())
            {
                // Lấy đường dẫn wwwroot
                var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Tạo thư mục theo ProductDetailId
                var productFolder = Path.Combine(webRootPath, "uploads", "images", request.ProductDetailId.ToString());
                if (!Directory.Exists(productFolder))
                    Directory.CreateDirectory(productFolder);

                foreach (var file in request.Files)
                {
                    var ext = Path.GetExtension(file.FileName);
                    var imageId = Guid.NewGuid(); // tạo trước để dùng làm tên file

                    var fileName = imageId + ext;
                    var filePath = Path.Combine(productFolder, fileName);

                    // Lưu file vào thư mục
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var relativeUrl = $"/uploads/images/{request.ProductDetailId}/{fileName}";

                    var image = new Image
                    {
                        Id = imageId,
                        Url = relativeUrl,
                        FileName = file.FileName,
                        ProductDetailId = request.ProductDetailId,
                        IsMainImage = index == request.MainImageIndex,
                        AltText = request.AltTexts?.ElementAtOrDefault(index),
                        CreatedAt = DateTime.Now
                    };
                    result.Add(image);
                    index++;
                }
            }

            // Nếu có ảnh chính, reset các ảnh cũ về false
            if (request.MainImageIndex != null)
            {
                var oldImages = await _context.Images
                    .Where(x => x.ProductDetailId == request.ProductDetailId && x.IsMainImage)
                    .ToListAsync();

                foreach (var img in oldImages)
                    img.IsMainImage = false;
            }

            _context.Images.AddRange(result);
            await _context.SaveChangesAsync();

            return result.Select(i => new ImageDto
            {
                Id = i.Id,
                Url = i.Url,
                FileName = i.FileName,
                AltText = i.AltText,
                IsMainImage = i.IsMainImage,
                ProductDetailId = i.ProductDetailId,
                CreatedAt = i.CreatedAt
            }).ToList();
        }



        public async Task<bool> UpdateImageAsync(UpdateImageRequest request)
        {
            var image = await _context.Images.FindAsync(request.Id);
            if (image == null)
                throw new Exception("Không tìm thấy ảnh.");

            // Nếu người dùng upload file mới
            if (request.File != null)
            {
                if (image.ProductDetailId == null)
                    throw new Exception("Ảnh chưa được liên kết với ProductDetail nên không thể lưu theo thư mục.");

                var ext = Path.GetExtension(request.File.FileName);
                var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Đường dẫn thư mục theo ProductDetailId
                var folderPath = Path.Combine(webRootPath, "uploads", "images", image.ProductDetailId.ToString());
                Directory.CreateDirectory(folderPath); // tạo nếu chưa có

                // Dùng Id của ảnh làm tên file
                var fileName = image.Id + ext;
                var filePath = Path.Combine(folderPath, fileName);

                // Ghi file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                // Cập nhật Url và Tên File
                var relativeUrl = $"/uploads/images/{image.ProductDetailId}/{fileName}";
                image.Url = relativeUrl;
                image.FileName = request.File.FileName;
            }
            else if (!string.IsNullOrWhiteSpace(request.Url))
            {
                // Người dùng nhập URL thủ công
                image.Url = request.Url;
                image.FileName = Path.GetFileName(request.Url);
            }

            // Cập nhật AltText nếu có
            if (!string.IsNullOrWhiteSpace(request.AltText))
                image.AltText = request.AltText;

            // Cập nhật FileName thủ công nếu có
            if (!string.IsNullOrWhiteSpace(request.FileName))
                image.FileName = request.FileName;

            // Xử lý ảnh chính
            if (request.IsMainImage.HasValue)
            {
                if (request.IsMainImage.Value)
                {
                    var currentMain = await _context.Images
                        .Where(x => x.ProductDetailId == image.ProductDetailId && x.IsMainImage && x.Id != image.Id)
                        .ToListAsync();

                    foreach (var img in currentMain)
                        img.IsMainImage = false;

                    image.IsMainImage = true;
                }
                else
                {
                    image.IsMainImage = false;
                }
            }

            image.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<bool> SetMainImageAsync(Guid imageId)
        {
            var image = await _context.Images.FindAsync(imageId);
            if (image == null)
                throw new Exception("Ảnh không tồn tại.");

            var productId = image.ProductDetailId;
            var images = await _context.Images
                .Where(x => x.ProductDetailId == productId)
                .ToListAsync();

            foreach (var img in images)
                img.IsMainImage = false;

            image.IsMainImage = true;
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<ImageDto>> GetAllAsync()
        {
            return await _context.Images
                .Include(i => i.ProductDetail)
                .Select(i => new ImageDto
                {
                    Id = i.Id,
                    Url = i.Url,
                    FileName = i.FileName,
                    AltText = i.AltText,
                    IsMainImage = i.IsMainImage,
                    ProductDetailId = i.ProductDetailId,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<ImageDto?> GetByIdAsync(Guid id)
        {
            return await _context.Images
                .Include(i => i.ProductDetail)
                .Where(i => i.Id == id)
                .Select(i => new ImageDto
                {
                    Id = i.Id,
                    Url = i.Url,
                    FileName = i.FileName,
                    AltText = i.AltText,
                    IsMainImage = i.IsMainImage,
                    ProductDetailId = i.ProductDetailId,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        private async Task<ImageDto> MapToDto(Guid id)
        {
            return await GetByIdAsync(id) ?? throw new Exception("Không tìm thấy hình ảnh sau khi tạo.");
        }
    }
}
