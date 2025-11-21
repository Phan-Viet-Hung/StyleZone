using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DbContextApp>();
    // Lệnh này sẽ tạo database và bảng nếu chưa có
    dbContext.Database.Migrate();
}
app.Run();
