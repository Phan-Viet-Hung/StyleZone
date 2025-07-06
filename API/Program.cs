using API.Domain.Request.CorlorRequest;
using API.Domain.Request.SizeRequest;
using API.Domain.Service;
using API.Domain.Service.IService;
using API.Services;
using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StyleZone_API.Domain.Service;
using StyleZone_API.Domain.Service.IService;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "StyleZone API", Version = "v1" });
});

builder.Services.AddDbContext<DbContextApp>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductDetailService, ProductDetailService>();
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IOriginService, OriginService>();
builder.Services.AddScoped<IImageService, ImageService>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<DbContextApp>();
    await SeedColorsRequest.SeedColorsAsync(dbContext);
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<DbContextApp>();

    // Seed dữ liệu size
    await SeedSizesRequest.SeedSizesAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
