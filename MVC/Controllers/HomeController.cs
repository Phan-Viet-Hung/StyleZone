using API.DomainCusTomer.DTOs; // Giữ lại using này
using API.DomainCusTomer.DTOs.TrangChu; // Giữ lại using này
using Microsoft.AspNetCore.Mvc;
using MVC.Models; // Giữ lại using này
using Newtonsoft.Json; // Giữ lại using này
using System.Diagnostics;
using System.Net.Http; // Đảm bảo có using này
using System.Security.Policy;
using System.Threading.Tasks; // Đảm bảo có using này

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // Sửa: Xóa IHttpClientFactory, chúng ta sẽ dùng HttpClient đã được cấu hình
        private readonly HttpClient _httpClient;

        // ===== SỬA CONSTRUCTOR =====
        // Tiêm (inject) IHttpClientFactory
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            // Yêu cầu Factory tạo ra client tên "ApiClient"
            // (Client này đã được cấu hình BaseAddress trong Program.cs)
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        // ===========================

        public async Task<IActionResult> Index()
        {
            // ===== SỬA LỖI Ở ĐÂY =====
            // 1. Client đã được tạo trong constructor
            // var client = _httpClientFactory.CreateClient(); // XÓA DÒNG NÀY

            // 2. Dùng URL tương đối (relative URL). BaseAddress (ví dụ: http://stylezone-all:8081/api/)
            //    sẽ được tự động thêm vào bởi _httpClient.
            var apiUrlSanPham = "TrangChuCustomer/SanPhamTrangChu";
            var apiUrlTinTuc = "TrangChuCustomer/TinTucTrangChu";
            // ===========================

            var viewModel = new HomepageViewModel();

            try
            {
                // Dùng _httpClient (thay vì 'client')
                viewModel.FeaturedProducts = await _httpClient.GetFromJsonAsync<Dictionary<string, List<HomeProductCustomerDto>>>(apiUrlSanPham);

                // G?i thêm Tin t?c khuy?n mãi
                viewModel.Promotions = await _httpClient.GetFromJsonAsync<List<HomeProductCustomerDto>>(apiUrlTinTuc);
            }
            catch (Exception ex)
            {
                viewModel.FeaturedProducts = new Dictionary<string, List<HomeProductCustomerDto>>();
                viewModel.Promotions = new List<HomeProductCustomerDto>();
                // Sửa log để hiển thị lỗi chính xác
                _logger.LogError(ex, "Lỗi khi gọi API: {ApiUrlSanPham} hoặc {ApiUrlTinTuc}", apiUrlSanPham, apiUrlTinTuc);
            }

            return View(viewModel);
        }

        // ... (Các action khác như Privacy, Error... giữ nguyên)
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}