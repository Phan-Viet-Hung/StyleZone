using System.Net.Http.Headers;
using API.DomainCusTomer.DTOs.MoMo;
using API.DomainCusTomer.DTOs.MomocustomerId;
using API.DomainCusTomer.Services;
using API.DomainCusTomer.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using System.Runtime.InteropServices; // Cần thư viện này để check OS
using MVC.Handlers;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // =============================================================
        // 1. SỬA LỖI DATA PROTECTION (Chạy được cả Windows & Docker)
        // =============================================================
        string keysFolder;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Nếu chạy trên Windows (Localhost Visual Studio)
            keysFolder = Path.Combine(Directory.GetCurrentDirectory(), "keys");
        }
        else
        {
            // Nếu chạy trên Linux (Docker / Render)
            keysFolder = "/root/.aspnet/DataProtection-Keys";
        }

        // Tạo thư mục nếu chưa có (để tránh lỗi DirectoryNotFound)
        if (!Directory.Exists(keysFolder)) Directory.CreateDirectory(keysFolder);

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
            .SetApplicationName("StyleZoneApp");
        // =============================================================

        // 2. Cấu hình Momo
        builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
        builder.Services.AddScoped<IMomoService, MomoServicer>();

        builder.Services.Configure<MomoOptionModelId>(builder.Configuration.GetSection("MomoAPI_Customer"));
        builder.Services.AddScoped<IMomoCustomerIdServices, MomoCustomerIdServices>();

        // 3. Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/LoginAccount/Login";
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            // Quan trọng cho Docker/Render:
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            options.Cookie.SameSite = SameSiteMode.Lax;
        })
        .AddGoogle(options =>
        {
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

        builder.Services.AddControllersWithViews();
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Name = ".StyleZone.CustomerSession";
            // Quan trọng cho Docker:
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        });

        // 4. Cấu hình HttpClient
        // Ưu tiên lấy từ config, nếu không có thì fallback về localhost
        var configuration = builder.Configuration;
        string apiBaseUrl = configuration["ApiBaseUrl"];

        if (string.IsNullOrEmpty(apiBaseUrl))
        {
            apiBaseUrl = "https://localhost:7257/api/";
        }

        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        // Fix lỗi HTTPS Redirect trên Render
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        // =============================================================
        // 5. MIDDLEWARE CHUYỂN HƯỚNG ẢNH (Sửa lại logic chuẩn)
        // =============================================================
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.Value != null &&
                context.Request.Path.Value.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase))
            {
                // Lấy PublicApiUrl (VD: https://stylezone-api.onrender.com)
                var config = context.RequestServices.GetRequiredService<IConfiguration>();
                var publicUrl = config["PublicApiUrl"];

                // Nếu không có config, fallback về localhost
                if (string.IsNullOrEmpty(publicUrl))
                {
                    publicUrl = "https://localhost:7257";
                }

                // Chuẩn hóa: Xóa dấu / ở cuối
                var apiDomain = publicUrl.TrimEnd('/');

                // Redirect sang API
                var newUrl = apiDomain + context.Request.Path.Value;
                context.Response.Redirect(newUrl);
                return;
            }
            await next();
        });
        // =============================================================

        app.UseHttpsRedirection(); // Tắt dòng này nếu chạy Docker port 8080 mà không có cert
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
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