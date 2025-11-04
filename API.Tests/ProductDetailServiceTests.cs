using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Domain.DTOs;
using API.Domain.Request.ProductDetailRequest;
using API.Domain.Request.VoucherRequest;
using API.Domain.Service;
using DAL_Empty.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ClosedXML.Excel;

namespace API.Domain.Tests
{
    public class ProductDetailServiceTests : IDisposable
    {
        private readonly DbContextApp _context;
        private readonly ProductDetailService _service;
        private readonly SqliteConnection _connection;

        public ProductDetailServiceTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<DbContextApp>()
                .UseSqlite(_connection)
                .Options;

            _context = new DbContextApp(options);
            _context.Database.EnsureCreated();

            // 🔧 Tắt kiểm tra foreign key trong SQLite
            _context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");

            _service = new ProductDetailService(_context);
        }


        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Dispose();
        }

        private static ProductDetail CreateFakeDetail(Guid? productId = null, string? code = null, string? name = null, int qty = 5, decimal price = 100)
        {
            return new ProductDetail
            {
                Id = Guid.NewGuid(),
                ProductId = productId ?? Guid.NewGuid(),
                Code = code ?? Guid.NewGuid().ToString("N")[..6],
                Name = name ?? $"Detail_{Guid.NewGuid():N}".Substring(0, 8),
                Quantity = qty,
                Price = price,
                Status = ProductDetailStatus.Active
            };
        }

        [Fact]
        public async Task CreateAsync_ExistingDetailByName_IncrementsQuantity()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "P1",
                BrandId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid()
            };

            // Gán navigation với dữ liệu hợp lệ
            product.Brand = new Brand
            {
                Id = product.BrandId,
                Name = "B1",
                Code = "BR001"   // bắt buộc
            };

            product.Category = new Category
            {
                Id = product.CategoryId,
                Name = "Cat1"
            };

            var existing = CreateFakeDetail(product.Id, "C1", $"{product.Name} - C1", 2, 50);
            existing.Product = product;

            _context.Products.Add(product);
            _context.ProductDetails.Add(existing);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var req = new CreateProductDetailRequest
            {
                ProductId = product.Id,
                Code = "C1",
                Price = 50,
                Quantity = 3,
                Status = ProductDetailStatus.Active
            };

            var dto = await _service.CreateAsync(req);

            var updatedEntity = await _context.ProductDetails
                .Include(x => x.Product).ThenInclude(p => p.Brand)
                .Include(x => x.Product).ThenInclude(p => p.Category)
                .Include(x => x.Color)
                .Include(x => x.Size)
                .Include(x => x.Material)
                .Include(x => x.Origin)
                .Include(x => x.Supplier)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(d => d.Id == existing.Id);

            Assert.NotNull(dto);
            Assert.NotNull(updatedEntity);
            Assert.Equal(5, updatedEntity.Quantity);

        }



        [Fact]
        public async Task CreateAsync_DuplicateCode_Throws()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "P1", BrandId = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
            var existing = CreateFakeDetail(product.Id, "C2", "Duplicate Detail");
            existing.Product = product;
            _context.Products.Add(product);
            _context.ProductDetails.Add(existing);
            await _context.SaveChangesAsync();

            var req = new CreateProductDetailRequest
            {
                ProductId = product.Id,
                Code = "C2",
                Price = 10,
                Quantity = 1,
                Status = ProductDetailStatus.Active
            };

            await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(req));
        }

        [Fact]
        public async Task UpdateAsync_MergeWithDuplicate_RemovesOldAndReturnsMerged()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "P",
                BrandId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid()
            };
            product.Brand = new Brand { Id = product.BrandId, Name = "Brand1", Code = "BR1" };
            product.Category = new Category { Id = product.CategoryId, Name = "Cat1" };

            // Fake detail
            var detailToUpdate = CreateFakeDetail(product.Id, "X", "DetailX", 2);
            detailToUpdate.Product = product;

            // Tạo Size/Color/Material/Origin/Supplier đầy đủ với các field Required
            var size = new Size { Id = detailToUpdate.SizeId ?? Guid.NewGuid(), Name = "M", Code = "S1" };
            var color = new Color { Id = detailToUpdate.ColorId ?? Guid.NewGuid(), Name = "Red", Code = "C1" };
            var material = new Material { Id = detailToUpdate.MaterialId ?? Guid.NewGuid(), Name = "Cotton", Description = "Soft Cotton" };
            var origin = new Origin { Id = detailToUpdate.OriginId ?? Guid.NewGuid(), Name = "VN", Description = "Vietnam" };
            var supplier = new Supplier
            {
                Id = detailToUpdate.SupplierId ?? Guid.NewGuid(),
                Name = "Sup1",
                Contact = "+84912345678",   // Required
                Email = "sup1@example.com", // Required
                Address = "Hanoi, Vietnam"  // Required
            };

            detailToUpdate.Size = size;
            detailToUpdate.Color = color;
            detailToUpdate.Material = material;
            detailToUpdate.Origin = origin;
            detailToUpdate.Supplier = supplier;

            // Duplicate
            var duplicate = CreateFakeDetail(product.Id, "Y", "DetailY", 5);
            duplicate.Product = product;
            duplicate.Size = size;
            duplicate.Color = color;
            duplicate.Material = material;
            duplicate.Origin = origin;
            duplicate.Supplier = supplier;

            // Thêm vào context
            _context.Products.Add(product);
            _context.Sizes.Add(size);
            _context.Colors.Add(color);
            _context.Materials.Add(material);
            _context.Origins.Add(origin);
            _context.Suppliers.Add(supplier);
            _context.ProductDetails.AddRange(detailToUpdate, duplicate);

            await _context.SaveChangesAsync();
        }


        [Fact]
        public async Task UpdateAsync_UpdatesPromotionProducts_PriceAfterDiscountChanged()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "P", BrandId = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
            var pd = CreateFakeDetail(product.Id, "C", "DetailC", 5, 100);
            var promotion = new Promotion
            {
                Id = Guid.NewGuid(),
                Name = "Promo_UpdatePrice",
                DiscountType = DiscountType.Percentage,
                DiscountValue = 10,
                Status = VoucherStatus.Active,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            };
            var pp = new PromotionProduct
            {
                Id = Guid.NewGuid(),
                ProductDetail = pd,
                Promotion = promotion,
                ProductDetailId = pd.Id,
                PromotionId = promotion.Id,
                Pricebeforereduction = pd.Price,
                Priceafterduction = 90m
            };

            _context.Products.Add(product);
            _context.ProductDetails.Add(pd);
            _context.Promotions.Add(promotion);
            _context.PromotionProducts.Add(pp);
            await _context.SaveChangesAsync();

            var req = new UpdateProductDetailRequest
            {
                Id = pd.Id,
                Code = "C",
                Price = 200,
                Quantity = 4,
                Status = ProductDetailStatus.Active
            };

            var dto = await _service.UpdateAsync(req);
            Assert.NotNull(dto);
        }

        [Fact]
        public async Task GetAllAsync_WithAndWithoutProductId_ReturnsCorrect()
        {
            var productA = new Product { Id = Guid.NewGuid(), Name = "A", BrandId = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
            productA.Brand = new Brand { Id = productA.BrandId, Name = "BrandA", Code = "BR1" };
            productA.Category = new Category { Id = productA.CategoryId, Name = "CatA" };

            var productB = new Product { Id = Guid.NewGuid(), Name = "B", BrandId = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
            productB.Brand = new Brand { Id = productB.BrandId, Name = "BrandB", Code = "BR2" };
            productB.Category = new Category { Id = productB.CategoryId, Name = "CatB" };

            var pdA = CreateFakeDetail(productA.Id, "PA", "DetailA");
            pdA.Id = Guid.NewGuid();
            pdA.Product = productA;
            pdA.Status = ProductDetailStatus.Active;

            var pdB = CreateFakeDetail(productB.Id, "PB", "DetailB");
            pdB.Id = Guid.NewGuid();
            pdB.Product = productB;
            pdB.Status = ProductDetailStatus.Active;

            _context.Products.AddRange(productA, productB);
            _context.ProductDetails.AddRange(pdA, pdB);
            await _context.SaveChangesAsync();

            var all = await _service.GetAllAsync();
            var onlyA = await _service.GetAllAsync(productA.Id);

            Assert.Equal(2, all.Count);
            Assert.Single(onlyA);

        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNullWhenNotFound()
        {
            var dto = await _service.GetByIdAsync(Guid.NewGuid());
            Assert.Null(dto);
        }

        [Fact]
        public async Task ChangeStatusAsync_InvalidRequests_ThrowAndValid_Change()
        {
            var pd = CreateFakeDetail();
            pd.Quantity = 0; // ✅ Cho phép chuyển sang trạng thái OutOfStock
            _context.ProductDetails.Add(pd);
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ChangeStatusAsync((ChangeStatusRequest?)null!));
            await Assert.ThrowsAsync<ArgumentException>(() => _service.ChangeStatusAsync(new ChangeStatusRequest { Id = pd.Id, Status = "" }));

            var req = new ChangeStatusRequest { Id = pd.Id, Status = ProductDetailStatus.OutOfStock.ToString() };
            var res = await _service.ChangeStatusAsync(req);

            Assert.True(res);
        }

        [Fact]
        public async Task BulkChangeStatusAsync_ValidatesAndChanges()
        {
            var pd1 = CreateFakeDetail(code: "P001", name: "Product 1");
            var pd2 = CreateFakeDetail(code: "P002", name: "Product 2");

            _context.ProductDetails.AddRange(pd1, pd2);
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.BulkChangeStatusAsync((BulkStatusChangeRequest?)null!));
            await Assert.ThrowsAsync<ArgumentException>(() => _service.BulkChangeStatusAsync(new BulkStatusChangeRequest { Ids = new List<Guid>(), Status = "Active" }));
            await Assert.ThrowsAsync<ArgumentException>(() => _service.BulkChangeStatusAsync(new BulkStatusChangeRequest { Ids = new List<Guid> { pd1.Id }, Status = "" }));

            var req = new BulkStatusChangeRequest { Ids = new List<Guid> { pd1.Id, pd2.Id }, Status = ProductDetailStatus.Inactive.ToString() };
            var ok = await _service.BulkChangeStatusAsync(req);

            Assert.True(ok);
        }

        [Fact]
        public async Task GetByIdsAsync_ReturnsRequested()
        {
            var pd1 = CreateFakeDetail();
            var pd2 = CreateFakeDetail();
            _context.ProductDetails.AddRange(pd1, pd2);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdsAsync(new List<Guid> { pd2.Id });

            Assert.Single(result);
        }

        [Fact]
        public async Task UpdateProductQuantityAfterOrderAsync_ReducesQuantityOrThrows()
        {
            var pd1 = CreateFakeDetail(qty: 5);
            var pd2 = CreateFakeDetail(qty: 1);
            _context.ProductDetails.AddRange(pd1, pd2);
            await _context.SaveChangesAsync();

            var odValid = new List<OrderDetail> { new OrderDetail { ProductDetailId = pd1.Id, Quantity = 3 } };
            await _service.UpdateProductQuantityAfterOrderAsync(odValid);
            Assert.Equal(2, _context.ProductDetails.Find(pd1.Id)!.Quantity);

            var odInvalid = new List<OrderDetail> { new OrderDetail { ProductDetailId = pd2.Id, Quantity = 5 } };
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateProductQuantityAfterOrderAsync(odInvalid));

            var odNull = new List<OrderDetail> { new OrderDetail { ProductDetailId = null, Quantity = 1 } };
            await Assert.ThrowsAsync<Exception>(() => _service.UpdateProductQuantityAfterOrderAsync(odNull));
        }

        [Fact]
        public async Task GetAllWithDisplayPriceAsync_UsesActivePromotion()
        {
            var pd = CreateFakeDetail(price: 100);
            var promo = new Promotion
            {
                Id = Guid.NewGuid(),
                Name = "Promo_Display",
                DiscountType = DiscountType.FixedAmount,
                DiscountValue = 20m,
                Status = VoucherStatus.Active,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            };

            var pp = new PromotionProduct
            {
                Id = Guid.NewGuid(),
                ProductDetail = pd,
                Promotion = promo,
                ProductDetailId = pd.Id,
                PromotionId = promo.Id,
                Priceafterduction = 80m
            };

            _context.ProductDetails.Add(pd);
            _context.Promotions.Add(promo);
            _context.PromotionProducts.Add(pp);
            await _context.SaveChangesAsync();

            var res = await _service.GetAllWithDisplayPriceAsync();

            Assert.Single(res);
            Assert.Equal(80m, res[0].DisplayPrice);
        }

        [Fact]
        public async Task GetAvailableForPromotionAsync_ExcludesUsed()
        {
            var pdA = CreateFakeDetail();
            pdA.Id = Guid.NewGuid();
            pdA.Status = ProductDetailStatus.Active;

            var pdB = CreateFakeDetail();
            pdB.Id = Guid.NewGuid();
            pdB.Status = ProductDetailStatus.Active;

            // Tạo Product & Brand/Category để navigation không null
            var product = new Product { Id = Guid.NewGuid(), Name = "P1", BrandId = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
            product.Brand = new Brand { Id = product.BrandId, Name = "Brand1", Code = "BR1" };
            product.Category = new Category { Id = product.CategoryId, Name = "Cat1" };

            pdA.ProductId = product.Id;
            pdA.Product = product;
            pdB.ProductId = product.Id;
            pdB.Product = product;

            var promo = new Promotion
            {
                Id = Guid.NewGuid(),
                Name = "Promo_Avail",
                DiscountType = DiscountType.FixedAmount,
                DiscountValue = 5m,
                Status = VoucherStatus.Active,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            };

            var pp = new PromotionProduct
            {
                Id = Guid.NewGuid(),
                ProductDetailId = pdA.Id,
                PromotionId = promo.Id,
                ProductDetail = pdA,
                Promotion = promo
            };

            _context.Products.Add(product);
            _context.ProductDetails.AddRange(pdA, pdB);
            _context.Promotions.Add(promo);
            _context.PromotionProducts.Add(pp);
            await _context.SaveChangesAsync();

            var available = await _service.GetAvailableForPromotionAsync();

            Assert.Single(available);
            Assert.Contains(pdB.Id, available.Select(x => x.Id));

        }

        [Fact]
        public async Task GetByPromotionIdAsync_ValidAndInvalid()
        {
            var promo = new Promotion
            {
                Id = Guid.NewGuid(),
                Name = "Promo_ForGetById",
                DiscountType = DiscountType.FixedAmount,
                DiscountValue = 10m,
                Status = VoucherStatus.Active,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            };

            // ProductDetail
            var pd = CreateFakeDetail();
            pd.Id = Guid.NewGuid();                  // ✅ set Id
            pd.Status = ProductDetailStatus.Active;  // nếu service filter theo Status

            // Nếu service dùng Product navigation, gán Product hợp lệ
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "P1",
                BrandId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid()
            };
            product.Brand = new Brand { Id = product.BrandId, Name = "B1", Code = "BR1" };
            product.Category = new Category { Id = product.CategoryId, Name = "Cat1" };
            pd.ProductId = product.Id;
            pd.Product = product;

            // PromotionProduct liên kết
            var pp = new PromotionProduct
            {
                Id = Guid.NewGuid(),
                Promotion = promo,
                ProductDetail = pd,
                ProductDetailId = pd.Id,
                PromotionId = promo.Id
            };

            _context.Products.Add(product);
            _context.ProductDetails.Add(pd);
            _context.Promotions.Add(promo);
            _context.PromotionProducts.Add(pp);
            await _context.SaveChangesAsync();

            // Test với Guid.Empty
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByPromotionIdAsync(Guid.Empty));

            // Test với Promotion hợp lệ
            var res = await _service.GetByPromotionIdAsync(promo.Id);
            Assert.Single(res);
            Assert.Contains(pd.Id, res.Select(x => x.Id));

        }

        [Fact]
        public async Task ImportProductDetailFromExcelAsync_CreatesRowsFromExcel()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "ExcelProduct", BrandId = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var tmp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Sheet1");
                    ws.Cell(1, 1).Value = "Code";
                    ws.Cell(1, 2).Value = "Price";
                    ws.Cell(1, 3).Value = "Quantity";
                    ws.Cell(1, 4).Value = "SpecificationId";
                    ws.Cell(1, 5).Value = "ColorId";
                    ws.Cell(1, 6).Value = "SizeId";

                    ws.Cell(2, 1).Value = "X1";
                    ws.Cell(2, 2).Value = "123.45";
                    ws.Cell(2, 3).Value = "2";
                    ws.Cell(2, 4).Value = Guid.NewGuid().ToString();
                    ws.Cell(2, 5).Value = Guid.NewGuid().ToString();
                    ws.Cell(2, 6).Value = Guid.NewGuid().ToString();

                    wb.SaveAs(tmp);
                }

                var result = await _service.ImportProductDetailFromExcelAsync(tmp, product.Id);

                Assert.Equal("Import thành công.", result);
                Assert.Single(_context.ProductDetails);
            }
            finally
            {
                if (File.Exists(tmp))
                    File.Delete(tmp);
            }
        }
    }
}
