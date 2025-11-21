using API.Configuration;
using API.Domain.Request.AccountRequest;
using API.Domain.Request.CategoryRequest; // Đã thêm using mới
using API.Domain.Request.ColorRequest;
using API.Domain.Request.SizeRequest;
using API.Domain.Service;
using API.Domain.Service.IService;
using API.Domain.Service.IService.ICustomerService;
using API.Domain.Validate;
using API.Domain.Validate.IExcelValidator;
using API.DomainCusTomer.Config;
using API.DomainCusTomer.Services;
using API.DomainCusTomer.Services.IServices;
using API.Service;
using API.Services;
using DAL_Empty.Models;
using DomainAPI.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ... (Giữ nguyên phần cấu hình Service từ đầu đến dòng 135) ...
// 1. Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("vi") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("vi");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// 2. Controllers
builder.Services.AddControllers()
  .AddJsonOptions(opt =>
  {
      opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
  });

// 3. JWT Auth
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt?.Issuer ?? builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = jwt?.Audience ?? builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt?.SecretKey ?? builder.Configuration["JwtSettings:SecretKey"])),
            ClockSkew = TimeSpan.Zero
        };
    });

// 4. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "StyleZone API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập JWT token: Bearer <token>",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// 5. CORS
var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(',') ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

// Fix lỗi ngày tháng cho Postgres
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// 6. Database
builder.Services.AddDbContext<DbContextApp>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 7. SERVICES REGISTRATION
// Admin Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IModeOfPaymentService, ModeOfPaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductDetailService, ProductDetailService>();
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IOriginService, OriginService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IExcelValidator<ProductDetail>, ProductDetailValidator>();
builder.Services.AddScoped<ExcelImporter>();

// Customer Services
builder.Services.AddScoped<ITheThaoCustomerServices, TheThaoCusTomerSerVices>();
builder.Services.AddScoped<IThoiTrangCustomerServices, ThoiTrangCustomerServices>();
builder.Services.AddScoped<INamCustomer, NamCustomerServices>();
builder.Services.AddScoped<INuCustomer, NuCustomerservices>();
builder.Services.AddScoped<IDetailCustomerServices, DetailCustomerServices>();
builder.Services.AddScoped<ILoginAccountCustomerServices, LoginAccountCustomerServices>();
builder.Services.AddScoped<ICartCustomerService, CartCustomerService>();
builder.Services.AddHttpClient<IGhnService, GhnSerVices>();
builder.Services.AddScoped<IThanhtoanCustomer, ThanhToanCustomer>();
builder.Services.AddScoped<ISeachCustomerService, SeachCustomerService>();
builder.Services.AddScoped<ITinTucService, TinTucService>();
builder.Services.AddScoped<ITrangChuCustomerService, TrangChuCustomerService>();
builder.Services.AddScoped<IDonMuaCustomerServices, DonMuaCustomerService>();
builder.Services.AddScoped<ICartCustomerIDServices, CartCustomerIDServices>();
builder.Services.AddScoped<IThanhtoanCartIdServices, ThanhtoanCartIdServices>();
builder.Services.AddSingleton<EmailCustomerServicer>();
builder.Services.AddSingleton<OtpHelperServices>();
builder.Services.AddScoped<JwtTokenHelper>();

// Common
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Logging.AddConsole();
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Mail"));

var app = builder.Build();

// 8. SEED DATA (Đã sắp xếp lại thứ tự ưu tiên)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<DbContextApp>();

    try
    {
        logger.LogInformation("--> 🛠️ Đang tạo hàm newid() cho PostgreSQL...");
        // Tạo hàm newid() giả để đánh lừa EF Core
        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE EXTENSION IF NOT EXISTS ""pgcrypto"";
            CREATE OR REPLACE FUNCTION newid() RETURNS uuid AS $$
            BEGIN
                RETURN gen_random_uuid();
            END;
            $$ LANGUAGE plpgsql;
        ");

        logger.LogInformation("--> Đang Migration Database...");
        dbContext.Database.Migrate();

        // 🚩 ƯU TIÊN SỐ 1: TẠO ADMIN ACCOUNT NGAY LẬP TỨC
        logger.LogInformation("--> Đang Seed Admin Account...");
        await SeedAccountRequest.SeedAccountsAsync(dbContext);

        // 🚩 ƯU TIÊN SỐ 2: TẠO CATEGORY (Dữ liệu nền tảng)
        logger.LogInformation("--> Đang Seed Categories...");
        await SeedCategoryRequest.SeedCategoriesAsync(dbContext);

        // Các dữ liệu phụ (Nếu lỗi thì cũng không sao, Admin vẫn vào được)
        logger.LogInformation("--> Đang Seed Colors...");
        await SeedColorsRequest.SeedColorsAsync(dbContext);

        logger.LogInformation("--> Đang Seed Sizes...");
        await SeedSizesRequest.SeedSizesAsync(dbContext);

        logger.LogInformation("--> ✅ Seed Data hoàn tất!");
    }
    catch (Exception ex)
    {
        // Log lỗi nhưng không làm sập app
        logger.LogError(ex, "🚨 LỖI KHI SEED DATA: " + ex.Message);
    }
}

// 9. Middleware
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StyleZone API V1"));

app.UseHttpsRedirection();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseRouting();
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigin"); // Quan trọng: Đặt trước Auth
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();