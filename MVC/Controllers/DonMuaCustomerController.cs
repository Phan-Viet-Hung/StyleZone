using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DomainCusTomer.DTOs.ThongTinCaNhaCustomer;
using DAL_Empty.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using API.DomainCusTomer.Request.ThongTinCaNhan;
using API.DomainCusTomer.DTOs.QuanLyDonHangCustomerDto;
using System.Net.Http;      // Thêm using
using System.Linq;          // Thêm using
using Microsoft.AspNetCore.Http; // Thêm using

namespace MVC.Controllers
{
    public class DonMuaCustomerController : Controller
    {
        private readonly HttpClient _httpClient;

        // ===== SỬA CONSTRUCTOR =====
        public DonMuaCustomerController(IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> CancelOrder(Guid orderId, string username, string Decription)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];
            if (string.IsNullOrEmpty(username))
            {
                TempData["CancelOrderError"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // URL tương đối
                var response = await _httpClient.PostAsync(
                    $"DonMuaCustomer/cancel/{orderId}?username={username}&Decription={Decription}",
                    null);

                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                string message = null;

                // Tìm property bất kể hoa thường
                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.Name.Equals("Message", StringComparison.OrdinalIgnoreCase))
                    {
                        message = prop.Value.GetString();
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(message))
                    message = "Không nhận được thông báo từ máy chủ.";

                if (response.IsSuccessStatusCode)
                {
                    TempData["CancelOrderSuccess"] = message;
                }
                else
                {
                    TempData["CancelOrderError"] = message;
                }
            }
            catch (Exception ex)
            {
                TempData["CancelOrderError"] = $"Lỗi khi hủy đơn hàng: {ex.Message}";
            }

            return RedirectToAction("ListDonHangPending");
        }


        public async Task<IActionResult> ListDonHang(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            var data = new List<QuanLyDonHangCustomerDto>();
            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"DonMuaCustomer/all/{username}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Lỗi khi tải danh sách đơn hàng.";
                    return View(data);
                }

