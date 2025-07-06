using API.Domain.Request.ProductDetaiRequest;
using API.Domain.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/product-details")]
    public class ProductDetailController : ControllerBase
    {
        private readonly IProductDetailService _service;

        public ProductDetailController(IProductDetailService service)
        {
            _service = service;
        }

        // GET: api/product-details?productId=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDetailDto>>> GetAll([FromQuery] Guid? productId)
        {
            var result = await _service.GetAllAsync(productId);
            return Ok(result);
        }


        // GET: api/product-details/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailDto>> GetById(Guid id)
        {
            var detail = await _service.GetByIdAsync(id);
            if (detail == null)
                return NotFound(new { Message = "Chi tiết sản phẩm không tồn tại." });

            return Ok(detail);
        }

        // POST: api/product-details
        [HttpPost]
        public async Task<ActionResult<ProductDetailDto>> Create([FromForm] CreateProductDetailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: api/product-details/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDetailDto>> Update( [FromForm] UpdateProductDetailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (id != request.Id)
            //    return BadRequest(new { Message = "ID không khớp với dữ liệu gửi lên." });

            try
            {
                var updated = await _service.UpdateAsync(request);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // DELETE: api/product-details/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (success)
                    return Ok(new { Message = "Xoá thành công." });

                return NotFound(new { Message = "Chi tiết sản phẩm không tồn tại." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
