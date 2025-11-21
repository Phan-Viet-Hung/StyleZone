using System.Net.Http.Headers;
using API.DomainCusTomer.DTOs.MoMo;
using API.DomainCusTomer.DTOs.MomocustomerId;
using API.DomainCusTomer.Services;
using API.DomainCusTomer.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using MVC.Handlers;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
        builder.Services.AddScoped<IMomoService, MomoServicer>();

        builder.Services.Configure<MomoOptionModelId>(builder.Configuration.GetSection("MomoAPI_Customer"));
        builder.Services.AddScoped<IMomoCustomerIdServices, MomoCustomerIdServices>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            // Sửa lại: Đọc config từ builder.Configuration
            options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
            options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
            options.CallbackPath = "/signin-google";

            options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Home/Index?error=" + Uri.EscapeDataString(context.Failure?.Message ?? "unknown"));
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddTransient<AuthHeaderHandler>();

        // Xóa dòng AddHttpClient() vì AddHttpClient("ApiClient") đã làm điều đó
        // builder.Services.AddHttpClient(); 

        builder.Services.AddDistributedMemoryCache(); // Chỉ cần gọi 1 lần

        // Chỉ gọi AddSession 1 lần
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(10); // Giữ cấu hình dài hơn
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            options.Cookie.Name = ".StyleZone.CustomerSession";
        });

        // ===== SỬA LỖI CẤU HÌNH HTTPCLIENT =====

        // 1. Đọc IConfiguration trực tiếp từ builder
        var configuration = builder.Configuration;

        // 2. Lấy URL API.
        // Hệ thống config sẽ TỰ ĐỘNG lấy "ApiBaseUrl" từ biến môi trường (khi chạy Docker)
        // hoặc từ appsettings.json (khi chạy local)
        string apiBaseUrl = configuration["ApiBaseUrl"];

        // 3. Đặt URL dự phòng NẾU nó vẫn rỗng (cho an toàn)
        if (string.IsNullOrEmpty(apiBaseUrl))
        {
            apiBaseUrl = "https://localhost:7257/api/"; // URL debug mặc định
        }

        builder.Services.AddHttpClient("ApiClient", client =>
        {
            // 4. Sử dụng biến apiBaseUrl đã được đọc chính xác
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }).AddHttpMessageHandler<AuthHeaderHandler>();

        // ===== KẾT THÚC SỬA LỖI =====

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Xóa bỏ AddSession() thứ hai vì đã khai báo ở trên
        // builder.Services.AddSession(options => ...);

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
            // app.UseDeveloperExceptionPage(); // Dòng này chỉ nên dùng trong Development
        }
        else
        {
            // Thêm else block để bật DeveloperExceptionPage khi phát triển
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.Use(async (context, next) =>
        {
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
            await next();
        });

       

        app.UseRouting();
        app.UseStaticFiles();
        // Chỉ gọi UseSession() MỘT LẦN, và phải SAU UseRouting()
        app.UseSession();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}