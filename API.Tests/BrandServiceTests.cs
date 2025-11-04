using API.Domain.Request.BrandRequest;
using API.Domain.Service;
using DAL_Empty.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Domain.Request.BrandRequest;
using API.Domain.Service;
using DAL_Empty.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace API.Domain.Tests
{
    public class BrandServiceTests : IDisposable
    {
        private readonly DbContextApp _context;
        private readonly BrandService _service;
        private readonly SqliteConnection _connection;

        public BrandServiceTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<DbContextApp>()
                .UseSqlite(_connection)
                .Options;

            _context = new DbContextApp(options);
            _context.Database.EnsureCreated();

            // Keep tests resilient to FK constraints when seeding isolated rows
            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");

            _service = new BrandService(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Dispose();
        }

        // Helper: run DataAnnotations validation for request objects
        private static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return results;
        }

        // CREATE -----------------------------------------------------------------

        /*
         ID: TC28
         Screen: Danh sách Thương hiệu
         Function: Thêm
         Purpose: Thêm thương hiệu với đầy đủ dữ liệu hợp lệ
         Steps:
           1. Mở trang quản lý thương hiệu
           2. Ấn nút Thêm
           3. Nhập Code = "NIKE", Name = "Nike"
           4. Lưu
         Input: Code = "NIKE", Name = "Nike"
        */
        [Fact]
        public async Task TC28_Create_ValidInput_SavesBrand()
        {
            var req = new CreateBrandRequest { Code = "NIKE", Name = "Nike" };

            var validation = ValidateModel(req);
            Assert.Empty(validation);

            var dto = await _service.CreateAsync(req);

            Assert.NotNull(dto);
            Assert.Equal("NIKE", dto.Code);
            Assert.Equal("Nike", dto.Name);
            Assert.Single(_context.Brands);
        }

        /*
         ID: TC29
         Screen: Danh sách Thương hiệu
         Function: Thêm
         Purpose: Thêm mà bỏ trống Mã thương hiệu (validation lỗi)
         Steps:
           1. Mở trang quản lý thương hiệu
           2. Ấn nút Thêm
           3. Để trống Code, nhập Name = "Adidas"
           4. Lưu
         Input: Code = "", Name = "Adidas"
        */
        [Fact]
        public async Task TC29_Create_EmptyCode_ModelValidationAndServiceThrows()
        {
            var req = new CreateBrandRequest { Code = "", Name = "Adidas" };

            var validation = ValidateModel(req);
            Assert.Contains(validation, v => v.MemberNames.Any(m => m.Equals("Code", StringComparison.OrdinalIgnoreCase)));

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(req));
        }

        /*
         ID: TC30
         Screen: Danh sách Thương hiệu
         Function: Thêm
         Purpose: Thêm mà bỏ trống Tên thương hiệu (validation lỗi)
         Steps:
           1. Mở trang quản lý thương hiệu
           2. Ấn nút Thêm
           3. Nhập Code = "ADIDAS", để trống Name
           4. Lưu
         Input: Code = "ADIDAS", Name = ""
        */
        [Fact]
        public async Task TC30_Create_EmptyName_ModelValidationAndServiceThrows()
        {
            var req = new CreateBrandRequest { Code = "ADIDAS", Name = "" };

            var validation = ValidateModel(req);
            Assert.Contains(validation, v => v.MemberNames.Any(m => m.Equals("Name", StringComparison.OrdinalIgnoreCase)));

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(req));
        }

        /*
         ID: TC31
         Screen: Danh sách Thương hiệu
         Function: Thêm
         Purpose: Thêm mà Tên thương hiệu vượt quá 100 ký tự (validation lỗi)
         Steps:
           1. Mở trang quản lý thương hiệu
           2. Ấn nút Thêm
           3. Nhập Code = "LONGNAME", Name = 101 chars
           4. Lưu
         Input: Code = "LONGNAME", Name = 101 chars
        */
        [Fact]
        public void TC31_Create_NameTooLong_ModelValidationFails()
        {
            var longName = new string('A', 101);
            var req = new CreateBrandRequest { Code = "LONGNAME", Name = longName };

            var validation = ValidateModel(req);
            Assert.Contains(validation, v => v.MemberNames.Any(m => m.Equals("Name", StringComparison.OrdinalIgnoreCase)));
        }

        /*
         ID: TC32
         Screen: Danh sách Thương hiệu
         Function: Thêm
         Purpose: Thêm mà Mã thương hiệu vượt quá 20 ký tự (validation lỗi)
         Steps:
           1. Mở trang quản lý thương hiệu
           2. Ấn nút Thêm
           3. Nhập Code = 21 chars, Name = "BrandX"
           4. Lưu
         Input: Code = 21 chars, Name = "BrandX"
        */
        [Fact]
        public void TC32_Create_CodeTooLong_ModelValidationFails()
        {
            var longCode = new string('C', 21);
            var req = new CreateBrandRequest { Code = longCode, Name = "BrandX" };

            var validation = ValidateModel(req);
            Assert.Contains(validation, v => v.MemberNames.Any(m => m.Equals("Code", StringComparison.OrdinalIgnoreCase)));
        }

        /*
         ID: TC33
         Screen: Danh sách Thương hiệu
         Function: Thêm
         Purpose: Thêm mà Mã thương hiệu sai định dạng (chứa chữ thường hoặc ký tự đặc biệt)
         Steps:
           1. Mở trang quản lý thương hiệu
           2. Ấn nút Thêm
           3. Nhập Code = "nike!" hoặc "nike" (lowercase)
           4. Lưu
         Input: Code = "nike!" or "nike", Name = "Nike"
        */
        [Fact]
        public void TC33_Create_CodeInvalidFormat_ModelValidationFails()
        {
            var reqLower = new CreateBrandRequest { Code = "nike", Name = "Nike" };
            var reqSpecial = new CreateBrandRequest { Code = "NIKE!", Name = "Nike" };

            var vLower = ValidateModel(reqLower);
            var vSpecial = ValidateModel(reqSpecial);

            Assert.Contains(vLower, v => v.MemberNames.Any(m => m.Equals("Code", StringComparison.OrdinalIgnoreCase)));
            Assert.Contains(vSpecial, v => v.MemberNames.Any(m => m.Equals("Code", StringComparison.OrdinalIgnoreCase)));
        }

        /*
         ID: TC34
         Screen: Danh sách Thương hiệu
         Function: Thêm
         Purpose: Thêm thương hiệu mà tên và mã bị trùng với thương hiệu đã có (validation lỗi)
         Steps:
           1. Tạo sẵn Brand { Code = "BRX", Name = "BrandX" }
           2. Thực hiện thêm Code = "BRX", Name = "BrandX"
           3. Lưu -> service ném ArgumentException
         Input: duplicate code/name
        */
        [Fact]
        public async Task TC34_Create_DuplicateCodeOrName_ThrowsArgumentException()
        {
            _context.Brands.Add(new Brand { Id = Guid.NewGuid(), Code = "BRX", Name = "BrandX" });
            await _context.SaveChangesAsync();

            var reqSameCode = new CreateBrandRequest { Code = "BRX", Name = "NewName" };
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(reqSameCode));
        }

        // UPDATE -----------------------------------------------------------------

        /*
         ID: TC35
         Screen: Danh sách Thương hiệu
         Function: Sửa
         Purpose: Sửa đầy đủ dữ liệu hợp lệ
         Steps:
           1. Tạo sẵn Brand { Code = "OLD", Name = "OldName" }
           2. Nhấn sửa, nhập Code = "NEW", Name = "NewName"
           3. Lưu
         Input: Id = existing.Id, Code = "NEW", Name = "NewName"
        */
        [Fact]
        public async Task TC35_Update_ValidInput_UpdatesBrand()
        {
            var b = new Brand { Id = Guid.NewGuid(), Code = "OLD", Name = "OldName" };
            _context.Brands.Add(b);
            await _context.SaveChangesAsync();

            var req = new UpdateBrandRequest { Id = b.Id, Code = "NEW", Name = "NewName" };
            var validation = ValidateModel(req);
            Assert.Empty(validation);

            var dto = await _service.UpdateAsync(b.Id, req);

            Assert.Equal("NEW", dto.Code);
            Assert.Equal("NewName", dto.Name);

            var db = await _context.Brands.FindAsync(b.Id);
            Assert.Equal("NEW", db!.Code);
            Assert.Equal("NewName", db.Name);
        }

        /*
         ID: TC36
         Screen: Danh sách Thương hiệu
         Function: Sửa
         Purpose: Sửa mà bỏ trống Tên thương hiệu (validation lỗi)
         Steps:
           1. Tạo sẵn Brand
           2. Mở form sửa, để trống Name
           3. Lưu -> model validation fails and service throws
         Input: Name = ""
        */
        [Fact]
        public async Task TC36_Update_EmptyName_ModelValidationAndServiceThrows()
        {
            var b = new Brand { Id = Guid.NewGuid(), Code = "BX", Name = "BrandX" };
            _context.Brands.Add(b);
            await _context.SaveChangesAsync();

            var req = new UpdateBrandRequest { Id = b.Id, Code = "BX", Name = "" };
            var validation = ValidateModel(req);
            Assert.Contains(validation, v => v.MemberNames.Any(m => m.Equals("Name", StringComparison.OrdinalIgnoreCase)));

            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(b.Id, req));
        }

        /*
         ID: TC37
         Screen: Danh sách Thương hiệu
         Function: Sửa
         Purpose: Sửa mà bỏ trống Mã thương hiệu (validation lỗi)
         Steps:
           1. Tạo sẵn Brand
           2. Mở form sửa, để trống Code
           3. Lưu -> model validation fails and service throws
         Input: Code = ""
        */
        [Fact]
        public async Task TC37_Update_EmptyCode_ModelValidationAndServiceThrows()
        {
            var b = new Brand { Id = Guid.NewGuid(), Code = "BX2", Name = "BrandX2" };
            _context.Brands.Add(b);
            await _context.SaveChangesAsync();

            var req = new UpdateBrandRequest { Id = b.Id, Code = "", Name = "BrandX2" };
            var validation = ValidateModel(req);
            Assert.Contains(validation, v => v.MemberNames.Any(m => m.Equals("Code", StringComparison.OrdinalIgnoreCase)));

            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(b.Id, req));
        }

        /*
         ID: TC38
         Screen: Danh sách Thương hiệu
         Function: Sửa
         Purpose: Sửa mà Tên vượt quá 100 ký tự (validation lỗi)
         Steps:
           1. Tạo sẵn Brand
           2. Mở form sửa, nhập Name = 101 chars
           3. Lưu -> model validation fails
         Input: Name = 101 chars
        */
        [Fact]
        public void TC38_Update_NameTooLong_ModelValidationFails()
        {
            var longName = new string('N', 101);
            var req = new UpdateBrandRequest { Id = Guid.NewGuid(), Code = "VALID", Name = longName };

            var validation = ValidateModel(req);
            Assert.Contains(validation, v => v.MemberNames.Any(m => m.Equals("Name", StringComparison.OrdinalIgnoreCase)));
        }

        /*
         ID: TC39
         Screen: Danh sách Thương hiệu
         Function: Sửa
         Purpose: Sửa mà Mã vượt quá 20 ký tự (validation lỗi)
         Steps:
           1. Tạo sẵn Brand
           2. Mở form sửa, nhập Code = 21 chars
           3. Lưu -> model validation fails
         Input: Code = 21 chars
        */
        [Fact]
        public void TC39_Update_CodeTooLong_ModelValidationFails()
        {
            var longCode = new string('C', 21);
            var req = new UpdateBrandRequest { Id = Guid.NewGuid(), Code = longCode, Name = "Name" };

            var validation = ValidateModel(req);
            Assert.Contains(validation, v => v.MemberNames.Any(m => m.Equals("Code", StringComparison.OrdinalIgnoreCase)));
        }

        /*
         ID: TC40
         Screen: Danh sách Thương hiệu
         Function: Sửa
         Purpose: Sửa mà Mã thương hiệu sai định dạng (validation lỗi)
         Steps:
           1. Tạo sẵn Brand
           2. Mở form sửa, nhập Code = "invalid!" or "invalid" (lowercase)
           3. Lưu -> model validation fails
         Input: Code = "invalid!" or "invalid"
        */
        [Fact]
        public void TC40_Update_CodeInvalidFormat_ModelValidationFails()
        {
            var reqLower = new UpdateBrandRequest { Id = Guid.NewGuid(), Code = "invalid", Name = "Name" };
            var reqSpecial = new UpdateBrandRequest { Id = Guid.NewGuid(), Code = "INVALID!", Name = "Name" };

            var vLower = ValidateModel(reqLower);
            var vSpecial = ValidateModel(reqSpecial);

            Assert.Contains(vLower, v => v.MemberNames.Any(m => m.Equals("Code", StringComparison.OrdinalIgnoreCase)));
            Assert.Contains(vSpecial, v => v.MemberNames.Any(m => m.Equals("Code", StringComparison.OrdinalIgnoreCase)));
        }

        /*
         ID: TC41
         Screen: Danh sách Thương hiệu
         Function: Sửa
         Purpose: Sửa thương hiệu mà mã bị trùng với thương hiệu khác (validation lỗi)
         Steps:
           1. Tạo brandA (A1) and brandB (B1)
           2. Thử sửa brandB.Code -> "A1"
           3. Lưu -> service ném ArgumentException
         Input: Code conflict
        */
        [Fact]
        public async Task TC41_Update_DuplicateCode_ThrowsArgumentException()
        {
            var a = new Brand { Id = Guid.NewGuid(), Code = "A1", Name = "A" };
            var b = new Brand { Id = Guid.NewGuid(), Code = "B1", Name = "B" };
            _context.Brands.AddRange(a, b);
            await _context.SaveChangesAsync();

            var req = new UpdateBrandRequest { Id = b.Id, Code = "A1", Name = "B Updated" };
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(b.Id, req));
        }

        /*
         ID: TC42
         Screen: Danh sách Thương hiệu
         Function: Sửa
         Purpose: Sửa thương hiệu không tồn tại (validation lỗi)
         Steps:
           1. Chuẩn bị UpdateBrandRequest với Id không tồn tại
           2. Gọi UpdateAsync -> expect KeyNotFoundException
         Input: Id = Guid.NewGuid(), Code = "X", Name = "X"
        */
        [Fact]
        public async Task TC42_Update_NotFound_ThrowsKeyNotFoundException()
        {
            var req = new UpdateBrandRequest { Id = Guid.NewGuid(), Code = "X1", Name = "X" };
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(req.Id, req));
        }
    }
}