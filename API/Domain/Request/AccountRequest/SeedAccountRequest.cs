using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace API.Domain.Request.AccountRequest
{
    public class SeedAccountRequest
    {
        public static async Task SeedAccountsAsync(DbContextApp context)
        {
            // 1. Đảm bảo Role Admin tồn tại
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Cố định ID cho dễ nhớ
                    Name = "Admin"
                };
                await context.Roles.AddAsync(adminRole);
                await context.SaveChangesAsync();
            }

            // 2. Kiểm tra tài khoản Admin
            var adminAccount = await context.Accounts.FirstOrDefaultAsync(a => a.UserName == "Admin");

            if (adminAccount == null)
            {
                // Nếu chưa có -> Tạo mới
                adminAccount = new Account
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), // Cố định ID Admin
                    Name = "Administrator",
                    Birthday = new DateTime(1990, 1, 1).ToUniversalTime(), // Nhớ chuyển UTC cho Postgres
                    Email = "admin@stylezone.com",
                    PhoneNumber = "0901234567",
                    UserName = "Admin",
                    Gender = GenderEnum.Nam, // 1: Nam
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // Mật khẩu chuẩn
                    Address = "Hanoi, Vietnam",
                    IsActive = true,
                    RoleId = adminRole.Id
                };
                await context.Accounts.AddAsync(adminAccount);
            }
            else
            {
                // Nếu đã có -> Reset lại mật khẩu về mặc định (Admin@123) để chắc chắn đăng nhập được
                adminAccount.Password = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                adminAccount.IsActive = true;
                adminAccount.RoleId = adminRole.Id; // Đảm bảo luôn có quyền Admin
                context.Accounts.Update(adminAccount);
            }

            // 3. Lưu thay đổi
            await context.SaveChangesAsync();
        }
    }
}