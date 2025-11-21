using API.DomainCusTomer.DTOs.MoMo;
using API.DomainCusTomer.DTOs.ThanhToanCustomer;
using API.DomainCusTomer.DTOs.ThanhToanCustomerId;
using API.DomainCusTomer.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System; // Thêm using
using System.Collections.Generic; // Thêm using
using System.Linq; // Thêm using
using System.Net.Http; // Thêm using
using System.Net.Http.Headers; // Thêm using
using System.Net.Http.Json; // Thêm using
using System.Threading.Tasks; // Thêm using
using Microsoft.AspNetCore.Http; // Thêm using
using Newtonsoft.Json; // Thêm using (vì bạn đang dùng cả hai)

namespace MVC.Controllers
{
    public class MoMoThanhtoanCustomerIdController : Controller
    {
        private readonly IMomoCustomerIdServices _momoService;
        private readonly HttpClient _httpClient;

        // ===== SỬA CONSTRUCTOR =====
        public MoMoThanhtoanCustomerIdController(IMomoCustomerIdServices momoService, IHttpClientFactory httpClientFactory)
        {
            _momoService = momoService;
            // 1. Sử dụng client "ApiClient" đã được cấu hình trong Program.cs
            _httpClient = httpClientFactory.CreateClient("ApiClient");

            // 2. Xóa bỏ các dòng gán "localhost" và header (đã được cấu hình trong factory)
            // _httpClient.BaseAddress = new Uri("https://localhost:7257/api/");
            // _httpClient.DefaultRequestHeaders.Accept.Add(
            //     new MediaTypeWithQualityHeaderValue("application/json"));
        }
        // ===========================

        [HttpGet]
        public async Task<IActionResult> PaymentCallBackId()
        {
            var username = HttpContext.Request.Cookies["UserName"]
                  ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Home");
            }

            Console.WriteLine("== MoMo CALLBACK ==\n" + string.Join("\n", Request.Query.Select(q => $"{q.Key} = {q.Value}")));

            if (!_momoService.ValidateSignature(Request.Query))
                return BadRequest("Chữ ký không hợp lệ (Invalid signature)");

            var result = _momoService.PaymentExecuteAsync(Request.Query);
            var jsonData = HttpContext.Session.GetString("MomoOrder");

            // Thêm kiểm tra null cho jsonData ngay lập tức
            if (string.IsNullOrEmpty(jsonData))
                return BadRequest("Không tìm thấy dữ liệu đơn hàng trong session");

            var order = System.Text.Json.JsonSerializer.Deserialize<OrderCustomerIdDto>(jsonData);

