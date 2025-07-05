using API.Domain.Request.OriginRequest;
using API.Domain.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OriginController : ControllerBase
    {
        private readonly IOriginService _service;

        public OriginController(IOriginService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound("Không tìm thấy xuất xứ.");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOriginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOriginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateAsync(request);
            return Ok(result);
        }
    }

}
