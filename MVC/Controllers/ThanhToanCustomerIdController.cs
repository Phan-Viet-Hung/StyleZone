using API.DomainCusTomer.DTOs.MuangayCustomer;
using API.DomainCusTomer.Request.GHN;
using API.DomainCusTomer.Request.MuaNgay;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using API.DomainCusTomer.DTOs.CastCustomerId;
using API.DomainCusTomer.DTOs.ThanhToanCustomerId;
using API.DomainCusTomer.DTOs.ThongTinCaNhaCustomer;
using API.DomainCusTomer.DTOs.MuaNgayCustomerID;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Collections.Generic;

namespace MVC.Controllers
{
    public class ThanhToanCustomerIdController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string CookieCartKey = "CustomerCart";

        // ===== SỬA CONSTRUCTOR =====
        public ThanhToanCustomerIdController(IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> AddMuaNgayID(MuaNgayCustomerRequest request, string username)
        {
            // Lấy username từ cookie nếu chưa truyền
            username = string.IsNullOrEmpty(username)
                ? HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"]
                : username;

            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Bạn cần đăng nhập để mua ngay.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Serialize dữ liệu gửi sang API
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // URL tương đối (BaseAddress đã được set trong Factory)
                var response = await _httpClient.PostAsync(
                    $"ThanhToanCustomerId/addmua-ngay?username={username}",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("Số lượng không đủ.");
                }

                return RedirectToAction("IndexMuaNgayID", "ThanhToanCustomerId");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> IndexMuaNgayID(string username)
        {
            // Lấy username từ cookie nếu chưa truyền
            username = string.IsNullOrEmpty(username)
                ? HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"]
                : username;

            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Bạn cần đăng nhập để xem sản phẩm mua ngay.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Gọi API lấy dữ liệu mua ngay (URL tương đối)
                var response = await _httpClient.GetAsync(
                    $"ThanhToanCustomerId/currentmua-ngay?username={username}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không có sản phẩm mua ngay.";
                    return RedirectToAction("Index", "Home");
                }

                var item = await response.Content.ReadFromJsonAsync<MuangaycustomerIdDto>();

                // Đảm bảo AddressList và VoucherList không bị null để tránh NullReferenceException
                item.AddressList ??= new List<AddressDto>();
                item.VoucherList ??= new List<VoucherDto>();

                return View(item);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi ngoại lệ: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListCartthanhtoanId()
        {
            string username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username không được để trống");
            }

            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"ThanhToanCustomerId/{username}");

                if (!response.IsSuccessStatusCode)
                {
                    return View("Error");
                }

                var model = await response.Content.ReadFromJsonAsync<ThanhToanCartIdDto>();

                return View(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ThemDiaChi(DiachiCustomerDto newAddress)
        {
            var username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction("ListCartthanhtoanId");
            }

            try
            {
                // URL tương đối
                var response = await _httpClient.PostAsync(
                    $"DonMuaCustomer/{username}/address",
                    new StringContent(JsonConvert.SerializeObject(newAddress), Encoding.UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Thêm địa chỉ thành công!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Thêm địa chỉ thất bại: {error}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
            }

            return RedirectToAction("ListCartthanhtoanId");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDiaChi(DiachiCustomerDto model, Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = "ID địa chỉ không hợp lệ.";
                return RedirectToAction("ListCartthanhtoanId");
            }

            try
            {
                // URL tương đối
                var response = await _httpClient.PutAsJsonAsync($"DonMuaCustomer/address/{id}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật địa chỉ thành công!";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Lỗi khi cập nhật địa chỉ: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
            }

            return RedirectToAction("ListCartthanhtoanId");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatusDiaChi(Guid id, string username)
        {
            // Lấy username từ cookie
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            try
            {
                // URL tương đối
                var apiUrl = $"DonMuaCustomer/UpdateStatusDiaChi/{username}/{id}";
                var response = await _httpClient.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật địa chỉ mặc định thành công.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Lỗi: {error}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
            }

            return RedirectToAction("ListCartthanhtoanId");
        }
    }
}