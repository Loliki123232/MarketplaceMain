using System;
using System.Collections.Generic;
using Xunit;
using Marketplace.BusinessLogic.Models;
using Marketplace.BusinessLogic.Services;

namespace Marketplace.Tests
{
    public class ReturnModuleTests
    {
        private readonly ReturnValidator _validator;
        private readonly ReturnCalculator _calculator;

        public ReturnModuleTests()
        {
            _validator = new ReturnValidator();
            _calculator = new ReturnCalculator();
        }

        /// <summary>
        /// Тест 1: Создание заявки на возврат
        /// </summary>
        [Fact]
        public void Test1_CreateReturnRequest_ShouldHaveCorrectProperties()
        {
            // Arrange
            var returnRequest = new ReturnRequest
            {
                Id = 1,
                OrderId = 100,
                ProductId = 5,
                UserId = 1,
                Reason = "Товар не подошёл по размеру",
                Quantity = 1,
                ReturnAmount = 29990,
                Status = ReturnStatuses.Created
            };

            // Assert
            Assert.Equal(1, returnRequest.Id);
            Assert.Equal(100, returnRequest.OrderId);
            Assert.Equal(5, returnRequest.ProductId);
            Assert.Equal("Товар не подошёл по размеру", returnRequest.Reason);
            Assert.Equal(ReturnStatuses.Created, returnRequest.Status);
        }

        /// <summary>
        /// Тест 2: Проверка возможности возврата
        /// </summary>
        [Fact]
        public void Test2_CanReturnProduct_ShouldValidateCorrectly()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                OrderDate = DateTime.Now.AddDays(-5),
                Status = "delivered"
            };
            var orderItem = new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 1000 };

            // Act - возврат возможен
            var validReturn = _validator.CanReturnProduct(order, orderItem, 1);

            // Act - возврат невозможен (больше чем заказано)
            var invalidQuantity = _validator.CanReturnProduct(order, orderItem, 3);

            // Assert
            Assert.True(validReturn.IsValid);
            Assert.False(invalidQuantity.IsValid);
            Assert.Contains("больше", invalidQuantity.Message);
        }

        /// <summary>
        /// Тест 3: Расчёт суммы возврата
        /// </summary>
        [Fact]
        public void Test3_CalculateReturnAmount_ShouldReturnCorrectSum()
        {
            // Arrange
            var orderItem = new OrderItem { ProductId = 1, Quantity = 5, UnitPrice = 1000 };

            // Act
            var amount1 = _calculator.CalculateReturnAmount(orderItem, 2);
            var amount2 = _calculator.CalculateReturnAmount(orderItem, 5);

            // Assert
            Assert.Equal(2000, amount1);
            Assert.Equal(5000, amount2);
        }

        /// <summary>
        /// Тест 4: Изменение статуса заявки
        /// </summary>
        [Fact]
        public void Test4_ChangeReturnStatus_ShouldUpdateCorrectly()
        {
            // Arrange
            var returnRequest = new ReturnRequest
            {
                Id = 1,
                Status = ReturnStatuses.Created
            };

            // Act
            returnRequest.Status = ReturnStatuses.Approved;

            // Assert
            Assert.Equal(ReturnStatuses.Approved, returnRequest.Status);
            Assert.NotEqual(ReturnStatuses.Created, returnRequest.Status);
        }

        /// <summary>
        /// Тест 5: Восстановление количества товара на складе
        /// </summary>
        [Fact]
        public void Test5_RestoreStock_ShouldIncreaseStockQuantity()
        {
            // Arrange
            var product = new Product { Id = 1, StockQuantity = 10 };
            int returnQuantity = 2;
            int expectedStock = product.StockQuantity + returnQuantity;

            // Act
            product.StockQuantity += returnQuantity;

            // Assert
            Assert.Equal(expectedStock, product.StockQuantity);
        }
    }
}