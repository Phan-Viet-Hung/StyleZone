using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using API.Domain.DTOs.ThongKe;
using API.Domain.Service;
using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace API.Domain.Tests
{
    public class StatisticServiceTests
    {
        private Mock<DbContextApp> CreateContextMock<T>(IEnumerable<T> items, Expression<Func<DbContextApp, DbSet<T>>> dbSetAccessor)
            where T : class
        {
            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(dbSetAccessor).ReturnsDbSet(items);
            return ctxMock;
        }

        // Helper để so sánh chuỗi tiếng Việt an toàn (loại bỏ dấu)
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            //return sb.ToString().Normalize(NormalizationForm.FormC);
            var result = sb.ToString().Normalize(NormalizationForm.FormC);

            // Thêm dòng này để xử lý ký tự "đ"
            return result.Replace('đ', 'd').Replace('Đ', 'D');
        }

        [Fact]
        public async Task GetDashboardStatisticsAsync_CustomWithData_ReturnsExpectedTotalsAndChart()
        {
            // Arrange - date range
            var start = DateTime.Now.Date.AddDays(-2);
            var end = DateTime.Now.Date;
            var filter = new DateFilterDto { FilterType = "custom", StartDate = start, EndDate = end };

            // Products, Categories, Customers counts
            var products = new List<Product> { new Product { Id = Guid.NewGuid() }, new Product { Id = Guid.NewGuid() } };
            var categories = new List<Category> { new Category { Id = Guid.NewGuid() } };
            var customers = new List<Customer> { new Customer { Id = Guid.NewGuid(), Status = "Active" }, new Customer { Id = Guid.NewGuid(), Status = "inactive" } };

            // Orders: one delivered order inside range, one other order outside
            var orderInRange = new OrderInfo { Id = Guid.NewGuid(), CreateAt = start.AddDays(1), Status = OrderStatus.Delivered };
            var orderOther = new OrderInfo { Id = Guid.NewGuid(), CreateAt = start.AddDays(-10), Status = OrderStatus.Pending };
            var orders = new List<OrderInfo> { orderInRange, orderOther };

            // OrderDetails: two details for delivered order
            var od1 = new OrderDetail { Id = Guid.NewGuid(), Order = orderInRange, Quantity = 2, Price = 10m };
            var od2 = new OrderDetail { Id = Guid.NewGuid(), Order = orderInRange, Quantity = 3, Price = 20m };
            var orderDetails = new List<OrderDetail> { od1, od2 };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.Products).ReturnsDbSet(products);
            ctxMock.Setup(x => x.Categories).ReturnsDbSet(categories);
            ctxMock.Setup(x => x.Customers).ReturnsDbSet(customers);
            ctxMock.Setup(x => x.OrderInfos).ReturnsDbSet(orders);
            ctxMock.Setup(x => x.OrderDetails).ReturnsDbSet(orderDetails);

            var service = new StatisticService(ctxMock.Object);

            // Act
            var result = await service.GetDashboardStatisticsAsync(filter);

            // Assert
            Assert.Equal(products.Count, result.TotalProducts);
            Assert.Equal(orders.Count, result.TotalOrders);
            Assert.Equal(categories.Count, result.TotalCategories);
            Assert.Equal(customers.Count(c => c.Status != null && c.Status.ToLower() == "active"), result.TotalUsers);

            // ChartData aggregated by date should contain the delivered order's date
            Assert.NotNull(result.ChartData);
            Assert.Single(result.ChartData);
            var chart = result.ChartData.First();
            Assert.Equal((od1.Quantity ?? 0) + (od2.Quantity ?? 0), chart.TotalQuantitySold);
            Assert.Equal((od1.Quantity ?? 0) * (od1.Price ?? 0) + (od2.Quantity ?? 0) * (od2.Price ?? 0), chart.TotalRevenue);
        }

        [Fact]
        public async Task GetDashboardStatisticsAsync_CustomNoOrders_ThrowsArgumentException()
        {
            // Arrange
            var start = DateTime.Now.Date.AddDays(-5);
            var end = DateTime.Now.Date;
            var filter = new DateFilterDto { FilterType = "custom", StartDate = start, EndDate = end };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.Products).ReturnsDbSet(new List<Product>());
            ctxMock.Setup(x => x.Categories).ReturnsDbSet(new List<Category>());
            ctxMock.Setup(x => x.Customers).ReturnsDbSet(new List<Customer>());
            // No orders
            ctxMock.Setup(x => x.OrderInfos).ReturnsDbSet(new List<OrderInfo>());
            ctxMock.Setup(x => x.OrderDetails).ReturnsDbSet(new List<OrderDetail>());

            var service = new StatisticService(ctxMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.GetDashboardStatisticsAsync(filter));
            Assert.Contains(RemoveDiacritics("Không tìm thấy đơn hàng nào"), RemoveDiacritics(ex.Message));
        }

        [Fact]
        public async Task GetDashboardStatisticsAsync_CustomNoRevenue_ThrowsArgumentException()
        {
            // Arrange: orders exist but no delivered order-details in range
            var start = DateTime.Now.Date.AddDays(-5);
            var end = DateTime.Now.Date;
            var filter = new DateFilterDto { FilterType = "custom", StartDate = start, EndDate = end };

            var orderInRange = new OrderInfo { Id = Guid.NewGuid(), CreateAt = start.AddDays(1), Status = OrderStatus.Pending };
            var orders = new List<OrderInfo> { orderInRange };

            // OrderDetails exist but not for delivered orders (or missing ProductDetail)
            var orderDetails = new List<OrderDetail> { new OrderDetail { Id = Guid.NewGuid(), Order = orderInRange, Quantity = 1, Price = 10m } };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.Products).ReturnsDbSet(new List<Product>());
            ctxMock.Setup(x => x.Categories).ReturnsDbSet(new List<Category>());
            ctxMock.Setup(x => x.Customers).ReturnsDbSet(new List<Customer>());
            ctxMock.Setup(x => x.OrderInfos).ReturnsDbSet(orders);
            ctxMock.Setup(x => x.OrderDetails).ReturnsDbSet(orderDetails);

            var service = new StatisticService(ctxMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.GetDashboardStatisticsAsync(filter));
            Assert.Contains(RemoveDiacritics("Không tìm thấy dữ liệu doanh thu"), RemoveDiacritics(ex.Message));
        }

        [Fact]
        public async Task GetOrderStatusStatisticsAsync_CustomWithData_ReturnsAllStatusesAndCounts()
        {
            // Arrange
            var start = DateTime.Now.Date.AddDays(-3);
            var end = DateTime.Now.Date;
            var filter = new DateFilterDto { FilterType = "custom", StartDate = start, EndDate = end };

            var o1 = new OrderInfo { Id = Guid.NewGuid(), CreateAt = start.AddDays(1), Status = OrderStatus.Pending };
            var o2 = new OrderInfo { Id = Guid.NewGuid(), CreateAt = start.AddDays(1), Status = OrderStatus.Delivered };
            var o3 = new OrderInfo { Id = Guid.NewGuid(), CreateAt = start.AddDays(1), Status = OrderStatus.Delivered };

            var orders = new List<OrderInfo> { o1, o2, o3 };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.OrderInfos).ReturnsDbSet(orders);

            var service = new StatisticService(ctxMock.Object);

            // Act
            var result = await service.GetOrderStatusStatisticsAsync(filter);

            // Assert: result contains entry per enum value
            var allStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();
            Assert.Equal(allStatuses.Count, result.Count);

            // Check counts for known statuses
            var pending = result.First(r => r.Status == OrderStatus.Pending);
            var delivered = result.First(r => r.Status == OrderStatus.Delivered);
            Assert.Equal(1, pending.TotalOrders);
            Assert.Equal(2, delivered.TotalOrders);

            // So sánh tên trạng thái bằng cách loại bỏ dấu để tránh vấn đề encoding
            Assert.Equal(RemoveDiacritics("Chờ xử lý"), RemoveDiacritics(pending.StatusName));
            Assert.Equal(RemoveDiacritics("Đã giao hàng"), RemoveDiacritics(delivered.StatusName));
        }

        [Fact]
        public async Task GetOrderStatusStatisticsAsync_CustomNoOrders_ThrowsArgumentException()
        {
            // Arrange
            var start = DateTime.Now.Date.AddDays(-3);
            var end = DateTime.Now.Date;
            var filter = new DateFilterDto { FilterType = "custom", StartDate = start, EndDate = end };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.OrderInfos).ReturnsDbSet(new List<OrderInfo>());

            var service = new StatisticService(ctxMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.GetOrderStatusStatisticsAsync(filter));
            Assert.Contains(RemoveDiacritics("Không tìm thấy đơn hàng nào"), RemoveDiacritics(ex.Message));
        }

        [Fact]
        public async Task GetTopBrandsAsync_ValidData_ReturnsTopBrandsOrdered()
        {
            // Arrange
            var start = DateTime.Now.Date.AddDays(-10);
            var end = DateTime.Now.Date;
            var filter = new DateFilterDto { FilterType = "custom", StartDate = start, EndDate = end };

            var brandA = new Brand { Id = Guid.NewGuid(), Name = "BrandA" };
            var brandB = new Brand { Id = Guid.NewGuid(), Name = "BrandB" };

            var product1 = new Product { Id = Guid.NewGuid(), BrandId = brandA.Id, Brand = brandA };
            var product2 = new Product { Id = Guid.NewGuid(), BrandId = brandB.Id, Brand = brandB };

            var pd1 = new ProductDetail { Id = Guid.NewGuid(), Product = product1 };
            var pd2 = new ProductDetail { Id = Guid.NewGuid(), Product = product2 };

            var deliveredOrder = new OrderInfo { Id = Guid.NewGuid(), CreateAt = start.AddDays(1), Status = OrderStatus.Delivered };

            var od1 = new OrderDetail { Id = Guid.NewGuid(), Order = deliveredOrder, ProductDetail = pd1, Quantity = 5, Price = 10m };
            var od2 = new OrderDetail { Id = Guid.NewGuid(), Order = deliveredOrder, ProductDetail = pd2, Quantity = 2, Price = 10m };
            var orderDetails = new List<OrderDetail> { od1, od2 };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.OrderDetails).ReturnsDbSet(orderDetails);
            ctxMock.Setup(x => x.OrderInfos).ReturnsDbSet(new List<OrderInfo> { deliveredOrder });

            var service = new StatisticService(ctxMock.Object);

            // Act
            var result = await service.GetTopBrandsAsync(filter, top: 2);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(brandA.Id, result[0].BrandId);
            Assert.Equal(5, result[0].TotalSold);
            Assert.Equal(brandB.Id, result[1].BrandId);
        }

        [Fact]
        public async Task GetTopBrandsAsync_TopLessOrEqualZero_ThrowsArgumentException()
        {
            // Arrange
            var filter = new DateFilterDto { FilterType = "7days" }; // not custom
            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.OrderDetails).ReturnsDbSet(new List<OrderDetail>());

            var service = new StatisticService(ctxMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetTopBrandsAsync(filter, top: 0));
        }

        [Fact]
        public async Task GetTopBrandsAsync_CustomNoData_ThrowsArgumentException()
        {
            // Arrange
            var start = DateTime.Now.Date.AddDays(-5);
            var end = DateTime.Now.Date;
            var filter = new DateFilterDto { FilterType = "custom", StartDate = start, EndDate = end };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.OrderDetails).ReturnsDbSet(new List<OrderDetail>());
            ctxMock.Setup(x => x.OrderInfos).ReturnsDbSet(new List<OrderInfo>());

            var service = new StatisticService(ctxMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.GetTopBrandsAsync(filter, top: 3));
            Assert.Contains(RemoveDiacritics("Không tìm thấy dữ liệu thương hiệu"), RemoveDiacritics(ex.Message));
        }

        [Fact]
        public async Task GetTopProductsAsync_ValidData_ReturnsTopProductsOrdered()
        {
            // Arrange
            var start = DateTime.Now.Date.AddDays(-10);
            var end = DateTime.Now.Date;
            var filter = new DateFilterDto { FilterType = "custom", StartDate = start, EndDate = end };

            var product1 = new Product { Id = Guid.NewGuid(), Name = "P1" };
            var product2 = new Product { Id = Guid.NewGuid(), Name = "P2" };

            var pd1 = new ProductDetail { Id = Guid.NewGuid(), Product = product1 };
            var pd2 = new ProductDetail { Id = Guid.NewGuid(), Product = product2 };

            var deliveredOrder = new OrderInfo { Id = Guid.NewGuid(), CreateAt = start.AddDays(1), Status = OrderStatus.Delivered };

            var od1 = new OrderDetail { Id = Guid.NewGuid(), Order = deliveredOrder, ProductDetail = pd1, Quantity = 7, Price = 10m };
            var od2 = new OrderDetail { Id = Guid.NewGuid(), Order = deliveredOrder, ProductDetail = pd2, Quantity = 3, Price = 10m };
            var orderDetails = new List<OrderDetail> { od1, od2 };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.OrderDetails).ReturnsDbSet(orderDetails);
            ctxMock.Setup(x => x.OrderInfos).ReturnsDbSet(new List<OrderInfo> { deliveredOrder });

            var service = new StatisticService(ctxMock.Object);

            // Act
            var result = await service.GetTopProductsAsync(filter, top: 2);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(product1.Id, result[0].ProductId);
            Assert.Equal(7, result[0].TotalSold);
            Assert.Equal(product2.Id, result[1].ProductId);
        }

        [Fact]
        public async Task GetTopProductsAsync_TopLessOrEqualZero_ThrowsArgumentException()
        {
            // Arrange
            var filter = new DateFilterDto { FilterType = "7days" };
            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.OrderDetails).ReturnsDbSet(new List<OrderDetail>());

            var service = new StatisticService(ctxMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetTopProductsAsync(filter, top: 0));
        }

        [Fact]
        public async Task GetTopProductsAsync_CustomNoData_ThrowsArgumentException()
        {
            // Arrange
            var start = DateTime.Now.Date.AddDays(-5);
            var end = DateTime.Now.Date;
            var filter = new DateFilterDto { FilterType = "custom", StartDate = start, EndDate = end };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(x => x.OrderDetails).ReturnsDbSet(new List<OrderDetail>());
            ctxMock.Setup(x => x.OrderInfos).ReturnsDbSet(new List<OrderInfo>());

            var service = new StatisticService(ctxMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.GetTopProductsAsync(filter, top: 5));
            Assert.Contains(RemoveDiacritics("Không tìm thấy dữ liệu sản phẩm"), RemoveDiacritics(ex.Message));
        }
    }
}