using API.DomainCusTomer.DTOs.Tintuc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System; // Thêm using này
using System.Collections.Generic; // Thêm using này
using System.Net.Http; // Thêm using này
using System.Threading.Tasks; // Thêm using này

namespace MVC.Controllers
{
    public class TinTucCustomerController : Controller
    {
        private readonly HttpClient _httpClient;

        // Bỏ dòng _apiUrl bị hard-code

        // ===== SỬA CONSTRUCTOR =====
        // Tiêm (inject) IHttpClientFactory
        public TinTucCustomerController(IHttpClientFactory httpClientFactory)
        {
            // Yêu cầu Factory tạo ra client tên "ApiClient"
            // (Client này đã được cấu hình BaseAddress trong Program.cs của MVC)
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        // ===========================

        public async Task<IActionResult> Index()
        {
            // ===== SỬA URL =====
            // Sử dụng URL tương đối
            var url = "TinTucCustomer";
            var tinTucs = new List<TinTucDto>();

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    tinTucs = JsonConvert.DeserializeObject<List<TinTucDto>>(json);
                }
                else
                {
                    // Thêm xử lý lỗi
                    var error = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Lỗi khi gọi API: {error}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi ngoại lệ: {ex.Message}";
            }

            return View(tinTucs);
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            // ===== SỬA URL =====
            // Sử dụng URL tương đối
            string detailApiUrl = $"TinTucCustomer/{id}";

            try
            {
                var response = await _httpClient.GetAsync(detailApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var tinTucDetail = JsonConvert.DeserializeObject<TinTucDetailDto>(json);
                    return View(tinTucDetail);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }
    }
}