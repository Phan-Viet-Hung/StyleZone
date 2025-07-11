using API.Domain.DTOs;
using API.Domain.Request.VoucherRequest;
using DAL_Empty.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StyleZone_API.Domain.Request.VoucherRequest;
using System.Net.Http;
using System.Text;

namespace StyleZone_MVC.Controllers
{
    [Area("Admin")]
    public class VoucherController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VoucherController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string? status, string? keyword, string? sort)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.GetAsync("vouchers");
            if (!response.IsSuccessStatusCode)
                return View(new List<VoucherDto>());

            var content = await response.Content.ReadAsStringAsync();
            var vouchers = JsonConvert.DeserializeObject<List<VoucherDto>>(content) ?? new List<VoucherDto>();

            // Filter theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                vouchers = vouchers
                    .Where(v => v.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter theo từ khóa
            if (!string.IsNullOrEmpty(keyword))
            {
                vouchers = vouchers
                    .Where(v =>
                        (!string.IsNullOrEmpty(v.Code) && v.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(v.Description) && v.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Sắp xếp
            vouchers = sort switch
            {
                "asc" => vouchers.OrderBy(v => v.Code).ToList(),
                "desc" => vouchers.OrderByDescending(v => v.Code).ToList(),
                "dateStart" => vouchers.OrderBy(v => v.StartDate).ToList(),
                "dateEnd" => vouchers.OrderByDescending(v => v.EndDate).ToList(),
                _ => vouchers
            };

            // Gửi lại dữ liệu view
            ViewBag.Status = status;
            ViewBag.Keyword = keyword;
            ViewBag.Sort = sort;

            return View(vouchers);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateVoucherRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("vouchers", httpContent);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, ExtractMessage(error));
            return View(request);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"vouchers/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<VoucherDto>(content);

            if (dto == null) return NotFound();

            var updateModel = new UpdateVoucherRequest
            {
                Id = dto.Id,
                Code = dto.Code,
                Description = dto.Description,
                DiscountType = Enum.TryParse<DiscountType>(dto.DiscountType, out var discountType)
                    ? discountType : throw new Exception("Loại giảm giá không hợp lệ."),
                DiscountValue = dto.DiscountValue,
                MinOrderAmount = dto.MinOrderAmount,
                MaxUsagePerCustomer = dto.MaxUsagePerCustomer,
                TotalUsageLimit = dto.TotalUsageLimit,
                Status = Enum.TryParse<VoucherStatus>(dto.Status, out var status)
                    ? status : throw new Exception("Trạng thái không hợp lệ."),
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            return View(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateVoucherRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("vouchers", httpContent);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, ExtractMessage(error));
            return View(request);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"vouchers/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<VoucherDto>(content);

            return View(dto);
        }
        public async Task<IActionResult> FilterByStatus(string status)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("vouchers");

            if (!response.IsSuccessStatusCode) return Content("");

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<VoucherDto>>(content);

            if (!string.IsNullOrEmpty(status))
                data = data.Where(x => x.Status == status).ToList();

            return PartialView("_VoucherTableBody", data);
        }
        public async Task<IActionResult> Sort(string type)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("vouchers");

            if (!response.IsSuccessStatusCode) return Content("");

            var data = JsonConvert.DeserializeObject<List<VoucherDto>>(await response.Content.ReadAsStringAsync());

            data = type switch
            {
                "asc" => data.OrderBy(x => x.Code).ToList(),
                "desc" => data.OrderByDescending(x => x.Code).ToList(),
                "dateStart" => data.OrderBy(x => x.StartDate).ToList(),
                "dateStartDesc" => data.OrderByDescending(x => x.StartDate).ToList(),
                "dateEnd" => data.OrderBy(x => x.EndDate).ToList(),
                "dateEndAsc" => data.OrderBy(x => x.EndDate).ToList(),  // alias cho clarity
                "dateEndDesc" => data.OrderByDescending(x => x.EndDate).ToList(),
                _ => data
            };

            return PartialView("_VoucherTableBody", data);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsync($"vouchers/{id}/toggle-status", null);
            if (!response.IsSuccessStatusCode) return BadRequest();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BulkChangeStatus([FromBody] BulkStatusChangeRequest req)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonConvert.SerializeObject(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("vouchers/bulk-change-status", content);
            if (!response.IsSuccessStatusCode) return BadRequest();
            return Ok();
        }
        private string ExtractMessage(string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<ErrorResponse>(json);
                return obj?.message ?? "Có lỗi xảy ra.";
            }
            catch
            {
                return "Có lỗi xảy ra.";
            }
        }
        

        private class ErrorResponse
        {
            public string message { get; set; }
        }
    }
}
