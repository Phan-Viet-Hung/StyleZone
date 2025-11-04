using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Domain.Request.OrderRequest;
using API.Domain.Service.IService;
using DAL_Empty.Models;
using DomainAPI.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; // added for DatabaseFacade
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace API.Domain.Tests
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreatePosOrderAsync_Should_CreateOrder_And_Call_ProductDetailService()
        {
            // arrange
            var paymentMethodId = Guid.NewGuid();
            var paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { Id = paymentMethodId, Name = "Thanh toán khi nhận hàng (COD)" }
            };
            var orders = new List<OrderInfo>();
            var emptyProducts = new List<ProductDetail>();
            var orderHistories = new List<OrderHistory>();

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(c => c.PaymentMethods).ReturnsDbSet(paymentMethods);
            ctxMock.Setup(c => c.OrderInfos).ReturnsDbSet(orders);
            ctxMock.Setup(c => c.ProductDetails).ReturnsDbSet(emptyProducts);
            ctxMock.Setup(c => c.OrderHistories).ReturnsDbSet(orderHistories);
            ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Setup transaction on DatabaseFacade
            var tranMock = new Mock<IDbContextTransaction>();
            tranMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            tranMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var databaseFacadeMock = new Mock<DatabaseFacade>(ctxMock.Object);
            databaseFacadeMock.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tranMock.Object);

            ctxMock.SetupGet(c => c.Database).Returns(databaseFacadeMock.Object);

            var prodDetailServiceMock = new Mock<IProductDetailService>();
            prodDetailServiceMock.Setup(p => p.UpdateProductQuantityAfterOrderAsync(It.IsAny<List<OrderDetail>>()))
                .Returns(Task.CompletedTask);

            var sut = new OrderService(ctxMock.Object, prodDetailServiceMock.Object);

            var req = new CreateOrderRequest
            {
                CustomerName = "Test",
                PhoneNumber = "123",
                Address = "Addr",
                ShippingFee = 0,
                TotalAmount = 100,
                Description = "Desc",
                OrderDetails = new List<CreatePosOrderDetailRequest>
                {
                    new CreatePosOrderDetailRequest { ProductDetailId = Guid.NewGuid(), Quantity = 1, Price = 100 }
                }
            };

            // act
            var dto = await sut.CreatePosOrderAsync(req, Guid.NewGuid());

            // assert
            Assert.NotNull(dto);
            prodDetailServiceMock.Verify(p => p.UpdateProductQuantityAfterOrderAsync(It.IsAny<List<OrderDetail>>()), Times.Once);
            ctxMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetAllOrdersAsync_Should_ReturnMappedList()
        {
            // arrange
            var now = DateTime.UtcNow;
            var orders = new List<OrderInfo>
            {
                new OrderInfo { Id = Guid.NewGuid(), CreateAt = now, Status = OrderStatus.Pending, OrderDetails = new List<OrderDetail>(), BillHistories = new List<OrderHistory>() }
            };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(c => c.OrderInfos).ReturnsDbSet(orders);

            var sut = new OrderService(ctxMock.Object, Mock.Of<IProductDetailService>());

            // act
            var result = await sut.GetAllOrdersAsync();

            // assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_ReturnNull_When_NotFound()
        {
            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(c => c.OrderInfos).ReturnsDbSet(new List<OrderInfo>());

            var sut = new OrderService(ctxMock.Object, Mock.Of<IProductDetailService>());

            var res = await sut.GetOrderByIdAsync(Guid.NewGuid());
            Assert.Null(res);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_CalculateTotal_From_OrderDetails()
        {
            var orderId = Guid.NewGuid();
            var order = new OrderInfo
            {
                Id = orderId,
                ShippingFee = 5,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { Id = Guid.NewGuid(), Price = 10, Quantity = 2, ProductDetail = new ProductDetail { Name = "p" } }
                },
                BillHistories = new List<OrderHistory>()
            };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(c => c.OrderInfos).ReturnsDbSet(new[] { order });

            var sut = new OrderService(ctxMock.Object, Mock.Of<IProductDetailService>());

            var res = await sut.GetOrderByIdAsync(orderId);

            Assert.NotNull(res);
            Assert.Equal(25, res.TotalAmount);
        }

        [Fact]
        public async Task DeleteOrderAsync_Should_ReturnFalse_When_NotFound()
        {
            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(c => c.OrderInfos).ReturnsDbSet(new List<OrderInfo>());

            // Make FindAsync return null
            ctxMock.Setup(c => c.OrderInfos.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((OrderInfo?)null);

            var sut = new OrderService(ctxMock.Object, Mock.Of<IProductDetailService>());

            var res = await sut.DeleteOrderAsync(Guid.NewGuid());
            Assert.False(res);
        }

        [Fact]
        public async Task DeleteOrderAsync_Should_Remove_And_Save_When_Found()
        {
            var order = new OrderInfo { Id = Guid.NewGuid() };
            var orders = new List<OrderInfo> { order };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(c => c.OrderInfos).ReturnsDbSet(orders);
            ctxMock.Setup(c => c.OrderInfos.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => orders.FirstOrDefault(o => o.Id == (Guid)ids[0]));
            ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var sut = new OrderService(ctxMock.Object, Mock.Of<IProductDetailService>());

            var res = await sut.DeleteOrderAsync(order.Id);
            Assert.True(res);
            ctxMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_Should_Throw_When_OrderNotFound()
        {
            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(c => c.OrderInfos).ReturnsDbSet(new List<OrderInfo>());

            var sut = new OrderService(ctxMock.Object, Mock.Of<IProductDetailService>());

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateOrderStatusAsync(Guid.NewGuid(), OrderStatus.Confirmed, Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_PendingToConfirmed_Should_DecreaseProductQuantity()
        {
            var prodId = Guid.NewGuid();
            var order = new OrderInfo
            {
                Id = Guid.NewGuid(),
                Status = OrderStatus.Pending,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { ProductDetailId = prodId, Quantity = 2 }
                },
                OrderPaymentMethods = new List<OrderPaymentMethod>
                {
                    new OrderPaymentMethod { PaymentMethod = new PaymentMethod { Name = "Thanh toán khi nhận hàng (COD)" } }
                },
                TotalAmount = 100,
                BillHistories = new List<OrderHistory>()
            };

            var product = new ProductDetail { Id = prodId, Quantity = 5, Name = "P" };

            var ctxMock = new Mock<DbContextApp>();
            ctxMock.Setup(c => c.OrderInfos).ReturnsDbSet(new[] { order });
            ctxMock.Setup(c => c.ProductDetails).ReturnsDbSet(new[] { product });
            ctxMock.Setup(c => c.OrderHistories).ReturnsDbSet(new List<OrderHistory>());
            ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var sut = new OrderService(ctxMock.Object, Mock.Of<IProductDetailService>());

            var res = await sut.UpdateOrderStatusAsync(order.Id, OrderStatus.Confirmed, Guid.NewGuid());

            Assert.True(res);
            Assert.Equal(3, product.Quantity); // 5 - 2
            ctxMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusBulkAsync_Should_Throw_On_InvalidInputs_And_UPDATE_On_Success()
        {
            var ctxMock = new Mock<DbContextApp>();
            var sut = new OrderService(ctxMock.Object, Mock.Of<IProductDetailService>());

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateOrderStatusBulkAsync(null!, OrderStatus.Confirmed, Guid.NewGuid()));
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.UpdateOrderStatusBulkAsync(new List<Guid>(), OrderStatus.Confirmed, Guid.NewGuid()));

            // setup two orders that can be confirmed (Pending + COD) with sufficient stock
            var prodId = Guid.NewGuid();
            var product = new ProductDetail { Id = prodId, Quantity = 10, Name = "P1" };

            var order1 = new OrderInfo
            {
                Id = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow.AddDays(-2),
                Status = OrderStatus.Pending,
                OrderDetails = new List<OrderDetail> { new OrderDetail { ProductDetailId = prodId, Quantity = 2 } },
                OrderPaymentMethods = new List<OrderPaymentMethod> { new OrderPaymentMethod { PaymentMethod = new PaymentMethod { Name = "Thanh toán khi nhận hàng (COD)" } } },
                TotalAmount = 50,
                BillHistories = new List<OrderHistory>()
            };

            var order2 = new OrderInfo
            {
                Id = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Pending,
                OrderDetails = new List<OrderDetail> { new OrderDetail { ProductDetailId = prodId, Quantity = 3 } },
                OrderPaymentMethods = new List<OrderPaymentMethod> { new OrderPaymentMethod { PaymentMethod = new PaymentMethod { Name = "Thanh toán khi nhận hàng (COD)" } } },
                TotalAmount = 75,
                BillHistories = new List<OrderHistory>()
            };

            var orders = new List<OrderInfo> { order1, order2 };

            ctxMock.Setup(c => c.OrderInfos).ReturnsDbSet(orders);
            ctxMock.Setup(c => c.ProductDetails).ReturnsDbSet(new[] { product });
            ctxMock.Setup(c => c.OrderHistories).ReturnsDbSet(new List<OrderHistory>());
            ctxMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var (updatedCount, errors) = await sut.UpdateOrderStatusBulkAsync(orders.Select(o => o.Id).ToList(), OrderStatus.Confirmed, Guid.NewGuid());

            Assert.Equal(2, updatedCount);
            Assert.Empty(errors);
            Assert.Equal(5, product.Quantity); // 10 - (2 + 3)
        }
    }
}