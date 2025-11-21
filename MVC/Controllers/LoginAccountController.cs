using API.DomainCusTomer.DTOs.AccountCustomer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using API.DomainCusTomer.Request.AccountCustomerRequest;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;
using API.DomainCusTomer.Request.LoginAccountCustomerRequest;
using DAL_Empty.Models;
using API.DomainCusTomer.Request.Cast;
using API.DomainCusTomer.DTOs.CartICustomer;
using System;                   // Thêm using
using System.Linq;              // Thêm using
using System.Net.Http;          // Thêm using
using System.Net.Http.Json;     // Thêm using
using System.Threading.Tasks;   // Thêm using
using System.Collections.Generic; // Thêm using

namespace MVC.Controllers
{
    public class LoginAccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string CookieCartKey = "CustomerCart";

        // ===== SỬA CONSTRUCTOR =====
        public LoginAccountController(IHttpClientFactory httpClientFactory)
        {
            // 1. Sử dụng client "ApiClient" đã được cấu hình trong Program.cs
            _httpClient = httpClientFactory.CreateClient("ApiClient");

            // 2. Xóa bỏ dòng gán "localhost"
            // _httpClient.BaseAddress = new Uri("https://localhost:7257/api/");
        }
        // ===========================

        // ========== SEND OTP FOR PASSWORD RESET ==========
        [HttpGet]
        public IActionResult SendOtp()
        {
            var username = HttpContext.Request.Cookies["UserName"]
                  ?? HttpContext.Request.Cookies["LoginMethod"];

            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.OtpMessage = "Vui lòng nhập email.";
                return View();
            }

