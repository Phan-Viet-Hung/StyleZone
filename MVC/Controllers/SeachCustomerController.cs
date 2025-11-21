using API.DomainCusTomer.DTOs.SeachCustomer;
using Microsoft.AspNetCore.Mvc;
using System; // Thêm using
using System.Collections.Generic; // Thêm using
using System.Net.Http; // Thêm using
using System.Net.Http.Json; // Thêm using
using System.Threading.Tasks; // Thêm using

namespace MVC.Controllers
{
    public class SeachCustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public SeachCustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index(string? keyword)
        {
            // ===== SỬA LỖI =====
            // 1. Dùng client "ApiClient" đã được cấu hình trong Program.cs
            var client = _httpClientFactory.CreateClient("ApiClient");

            // 2. Dùng URL tương đối (BaseAddress sẽ được tự động thêm vào)
            string apiUrl = "SeachCustomer";
            // ==================

            if (!string.IsNullOrWhiteSpace(keyword))
                apiUrl += $"?keyword=" + keyword;

            var products = new List<ProductSearchResultDto>();

            try
            {
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    products = await response.Content.ReadFromJsonAsync<List<ProductSearchResultDto>>();
                }
                else
                {
                    // Thêm xử lý lỗi để bạn biết khi API hỏng
                    ViewBag.ErrorMessage = $"Lỗi khi gọi API: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu API không thể kết nối (ví dụ: connection refused)
                ViewBag.ErrorMessage = $"Lỗi ngoại lệ: {ex.Message}";
            }

            return View(products);
        }
    }
}