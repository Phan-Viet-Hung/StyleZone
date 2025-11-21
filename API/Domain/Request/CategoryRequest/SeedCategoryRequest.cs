using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Domain.Request.CategoryRequest
{
    public class SeedCategoryRequest
    {
        public static async Task SeedCategoriesAsync(DbContextApp context)
        {
            // Danh sách Category chuẩn từ file insert-category.sql
            var categoriesToSeed = new List<Category>
            {
                new Category { Name = "Giày Thời Trang", Description = "Giày thời trang nam" },
                new Category { Name = "Bộ Quần Áo Cầu Lông", Description = "Set quần áo cầu lông cho nam" },
                new Category { Name = "Bóng Rổ", Description = "Danh mục sản phẩm cho bóng rổ" },
                new Category { Name = "Quần Gió", Description = "Quần gió cho nam" },
                new Category { Name = "Giày Chạy Bộ", Description = "Giày chạy bộ nam" },
                new Category { Name = "Isaac", Description = "Thời trang phong cách Isaac" },
                new Category { Name = "Badfive", Description = "Thời trang phong cách Badfive" },
                new Category { Name = "Áo Gió", Description = "Áo khoác gió cho nam" },
                new Category { Name = "Golf", Description = "Danh mục sản phẩm cho golf" },
                new Category { Name = "Lifestyle", Description = "Thời trang phong cách Lifestyle" },
                new Category { Name = "Áo T-Shirt", Description = "Áo T-Shirt cho nam" },
                new Category { Name = "Áo Nỉ", Description = "Áo nỉ cho nam" },
                new Category { Name = "Quần Nỉ", Description = "Quần nỉ cho nam" },
                new Category { Name = "Bé Trai (7-14 tuổi)", Description = "Thời trang dành cho bé trai từ 7 đến 14 tuổi" },
                new Category { Name = "Áo Lông Vũ", Description = "Áo lông vũ cho nam" },
                new Category { Name = "Giày Cầu Lông", Description = "Giày cầu lông nam" },
                new Category { Name = "Bé Gái (7-14 tuổi)", Description = "Thời trang dành cho bé gái từ 7 đến 14 tuổi" },
                new Category { Name = "Áo Dài Tay", Description = "Áo dài tay cho nam" },
                new Category { Name = "Wade", Description = "Thời trang phong cách Wade" },
                new Category { Name = "Giày Bóng Rổ", Description = "Giày bóng rổ nam" },
                new Category { Name = "Bóng Đá", Description = "Danh mục sản phẩm cho bóng đá" },
                new Category { Name = "Quần Short", Description = "Quần short cho nam" },
                new Category { Name = "Tập Luyện", Description = "Danh mục sản phẩm cho luyện tập thể thao" },
                new Category { Name = "Áo Polo", Description = "Áo Polo cho nam" },
                new Category { Name = "Thể Thao", Description = "Danh mục sản phẩm thể thao, trang phục và phụ kiện luyện tập" },
                new Category { Name = "Pickleball", Description = "Danh mục sản phẩm cho môn Pickleball" },
                new Category { Name = "Chạy Bộ", Description = "Danh mục sản phẩm cho chạy bộ" },
                new Category { Name = "YOUNG", Description = "Danh mục sản phẩm cho trẻ nhỏ" },
                new Category { Name = "Bộ Quần Áo Bóng Rổ", Description = "Set quần áo bóng rổ cho nam" },
                new Category { Name = "Giày Bóng Đá", Description = "Giày bóng đá nam" },
                new Category { Name = "Thời Trang", Description = "Danh mục sản phẩm thời trang, phong cách và lifestyle" },
                new Category { Name = "Cầu Lông", Description = "Danh mục sản phẩm cho cầu lông" }
            };

            // Duyệt qua danh sách và thêm vào DB nếu chưa tồn tại
            foreach (var cat in categoriesToSeed)
            {
                // Kiểm tra xem Category đã tồn tại chưa (dựa theo Name)
                bool exists = await context.Categories.AnyAsync(c => c.Name == cat.Name);

                if (!exists)
                {
                    // Gán các giá trị mặc định cho giống với file SQL
                    cat.Id = Guid.NewGuid(); // Tương ứng NEWID()
                    cat.CreatedAt = DateTime.Now; // Tương ứng GETDATE()
                    cat.UpdatedAt = null; // NULL theo yêu cầu

                    await context.Categories.AddAsync(cat);
                }
            }

            // Lưu thay đổi vào Database
            await context.SaveChangesAsync();
        }
    }
}
