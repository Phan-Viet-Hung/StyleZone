using API.DomainCusTomer.DTOs;
using API.DomainCusTomer.DTOs.ThoiTrang;
using API.DomainCusTomer.Request;
using API.DomainCusTomer.Request.ThoiTrang;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http; // Đảm bảo có using này
using System.Linq;    // Đảm bảo có using này
using System.Web;

namespace MVC.Controllers
{
    public class ThoiTrangCustomerMVCController : Controller
    {
        private readonly HttpClient _httpClient;

        // ===== SỬA CONSTRUCTOR =====
        // Tiêm (inject) IHttpClientFactory
        public ThoiTrangCustomerMVCController(IHttpClientFactory httpClientFactory)
        {
            // Yêu cầu Factory tạo ra client tên "ApiClient"
            // (Client này đã được cấu hình BaseAddress trong Program.cs của MVC)
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        // ===========================

        // GET: TheThaoCustomer
        [HttpGet]
        public async Task<IActionResult> ThoiTrangCusTomer([FromQuery] ThoiTrangFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            // Sử dụng URL tương đối. BaseAddress (http://stylezone-all:8081/api/) sẽ tự động được thêm vào.
            var url = $"ThoiTrangCustomer/ThoiTrang/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageThoiTrang>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                // Thêm try-catch để xử lý lỗi (ví dụ: Connection refused nếu API chưa chạy)
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> WADECusTomer([FromQuery] WadeFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            var url = $"ThoiTrangCustomer/WADE/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageWaDe>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BADFIVECusTomer([FromQuery] BADFIVEFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            var url = $"ThoiTrangCustomer/BADFIVE/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageBADFIVE>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LIFESTYLECusTomer([FromQuery] LIFESTYLEFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            var url = $"ThoiTrangCustomer/LIFESTYLE/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageLIFESTYLE>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ISAACCusTomer([FromQuery] ISAACFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            var url = $"ThoiTrangCustomer/ISAAC/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageISAAC>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> YOUNGCusTomer([FromQuery] YOUNGFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            var url = $"ThoiTrangCustomer/YOUNG/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageYOUNG>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BeTraiCusTomer([FromQuery] BeTraiFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            var url = $"ThoiTrangCustomer/Betrai/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageBeTrai>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BeGaiCusTomer([FromQuery] BeGaiFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            var url = $"ThoiTrangCustomer/BeGai/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageBeGai>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OUTLETCusTomer([FromQuery] OUTLETFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            var url = $"ThoiTrangCustomer/OUTLET/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageOUTLET>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OUTLETPICKLEBALLCusTomer([FromQuery] OUTLETPICKLEBALLFilterRequst filterRequest)
        {
            filterRequest.Page = filterRequest.Page <= 0 ? 1 : filterRequest.Page;
            filterRequest.PageSize = filterRequest.PageSize <= 0 ? 12 : filterRequest.PageSize;
            filterRequest.SortBy ??= "createdat";
            filterRequest.SortOrder ??= "desc";

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (filterRequest.Product != null)
                foreach (var item in filterRequest.Product.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Product", item);

            if (filterRequest.Colors != null)
                foreach (var color in filterRequest.Colors.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Colors", color);

            if (filterRequest.Sizes != null)
                foreach (var size in filterRequest.Sizes.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Sizes", size);

            if (filterRequest.Genders != null)
                foreach (var gender in filterRequest.Genders.Where(x => !string.IsNullOrWhiteSpace(x)))
                    query.Add("Genders", gender);

            query.Add("SortBy", filterRequest.SortBy);
            query.Add("SortOrder", filterRequest.SortOrder);
            query.Add("Page", filterRequest.Page.ToString());
            query.Add("PageSize", filterRequest.PageSize.ToString());

            // ===== SỬA URL =====
            var url = $"ThoiTrangCustomer/OUTLETPICKLEBALL/?{query.ToString()}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Lỗi khi gọi API: {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PageOUTLETPICKLEBALL>(json);
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi ngoại lệ: {ex.Message}");
            }
        }

        // GET: ThoiTrangCustomerMVCController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ThoiTrangCustomerMVCController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ThoiTrangCustomerMVCController/Create
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

        // GET: ThoiTrangCustomerMVCController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ThoiTrangCustomerMVCController/Edit/5
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

        // GET: ThoiTrangCustomerMVCController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ThoiTrangCustomerMVCController/Delete/5
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