using API.Domain.Request.VoucherRequest;
using DAL_Empty.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StyleZone_API.Domain.Request.VoucherRequest;
using StyleZone_API.Domain.Service.IService;

namespace StyleZone_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateVoucherRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _voucherService.CreateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromForm] UpdateVoucherRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _voucherService.UpdateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _voucherService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _voucherService.GetAllAsync();
            return Ok(result);
        }
        //[HttpPost("{id}/toggle-status")]
        //public async Task<IActionResult> ToggleStatus(Guid id)
        //{
        //    try
        //    {
        //        var result = await _voucherService.ToggleStatusAsync(id);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}
        [HttpPost("{id}/change-status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromForm] ChangeStatusRequest request)
        {
            try
            {
                // Chuyển enum sang string khi gọi service nhận string
                var result = await _voucherService.ChangeStatusAsync(id, request.Status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpPost("bulk-change-status")]
        public async Task<IActionResult> BulkChangeStatus([FromForm] BulkStatusChangeRequest request)
        {
            try
            {
                var result = await _voucherService.BulkChangeStatusAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
