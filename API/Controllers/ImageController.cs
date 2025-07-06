using API.Domain.Request.ImageRequest;
using API.Domain.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IProductDetailService _productDetailService;

        public ImageController(IImageService imageService, IProductDetailService productDetailService)
        {
            _imageService = imageService;
            _productDetailService = productDetailService;
        }

        [HttpPost("upload-multiple")]
        [RequestSizeLimit(10_000_000)] // 10MB nếu cần giới hạn
        public async Task<IActionResult> Upload([FromForm] UploadMultipleImagesRequest request)
        {
            var result = await _imageService.UploadMultipleImagesAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateImageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _imageService.UpdateImageAsync(request);
                return Ok(new { Success = success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }



        [HttpPut("set-main/{imageId}")]
        public async Task<IActionResult> SetMain(Guid imageId)
        {
            await _imageService.SetMainImageAsync(imageId);
            return Ok(new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _imageService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _imageService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }

}
