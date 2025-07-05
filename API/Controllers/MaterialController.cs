using API.Domain.Request.MaterialRequest;
using API.Domain.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace StyleZone_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        // GET: api/Material
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var materials = await _materialService.GetAllAsync();
            return Ok(materials);
        }

        // GET: api/Material/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var material = await _materialService.GetByIdAsync(id);
            if (material == null)
                return NotFound(new { message = "Không tìm thấy chất liệu." });

            return Ok(material);
        }

        // POST: api/Material
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateMaterialRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _materialService.CreateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Material
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateMaterialRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _materialService.UpdateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
