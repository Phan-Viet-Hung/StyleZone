using API.DomainCusTomer.DTOs.CartICustomer;
using API.DomainCusTomer.DTOs.MuangayCustomer;
using API.DomainCusTomer.DTOs.ThanhToanCustomer;
using API.DomainCusTomer.Request.GHN;
using API.DomainCusTomer.Request.MuaNgay;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Http;          // Thêm using
using System.Threading.Tasks;   // Thêm using
using System;                   // Thêm using
using System.Collections.Generic; // Thêm using
using System.Net.Http.Json;     // Thêm using
using Microsoft.AspNetCore.Http;  // Thêm using

namespace MVC.Controllers
{
    public class ThanhToanCustomerController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string CookieCartKey = "CustomerCart";

        // ===== SỬA CONSTRUCTOR =====
        public ThanhToanCustomerController(IHttpClientFactory httpClientFactory)
        {
            // 1. Sử dụng client "ApiClient" đã được cấu hình trong Program.cs
            _httpClient = httpClientFactory.CreateClient("ApiClient");

            // 2. Xóa bỏ các dòng gán "localhost" và header (đã được cấu hình trong factory)
            // _httpClient.BaseAddress = new Uri("https://localhost:7257/api/");
            // _httpClient.DefaultRequestHeaders.Accept.Add(
            //     new MediaTypeWithQualityHeaderValue("application/json"));
        }
        // ===========================

        [HttpPost]
        public async Task<IActionResult> AddMuaNgay(MuaNgayCustomerRequest request)
        {
            string username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];
            if (!string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("CartCustomer/addmua-ngay", content);

                if (!response.IsSuccessStatusCode)
                {
                    // ===== SỬA ĐOẠN NÀY =====
                    // Đọc nội dung lỗi thực sự từ API gửi về
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // Nếu API trả về lỗi 404 (Not Found) -> Sai đường dẫn
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return BadRequest("Lỗi 404: Sai đường dẫn API (CartCustomer/addmua-ngay).");
                    }

                    // Nếu API trả về lỗi 500 -> Lỗi Code Server (thường là thiếu Session)
                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        return BadRequest("Lỗi 500: Server API gặp sự cố (Kiểm tra lại cấu hình Session).");
                    }

                    // Trả về thông báo lỗi gốc từ API (ví dụ: "Sản phẩm hết hàng", "Yêu cầu không hợp lệ")
                    return BadRequest($"Lỗi API: {errorContent}");
                }

                return RedirectToAction("IndexMuaNgay", "ThanhToanCustomer");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ MVC: {ex.Message}");
            }
        }


        // ========== TẠO ĐƠN HÀNG ==========

        [HttpGet]
        public async Task<IActionResult> IndexMuaNgay()
        {
            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync("CartCustomer/currentmua-ngay");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không có sản phẩm mua ngay.";
                    return RedirectToAction("Index", "Home");
                }
                var item = await response.Content.ReadFromJsonAsync<MuangaycustomerDto>();
                return View(item);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi ngoại lệ: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<decimal> GetShippingFeeAsync(ShippingFeeRequest request)
        {
            try
            {
                // URL tương đối
                var response = await _httpClient.PostAsJsonAsync("shipping/calculate-fee", request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                return json.GetProperty("total_fee").GetDecimal();
            }
            catch (Exception)
            {
                // Trả về một giá trị lỗi (ví dụ: -1) để client JavaScript biết
                return -1;
            }
        }

        public async Task<IActionResult> IndexThanhToan()
        {
            string username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (!string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            // Phần này không gọi API, chỉ đọc cookie nên không cần sửa
            if (Request.Cookies.TryGetValue(CookieCartKey, out var json))
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartCustomerDto>>(json);
                return View(cartItems);
            }
            return View(new List<CartCustomerDto>());
        }
    }
}