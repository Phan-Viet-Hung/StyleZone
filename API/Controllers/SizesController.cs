using API.Domain.Request.SizeRequest;
using API.Domain.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        // GET: api/Sizes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sizes = await _sizeService.GetAllAsync();
            return Ok(sizes);
        }

        // GET: api/Sizes/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var size = await _sizeService.GetByIdAsync(id);
            if (size == null)
                return NotFound(new { message = "Không tìm thấy size." });

            return Ok(size);
        }

        // POST: api/Sizes
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateSizeRequest request)
        {
            try
            {
                var createdSize = await _sizeService.CreateAsync(request.Code, request.Name);
                return CreatedAtAction(nameof(GetById), new { id = createdSize.Id }, createdSize);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Sizes/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateSizeRequest request)
        {
            try
            {
                var updatedSize = await _sizeService.UpdateAsync(id, request.Code, request.Name);
                return Ok(updatedSize);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Không tìm thấy"))
                    return NotFound(new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Sizes/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _sizeService.DeleteAsync(id);
                if (result)
                    return NoContent();
                else
                    return BadRequest(new { message = "Xóa size không thành công." });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Không tìm thấy"))
                    return NotFound(new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}
