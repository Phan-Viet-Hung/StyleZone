using API.Domain.DTOs;
using API.Domain.Request.CategoryRequest;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MVC.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient"); // lấy client đã cấu hình sẵn base URL
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("category");
            if (!response.IsSuccessStatusCode)
                return View(new List<CategoryDto>());

            var json = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(json);
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("categories", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, error);
            return View(request);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"category/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<CategoryDto>(json);

            var model = new UpdateCategoryRequest
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("category", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, error);
            return View(request);
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetAsync($"category/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<CategoryDto>(json);

            return View(dto);
        }

    }
}
