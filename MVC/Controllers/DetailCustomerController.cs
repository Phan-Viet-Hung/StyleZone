using API.DomainCusTomer.DTOs;
using API.DomainCusTomer.DTOs.DetailCustomer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json; // Thêm using
using System.Threading.Tasks; // Thêm using
using System; // Thêm using

namespace MVC.Controllers
{
    public class DetailCustomerController : Controller
    {
        private readonly HttpClient _httpClient;

        // ===== SỬA CONSTRUCTOR =====
        // Tiêm (inject) IHttpClientFactory
        public DetailCustomerController(IHttpClientFactory httpClientFactory)
        {
            // Yêu cầu Factory tạo ra client tên "ApiClient"
            // (Client này đã được cấu hình BaseAddress trong Program.cs của MVC)
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        // ===========================

        // GET: DetailCustomerController
        public ActionResult Index()
        {
            return View();
        }

        // GET: DetailCustomerController/Details/5
        [HttpGet]
        public async Task<IActionResult> DetailCustomer(Guid id)
        {
            try
            {
                // ===== SỬA URL =====
                // Sử dụng URL tương đối
                var response = await _httpClient.GetAsync($"DetailCustomer/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var json = await response.Content.ReadAsStringAsync();
                var productDetail = JsonConvert.DeserializeObject<DetailCustomerDto>(json);

                if (productDetail == null)
                {
                    return NotFound();
                }
                return View(productDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailCustomerID(Guid id)
        {
            try
            {
                // ===== SỬA URL =====
                // Sử dụng URL tương đối
                var response = await _httpClient.GetAsync($"DetailCustomer/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var json = await response.Content.ReadAsStringAsync();
                var productDetail = JsonConvert.DeserializeObject<DetailCustomerDto>(json);

                if (productDetail == null)
                {
                    return NotFound();
                }
                return View(productDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }
    }
}