            if (result.ErrorCode == "0") // Thanh toán MoMo thành công
            {
                try
                {
                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");

                    // URL tương đối
                    var response = await _httpClient.PostAsync($"ThanhToanCustomerId/create-by-customer-id?username={username}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        if (order.IsFromCart == true)
                        {
                            // URL tương đối
                            var responseremove = await _httpClient.DeleteAsync($"ThanhToanCustomerId/remove-all/{username}");

                            if (!responseremove.IsSuccessStatusCode)
                            {
                                // Ghi log lỗi xóa giỏ hàng nhưng vẫn tiếp tục vì đơn hàng đã thành công
                                var error = await responseremove.Content.ReadAsStringAsync();
                                Console.WriteLine($"Lỗi khi xóa giỏ hàng sau khi thanh toán MoMo: {error}");
                            }
                        }
                        TempData["SuccessMessage"] = "Đặt hàng thành công qua MoMo!";
                        return RedirectToAction("ListDonHangPending", "DonMuaCustomer");
                    }
                    else // Lỗi khi tạo đơn hàng (dù MoMo đã trừ tiền)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var json = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(errorContent);
                        string errorMessage = json != null && json.ContainsKey("message") ? json["message"] : "Lỗi không xác định";

                        TempData["Errormomothanhtoan"] = errorMessage + ". Vui lòng liên hệ cửa hàng để được nhận lại tiền.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Errormomothanhtoan"] = $"Lỗi hệ thống sau khi thanh toán: {ex.Message}. Vui lòng liên hệ cửa hàng.";
                    return RedirectToAction("Index", "Home");
                }
            }
            else // Thanh toán MoMo thất bại
            {
                TempData["Error"] = $"Thanh toán thất bại. Mã lỗi: {result.ErrorCode} - {result.Message}";
                if (order.IsFromCart == false)
                {
                    return RedirectToAction("IndexMuaNgayID", "ThanhToanCustomerId");
                }
                else
                {
                    return RedirectToAction("ListCartthanhtoanId", "ThanhToanCustomerId");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderCustomerId(OrderCustomerIdDto request, string username)
        {
            // Lấy username từ cookie
            username = HttpContext.Request.Cookies["UserName"]
                            ?? HttpContext.Request.Cookies["LoginMethod"];

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // URL tương đối
                var accountResponse = await _httpClient.GetAsync($"LoginAccountCustomer/check-active/{username}");
                if (!accountResponse.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete("UserName");
                    Response.Cookies.Delete("LoginMethod");
                    return RedirectToAction("Index", "Home");
                }

                if (request.ShippingFee == 0)
                {
                    TempData["MessageDiaChi"] = "Vui lòng chọn địa chỉ";
                    if (request.IsFromCart == true)
                        return RedirectToAction("ListCartthanhtoanId", "ThanhToanCustomerId");
                    else
                        return RedirectToAction("IndexMuaNgayID", "ThanhToanCustomerId");
                }

                // ==== Thanh toán Momo ====
                if (request.PaymentMethodCode == "momo")
                {
                    HttpContext.Session.SetString("MomoOrder", System.Text.Json.JsonSerializer.Serialize(request));

                    var paymentResponse = await _momoService.CreatePaymentAsync(new OrderInfoModel
                    {
                        FullName = request.FullName,
                        Amount = (int)request.TotalAmount,
                        OrderInfo = "Thanh toán sản phẩm tại stylezone"
                    });

                    if (paymentResponse != null && !string.IsNullOrEmpty(paymentResponse.PayUrl))
                    {
                        return Redirect(paymentResponse.PayUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Không nhận được PayUrl từ MoMo.");
                        if (request.IsFromCart == true)
                            return RedirectToAction("ListCartthanhtoanId", "ThanhToanCustomerId", request);
                        else
                            return RedirectToAction("IndexMuaNgayID", "ThanhToanCustomerId", request);
                    }
                }

                // ==== Thanh toán COD ====
                if (request.PaymentMethodCode == "cod")
                {
                    // URL tương đối
                    var response = await _httpClient.PostAsJsonAsync(
                        $"ThanhToanCustomerId/create-by-customer-id?username={username}",
                        request
                    );

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var json = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(errorContent);
                        string errorMessage = json != null && json.ContainsKey("message") ? json["message"] : "Lỗi không xác định";

                        TempData["ErroaccountId"] = errorMessage;
                        if (request.IsFromCart == true)
                            return RedirectToAction("ListCartthanhtoanId", "ThanhToanCustomerId", request);
                        else
                            return RedirectToAction("IndexMuaNgayID", "ThanhToanCustomerId", request);
                    }

                    // Nếu đơn được tạo từ giỏ hàng thì xóa giỏ hàng
                    if (request.IsFromCart == true)
                    {
                        // URL tương đối
                        var removeCartResponse = await _httpClient.DeleteAsync($"ThanhToanCustomerId/remove-all/{username}");
                        if (removeCartResponse.IsSuccessStatusCode)
                        {
                            var result = await removeCartResponse.Content.ReadFromJsonAsync<dynamic>();
                            TempData["Message"] = result?.message ?? "Xóa thành công";
                        }
                    }
                    TempData["SuccessMessage"] = "Đặt hàng thành công!";
                    return RedirectToAction("ListDonHangPending", "DonMuaCustomer");
                }

                ModelState.AddModelError(string.Empty, "Phương thức thanh toán không hợp lệ.");
                if (request.IsFromCart == true)
                    return RedirectToAction("ListCartthanhtoanId", "ThanhToanCustomerId", request);
                else
                    return RedirectToAction("IndexMuaNgayID", "ThanhToanCustomerId", request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi không mong muốn: {ex.Message}");
                if (request.IsFromCart == true)
                    return RedirectToAction("ListCartthanhtoanId", "ThanhToanCustomerId", request);
                else
                    return RedirectToAction("IndexMuaNgayID", "ThanhToanCustomerId", request);
            }
        }

        [HttpPost]
        public IActionResult MomoNotifyId([FromForm] IFormCollection collection)
        {
            if (!_momoService.ValidateSignature(collection))
                return BadRequest("Invalid signature");

            var result = _momoService.PaymentExecuteAsync(collection);

            // Cập nhật trạng thái thanh toán trong DB
            if (result.ErrorCode == "0")
            {
                // Thanh toán thành công
                // Ví dụ: _orderService.UpdatePaymentStatus(result.OrderId, true);
            }
            else
            {
                // Thanh toán thất bại
                // _orderService.UpdatePaymentStatus(result.OrderId, false);
            }

            return Ok("success");
        }
    }
}