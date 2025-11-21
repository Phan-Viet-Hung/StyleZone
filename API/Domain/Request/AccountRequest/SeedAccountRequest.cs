using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net; // Cần thêm using này để băm mật khẩu

namespace API.Domain.Request.AccountRequest
{
    public class SeedAccountRequest
    {
        public static async Task SeedAccountsAsync(DbContextApp context)
        {
            // 1. Kiểm tra xem tài khoản đã tồn tại chưa
            if (await context.Accounts.AnyAsync(a => a.UserName == "Admin"))
                return; // Nếu user 'admin' đã tồn tại, không làm gì cả

            // 2. Tìm Role "Admin". 
            //    (Giả sử bạn đã có một file SeedRoles.cs chạy trước)
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

            // 3. Nếu Role "Admin" chưa tồn tại, tạo mới nó
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin"
                };
                await context.Roles.AddAsync(adminRole);
                await context.SaveChangesAsync(); // Lưu Role trước
            }

            // 4. Tạo tài khoản Admin mới
            var adminAccount = new Account
            {
                Id = Guid.NewGuid(),
                Name = "Administrator", // Dữ liệu ngẫu nhiên chứa "Admin"
                Birthday = new DateTime(1990, 1, 1), // Thỏa mãn MinAge(16)

                Email = "admin@admin.admin", // Theo yêu cầu

                PhoneNumber = "0901234567", // Thỏa mãn Regex SĐT Việt Nam

                UserName = "Admin", // Theo yêu cầu (đã chỉnh sửa từ "admin" thành "Admin")

                Gender = GenderEnum.Nam, // Dữ liệu ngẫu nhiên

                // Băm mật khẩu "Admin@123". 
                // Mặc dù bạn yêu cầu "ngẫu nhiên", nhưng mật khẩu ngẫu nhiên
                // sẽ không thể đăng nhập được. Dùng mật khẩu cố định đã băm là cách làm đúng.
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),

                Address = "123 Admin Street", // Dữ liệu ngẫu nhiên
                IsActive = true, // Admin nên được kích hoạt
                RoleId = adminRole.Id // Gán Role Admin
            };

            // 5. Thêm và lưu vào Database
            await context.Accounts.AddAsync(adminAccount);
            await context.SaveChangesAsync();
        }
    }
}