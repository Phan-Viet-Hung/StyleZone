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
            // 1. TÌM HOẶC TẠO ROLE ADMIN
            // Sử dụng ID cố định cho Role Admin để dễ quản lý
            var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Id = adminRoleId,
                    Name = "Admin"
                };
                await context.Roles.AddAsync(adminRole);
            }
            else
            {
                // Nếu Role đã có nhưng ID khác, ta vẫn dùng ID của role đó
                adminRoleId = adminRole.Id;
            }

            // Lưu Role trước để đảm bảo khóa ngoại tồn tại
            await context.SaveChangesAsync();

            // 2. XỬ LÝ TÀI KHOẢN ADMIN
            var adminUser = await context.Accounts.FirstOrDefaultAsync(a => a.UserName == "Admin");

            // Mật khẩu mặc định: Admin@123
            string passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

            if (adminUser == null)
            {
                // --- TRƯỜNG HỢP CHƯA CÓ -> TẠO MỚI ---
                adminUser = new Account
                {
                    Id = Guid.NewGuid(), // Hoặc cố định nếu muốn
                    Name = "Administrator",
                    Birthday = new DateTime(1990, 1, 1).ToUniversalTime(), // Quan trọng: PostgreSQL cần UTC
                    Email = "admin@stylezone.com",
                    PhoneNumber = "0909999999",
                    UserName = "Admin",
                    Gender = GenderEnum.Nam, // 1: Nam
                    Password = passwordHash,
                    Address = "System Admin",
                    IsActive = true,
                    RoleId = adminRoleId // Gán quyền Admin
                };
                await context.Accounts.AddAsync(adminUser);
            }
            else
            {
                // --- TRƯỜNG HỢP ĐÃ CÓ -> RESET LẠI THÔNG TIN (QUAN TRỌNG) ---
                // Điều này giúp bạn đăng nhập được ngay cả khi quên mật khẩu cũ
                adminUser.Password = passwordHash;
                adminUser.IsActive = true;
                adminUser.RoleId = adminRoleId; // Đảm bảo quyền Admin không bị mất

                context.Accounts.Update(adminUser);
            }
            if (!context.PaymentMethods.Any())
            {
                var momo = new PaymentMethod
                {
                    Id = Guid.NewGuid(),
                    Name = "Thanh toán qua MoMo"
                };
                var cod = new PaymentMethod
                {
                    Id = Guid.NewGuid(),
                    Name = "Thanh toán khi nhận hàng (COD)"
                };
                await context.PaymentMethods.AddAsync(momo);
                await context.PaymentMethods.AddAsync(cod);
            }
            
            if (!context.ModeOfPayments.Any())
            {
                var tienmat = new ModeOfPayment
                {
                    Id = Guid.NewGuid(),
                    Name = "Tiền mặt"
                };
                var chuyenkhoan = new ModeOfPayment
                {
                    Id = Guid.NewGuid(),
                    Name = "Chuyển khoản"
                };
                await context.ModeOfPayments.AddAsync(tienmat);
                await context.ModeOfPayments.AddAsync(chuyenkhoan);
            }
            // 3. LƯU THAY ĐỔI CUỐI CÙNG
            await context.SaveChangesAsync();
        }
    }
}