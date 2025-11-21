using API.DomainCusTomer.DTOs.Nam;
using API.DomainCusTomer.DTOs.TheThao; // Giữ lại using này nếu PageDoNamCustomer, v.v. nằm ở đây
using API.DomainCusTomer.Request.Nam;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http; // Đảm bảo có using này
using System.Threading.Tasks; // Đảm bảo có using này
using System.Web; // Đảm bảo có using này

namespace MVC.Controllers
{
    public class NamCustomerController : Controller
    {
        private readonly HttpClient _httpClient; // HttpClient này sẽ được lấy từ Factory

        // ===== SỬA CONSTRUCTOR =====
        // Tiêm (inject) IHttpClientFactory thay vì HttpClient
        public NamCustomerController(IHttpClientFactory httpClientFactory)
        {
            // Yêu cầu Factory tạo ra client tên "ApiClient"
            // (Client này đã được cấu hình BaseAddress trong Program.cs)
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        // ===========================

        [HttpGet]
        public async Task<IActionResult> DoNamCustomer([FromQuery] DoNamCustomerFilterRequest filterRequest)
        {
            // Gán giá trị mặc định nếu thiếu
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;

            // Tạo query string
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
            {
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);
            }

            if (filterRequest.Colors != null)
            {
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);
            }

            if (filterRequest.Sizes != null)
            {
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);
            }

            if (filterRequest.Genders != null)
            {
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);
            }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy))
                query.Add("SortBy", filterRequest.SortBy.ToLower());

            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder))
                query.Add("SortOrder", filterRequest.SortOrder.ToLower());

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            // Tạo URL tương đối (relative URL). BaseAddress (ví dụ: http://stylezone-all:8081/api/)
            // sẽ được tự động thêm vào bởi _httpClient.
            var url = $"NamCustomer/DoNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();

                var pagedResult = JsonConvert.DeserializeObject<PageDoNamCustomer>(json);

                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AoNam([FromQuery] AoNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/AoNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageAonam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AoTShirtNam([FromQuery] AoTShirtNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/AoTShirtNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageAoTShirtNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AoPoLoNam([FromQuery] AoPoLoNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/AoPoLoNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageAoPoLoNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AoGioNam([FromQuery] AoGioNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/AoGioNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageAoGioNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AoNiNam([FromQuery] AoNiNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/AoNiNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageAoNiNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AoDaiTayNam([FromQuery] AoDaiTayNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/AoDaiTayNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageAoDaiTayNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AoLongVuNam([FromQuery] AoLongVuNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/AoLongVuNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageAoLongVuNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuanNam([FromQuery] QuanNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/QuanNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageQuanNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuanShortNam([FromQuery] QuanShortNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/QuanShortNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageQuanShortNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuanGioNam([FromQuery] QuanGioNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/QuanGioNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageQuanGioNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuanNiNam([FromQuery] QuanNiNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/QuanNiNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageQuanNiNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GiayNam([FromQuery] GiayNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/GiayNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageGiayNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GiayThoiTrangNam([FromQuery] GiayThoiTrangNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/GiayThoiTrangNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageGiayThoiTrangNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GiayChayBoNam([FromQuery] GiayChayBoNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/GiayChayBoNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageGiayChayBoNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GiayCauLongNam([FromQuery] GiayCauLongNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/GiayCauLongNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageGiayCauLongNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GiayBongRoNam([FromQuery] GiayBongRoNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/GiayBongRoNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageGiayBongRoNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GiayBongDaNam([FromQuery] GiayBongDaNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/GiayBongDaNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageGiayBongDaNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BoQuanAoNam([FromQuery] BoQuanAoNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/BoQuanAoNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageBoQuanAoNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BoQuanAoBongRoNam([FromQuery] BoQuanAoBongRoNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/BoQuanAoBongRoNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageBoQuanAoBongRoNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BoQuanAoCauLongNam([FromQuery] BoQuanAoCauLongNamFilterRequest filterRequest)
        {
            // ... (Code gán giá trị mặc định và tạo query giữ nguyên) ...
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy = string.IsNullOrWhiteSpace(filterRequest.SortBy) ? "createdat" : filterRequest.SortBy;
            filterRequest.SortOrder = string.IsNullOrWhiteSpace(filterRequest.SortOrder) ? "desc" : filterRequest.SortOrder;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (filterRequest.Product != null) { foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Product", item); }
            if (filterRequest.Colors != null) { foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Colors", color); }
            if (filterRequest.Sizes != null) { foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Sizes", size); }
            if (filterRequest.Genders != null) { foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x))) query.Add("Genders", gender); }
            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy)) query.Add("SortBy", filterRequest.SortBy.ToLower());
            if (!string.IsNullOrWhiteSpace(filterRequest.SortOrder)) query.Add("SortOrder", filterRequest.SortOrder.ToLower());
            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA LỖI Ở ĐÂY =====
            var url = $"NamCustomer/BoQuanAoCauLongNam?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                // ... (Code xử lý response giữ nguyên) ...
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageBoQuanAoCauLongNam>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }


        // GET: NamCustomerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NamCustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NamCustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NamCustomerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NamCustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NamCustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NamCustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}