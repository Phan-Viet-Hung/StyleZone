using System.Net.Http.Headers;
using API.DomainCusTomer.DTOs.MoMo;
using API.DomainCusTomer.DTOs.MomocustomerId;
using API.DomainCusTomer.Services;
using API.DomainCusTomer.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection; // Nhớ using cái này
using MVC.Handlers;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Cấu hình Data Protection (QUAN TRỌNG: Sửa lỗi key login)
        // Nếu bạn chưa tạo Disk, dòng này sẽ lưu tạm vào ổ cứng container
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/root/.aspnet/DataProtection-Keys"))
            .SetApplicationName("StyleZoneApp");

        // 2. Cấu hình Momo
        builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
        builder.Services.AddScoped<IMomoService, MomoServicer>();

        builder.Services.Configure<MomoOptionModelId>(builder.Configuration.GetSection("MomoAPI_Customer"));
        builder.Services.AddScoped<IMomoCustomerIdServices, MomoCustomerIdServices>();

        // 3. Authentication (Cookie + Google)
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
        });

        // 4. Cấu hình HttpClient gọi về API
        var configuration = builder.Configuration;
        string apiBaseUrl = configuration["ApiBaseUrl"];
        if (string.IsNullOrEmpty(apiBaseUrl)) apiBaseUrl = "https://localhost:7257/api/";

        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession(); // Session phải sau Routing
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