            try
            {
                // URL tương đối
                var response = await _httpClient.PostAsync($"LoginAccountCustomer/send-otp?email={email}", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (responseContent.Contains("Tài khoản chưa tồn tại"))
                    {
                        ViewBag.OtpMessage = "Email chưa tồn tại trong hệ thống.";
                        return View();
                    }
                    ViewBag.OtpMessage = "Không thể gửi OTP. Vui lòng thử lại.";
                    return View();
                }

                HttpContext.Session.SetString("EmailForgot", email);
                return RedirectToAction("ConfirmOtpp", "LoginAccount");
            }
            catch (Exception ex)
            {
                ViewBag.OtpMessage = $"Lỗi kết nối API: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public IActionResult ConfirmOtpp()
        {
            var email = HttpContext.Session.GetString("EmailForgot");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("SendOtp");

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOtpp(OtpCustomerDto model)
        {
            var email = HttpContext.Session.GetString("EmailForgot");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("SendOtp");

            if (string.IsNullOrWhiteSpace(model.OTP))
            {
                ViewBag.ConfirmOtppnull = "Vui lòng nhập OTP.";
                ViewBag.Email = email;
                return View();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ConfirmOtppnull = "Vui lòng nhập đúng mã OTP.";
                ViewBag.Email = email;
                return View();
            }

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                // URL tương đối
                var response = await _httpClient.PostAsync("LoginAccountCustomer/OTP", content);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ConfirmOtpp = "Mã OTP không đúng";
                    ViewBag.Email = email;
                    return View();
                }

                HttpContext.Session.SetString("OtpForgot", model.OTP);
                return RedirectToAction("ResetPassword", "LoginAccount");
            }
            catch (Exception ex)
            {
                ViewBag.ConfirmOtpp = $"Lỗi kết nối API: {ex.Message}";
                ViewBag.Email = email;
                return View();
            }
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var email = HttpContext.Session.GetString("EmailForgot");
            var otp = HttpContext.Session.GetString("OtpForgot");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
                return RedirectToAction("SendOtp");

            ViewBag.Email = email;
            ViewBag.Otp = otp;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgotpasswordCustomerRequest request)
        {
            var email = HttpContext.Session.GetString("EmailForgot");
            var otp = HttpContext.Session.GetString("OtpForgot");

            request.Email = email;
            request.Otp = otp;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(request.NewPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                ViewBag.Email = email;
                ViewBag.Otp = otp;
                return View(request);
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                ViewBag.Error = "Dữ liệu không hợp lệ: " + errors;
                ViewBag.Email = email;
                ViewBag.Otp = otp;
                return View(request);
            }

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // URL tương đối
                var response = await _httpClient.PostAsync("LoginAccountCustomer/reset-password", content);

                if (!response.IsSuccessStatusCode)
                {
                    var apiResult = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = "Đổi mật khẩu thất bại. Chi tiết: " + apiResult;
                    ViewBag.Email = email;
                    ViewBag.Otp = otp;
                    return View(request);
                }

                HttpContext.Session.Remove("EmailForgot");
                HttpContext.Session.Remove("OtpForgot");
                TempData["Message"] = "Đổi mật khẩu thành công.";

                return RedirectToAction("Login", "LoginAccount");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi kết nối API: {ex.Message}";
                ViewBag.Email = email;
                ViewBag.Otp = otp;
                return View(request);
            }
        }


        // ========== SEND OTP FOR REGISTER ==========
        [HttpGet]
        public IActionResult SendOtppRegister()
        {
            var username = HttpContext.Request.Cookies["UserName"]
                  ?? HttpContext.Request.Cookies["LoginMethod"];

            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendOtppRegister(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.OtpMessage = "Vui lòng nhập email.";
                return View();
            }

            try
            {
                // URL tương đối
                var response = await _httpClient.PostAsync($"LoginAccountCustomer/send-otpRegister?email={email}", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (responseContent.Contains("Tài khoản đã tồn tại"))
                    {
                        ViewBag.OtpMessage = "Email đã tồn tại trong hệ thống.";
                        return View();
                    }
                    ViewBag.OtpMessage = "Không thể gửi OTP. Vui lòng thử lại.";
                    return View();
                }

                HttpContext.Session.SetString("EmailRegister", email);
                return RedirectToAction("ConfirmOtppRegister", "LoginAccount");
            }
            catch (Exception ex)
            {
                ViewBag.OtpMessage = $"Lỗi kết nối API: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public IActionResult ConfirmOtppRegister()
        {
            var email = HttpContext.Session.GetString("EmailRegister");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("SendOtppRegister");

            ViewBag.Email1 = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOtppRegister(OtpCustomerDto model)
        {
            var email = HttpContext.Session.GetString("EmailRegister");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("SendOtppRegister");

            if (string.IsNullOrWhiteSpace(model.OTP))
            {
                ViewBag.ConfirmOtppnull = "Vui lòng nhập OTP.";
                ViewBag.Email1 = email;
                return View();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ConfirmOtppnull = "Vui lòng nhập đúng mã OTP";
                ViewBag.Email1 = email;
                return View();
            }

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                // URL tương đối
                var response = await _httpClient.PostAsync("LoginAccountCustomer/OTP", content);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ConfirmOtpp = "Mã OTP không đúng";
                    ViewBag.Email1 = email;
                    return View();
                }

                HttpContext.Session.SetString("OtpVerified", model.OTP);
                return RedirectToAction("Register", "LoginAccount");
            }
            catch (Exception ex)
            {
                ViewBag.ConfirmOtpp = $"Lỗi kết nối API: {ex.Message}";
                ViewBag.Email1 = email;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var email = HttpContext.Session.GetString("EmailRegister");
            var otp = HttpContext.Session.GetString("OtpVerified");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
                return RedirectToAction("SendOtppRegister");

            ViewBag.Email1 = email;
            ViewBag.Otp1 = otp;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisteCustomerRequest request)
        {
            request.Email = HttpContext.Session.GetString("EmailRegister") ?? string.Empty;
            request.Otp = HttpContext.Session.GetString("OtpVerified") ?? string.Empty;

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
            {
                TempData["Message"] = "Email hoặc mã OTP không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("SendOtppRegister");
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                ViewBag.Error = "Dữ liệu không hợp lệ: " + errors;
                ViewBag.Email1 = request.Email;
                ViewBag.Otp1 = request.Otp;
                return View(request);
            }

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // URL tương đối
                var response = await _httpClient.PostAsync("LoginAccountCustomer/register", content);

                if (!response.IsSuccessStatusCode)
                {
                    var apiResult = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = "Đăng ký thất bại hoặc email đã tồn tại. Chi tiết: " + apiResult;
                    ViewBag.Email1 = request.Email;
                    ViewBag.Otp1 = request.Otp;
                    return View(request);
                }

                HttpContext.Session.Remove("EmailRegister");
                HttpContext.Session.Remove("OtpVerified");
                TempData["Message"] = "Đăng ký thành công. Vui lòng đăng nhập.";

                return RedirectToAction("Login", "LoginAccount");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi kết nối API: {ex.Message}";
                ViewBag.Email1 = request.Email;
                ViewBag.Otp1 = request.Otp;
                return View(request);
            }
        }


        private async Task MergeGuestCart(string username)
        {
            var guestCartJson = HttpContext.Request.Cookies[CookieCartKey];
            if (string.IsNullOrEmpty(guestCartJson))
                return;

            try
            {
                var guestCartDto = JsonConvert.DeserializeObject<List<CartCustomerDto>>(guestCartJson) ?? new();
                if (!guestCartDto.Any())
                    return;

                var requests = guestCartDto.Select(x => new CartCustomerRequest
                {
                    ProductDetailcode = x.ProductDetailcode,
                    Quantity = x.Quantity > 0 ? x.Quantity : 1
                }).ToList();

                // URL tương đối
                var response = await _httpClient.PostAsJsonAsync($"CartCustomerID/merge/{username}", requests);

                if (response.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete(CookieCartKey);
                    Console.WriteLine("Merge cart thành công và đã xóa cookie.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Merge cart failed: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error merging cart: {ex.Message}");
            }
        }


        // ========== LOGIN ==========

        [HttpGet]
        public IActionResult Login()
        {
            var username = HttpContext.Request.Cookies["UserName"]
                  ?? HttpContext.Request.Cookies["LoginMethod"];

            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginnCustomerRequest request)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                // URL tương đối
                var response = await _httpClient.PostAsync("LoginAccountCustomer/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = errorMsg;
                    return View(request);
                }

                HttpContext.Response.Cookies.Append("UserName", request.UserName ?? string.Empty, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });

                await MergeGuestCart(request.UserName);
                TempData["Message"] = "Đăng nhập thành công";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi kết nối API: {ex.Message}";
                return View(request);
            }
        }

        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var username = HttpContext.Request.Cookies["UserName"]
                  ?? HttpContext.Request.Cookies["LoginMethod"];

            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                TempData["error"] = "Xác thực Google thất bại.";
                return RedirectToAction("Login");
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                TempData["error"] = "Google không trả về email.";
                return RedirectToAction("Login");
            }

            var request = new LoginGoogleCustomerRequest
            {
                Email = email,
                Name = name,
                UserName = email.Split('@')[0]
            };

            try
            {
                // URL tương đối
                var response = await _httpClient.PostAsJsonAsync("LoginAccountCustomer/LoginGoole", request);

                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Response.Cookies.Append("LoginMethod", request.UserName, new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    });

                    await MergeGuestCart(request.UserName);
                    var customer = await response.Content.ReadFromJsonAsync<Customer>();
                    if (customer != null)
                    {
                        TempData["successs"] = "Đăng nhập Google thành công!";
                    }
                    else
                    {
                        TempData["errorgoogle"] = "Không đọc được dữ liệu trả về từ API.";
                        return RedirectToAction("Login");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    TempData["errorgoogle"] = $"Đăng nhập thất bại: {errorMsg}";
                    return RedirectToAction("Login");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    TempData["errorgoogle"] = $"Yêu cầu không hợp lệ: {errorMsg}";
                    return RedirectToAction("Login");
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    TempData["errorgoogle"] = $"Lỗi hệ thống: {errorMsg}";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                TempData["errorgoogle"] = $"Lỗi kết nối tới API: {ex.Message}";
                return RedirectToAction("Login");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("UserName"))
                Response.Cookies.Delete("UserName");

            if (Request.Cookies.ContainsKey("LoginMethod"))
                Response.Cookies.Delete("LoginMethod");

            return RedirectToAction("Index", "Home");
        }

    }
}