                var json = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<QuanLyDonHangCustomerDto>>(json);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi ngoại lệ: {ex.Message}";
            }
            return View(data);
        }

        public async Task<IActionResult> ListDonHangPending(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            var data = new List<QuanLyDonHangCustomerDto>();
            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"DonMuaCustomer/pending/{username}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Lỗi khi tải danh sách đơn hàng.";
                    return View(data);
                }

                var json = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<QuanLyDonHangCustomerDto>>(json);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi ngoại lệ: {ex.Message}";
            }
            return View(data);
        }

        public async Task<IActionResult> ListDonHangConfirmed(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            var data = new List<QuanLyDonHangCustomerDto>();
            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"DonMuaCustomer/confirmed/{username}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Lỗi khi tải danh sách đơn hàng.";
                    return View(data);
                }
                var json = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<QuanLyDonHangCustomerDto>>(json);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi ngoại lệ: {ex.Message}";
            }
            return View(data);
        }

        public async Task<IActionResult> ListDonHangProcessing(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            var data = new List<QuanLyDonHangCustomerDto>();
            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"DonMuaCustomer/processing/{username}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Lỗi khi tải danh sách đơn hàng.";
                    return View(data);
                }
                var json = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<QuanLyDonHangCustomerDto>>(json);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi ngoại lệ: {ex.Message}";
            }
            return View(data);
        }

        public async Task<IActionResult> ListDonHangShipping(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            var data = new List<QuanLyDonHangCustomerDto>();
            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"DonMuaCustomer/shipping/{username}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Lỗi khi tải danh sách đơn hàng.";
                    return View(data);
                }
                var json = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<QuanLyDonHangCustomerDto>>(json);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi ngoại lệ: {ex.Message}";
            }
            return View(data);
        }

        public async Task<IActionResult> ListDonHangDelivered(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            var data = new List<QuanLyDonHangCustomerDto>();
            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"DonMuaCustomer/delivered/{username}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Lỗi khi tải danh sách đơn hàng.";
                    return View(data);
                }
                var json = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<QuanLyDonHangCustomerDto>>(json);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi ngoại lệ: {ex.Message}";
            }
            return View(data);
        }

        public async Task<IActionResult> ListDonHangCancelled(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            var data = new List<QuanLyDonHangCustomerDto>();
            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"DonMuaCustomer/cancelled/{username}");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Lỗi khi tải danh sách đơn hàng.";
                    return View(data);
                }
                var json = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<QuanLyDonHangCustomerDto>>(json);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi ngoại lệ: {ex.Message}";
            }
            return View(data);
        }

        // Hàm lấy username dựa trên cookie LoginMethod
        private string GetUsernameFromCookie()
        {
            var loginMethod = Request.Cookies["LoginMethod"];
            if (string.IsNullOrEmpty(loginMethod))
                return null;

            if (loginMethod.Equals("Normal", StringComparison.OrdinalIgnoreCase))
            {
                return Request.Cookies["UserName"];
            }
            else if (loginMethod.Equals("Google", StringComparison.OrdinalIgnoreCase))
            {
                return Request.Cookies["GoogleUserName"];
            }
            return null;
        }


        // GET: /TaiKhoanCuaToi
        public async Task<IActionResult> TaiKhoanCuaToi(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            // URL tương đối
            string apiUrl = $"DonMuaCustomer/{username}";

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể tải thông tin khách hàng.";
                    return View(new UpdateThongTinCaNhanDto());
                }

                var json = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<UpdateThongTinCaNhanDto>(json);

                return View(customer);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi ngoại lệ: {ex.Message}";
                return View(new UpdateThongTinCaNhanDto());
            }
        }

        [HttpPost]
        public async Task<IActionResult> TaiKhoanCuaToi(ThongTinCaNhanRequest updatedCustomer)
        {
            string username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return BadRequest("Không tìm thấy thông tin người dùng.");

            if (!ModelState.IsValid)
            {
                TempData["Errortaikhoancuatoi"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                // Phải trả về DTO chứ không phải Request
                return View(new UpdateThongTinCaNhanDto());
            }

            try
            {
                // URL tương đối
                var response = await _httpClient.PostAsync($"DonMuaCustomer/{username}/update",
                    new StringContent(JsonConvert.SerializeObject(updatedCustomer), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật thông tin thành công.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Có lỗi xảy ra: {error}";
                    // Phải trả về DTO chứ không phải Request
                    return View(new UpdateThongTinCaNhanDto());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi ngoại lệ: {ex.Message}";
                return View(new UpdateThongTinCaNhanDto());
            }
        }

        // Form đổi mật khẩu (GET)
        public IActionResult DoiMatKhau(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");
            return View();
        }

        // Đổi mật khẩu (POST)
        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(RePassDtoCustomer model)
        {
            string username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return BadRequest("Không tìm thấy thông tin người dùng.");

            try
            {
                // URL tương đối
                var response = await _httpClient.PostAsync($"DonMuaCustomer/{username}/change-password", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["errodoimatkhau"] = $"Lỗi: {error}"; // Hiển thị lỗi rõ hơn
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["errodoimatkhau"] = $"Lỗi ngoại lệ: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DiaChi(string username)
        {
            username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");
            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"DonMuaCustomer/{username}/addresses");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể tải địa chỉ.";
                    return View(new List<Address>());
                }
                var json = await response.Content.ReadAsStringAsync();
                var addresses = JsonConvert.DeserializeObject<List<Address>>(json);
                return View(addresses);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<Address>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatusDiaChi(Guid id, string username)
        {
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

            return RedirectToAction("DiaChi");
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
                return RedirectToAction("DiaChi");
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

            return RedirectToAction("DiaChi");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDiaChi(DiachiCustomerDto model, Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = "ID địa chỉ không hợp lệ.";
                return RedirectToAction("DiaChi");
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

            return RedirectToAction("DiaChi");
        }

        [HttpGet]
        public async Task<IActionResult> RemoveDiaChi(Guid ID)
        {
            var username = HttpContext.Request.Cookies["UserName"] ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");

            try
            {
                // URL tương đối
                var response = await _httpClient.GetAsync($"DonMuaCustomer/remove/{ID}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Xóa địa chỉ thành công!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Lỗi khi xóa địa chỉ: " + error;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
            }

            return RedirectToAction("DiaChi");
        }
    }
}