using API.Domain.DTOs;
using API.Domain.Request.ImageRequest;

namespace API.Domain.Service.IService
{
    public interface IImageService
    {
        Task<List<ImageDto>> UploadMultipleImagesAsync(UploadMultipleImagesRequest request);
        Task<bool> UpdateImageAsync(UpdateImageRequest request);
        Task<bool> SetMainImageAsync(Guid imageId);
        Task<List<ImageDto>> GetAllAsync();
        Task<ImageDto?> GetByIdAsync(Guid id);
    }
}
