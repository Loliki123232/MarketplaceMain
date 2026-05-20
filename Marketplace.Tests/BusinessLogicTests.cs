using Xunit;
using Marketplace.BusinessLogic.Models;
using Marketplace.BusinessLogic.Services;
using System.Collections.Generic;

namespace Marketplace.Tests
{
    /// <summary>
    /// Модульные тесты для проверки бизнес-логики маркетплейса
    /// </summary>
    public class BusinessLogicTests
    {
        private readonly PriceCalculator _priceCalculator;
        private readonly StockValidator _stockValidator;
        private readonly ReceiptGenerator _receiptGenerator;

        public BusinessLogicTests()
        {
            _priceCalculator = new PriceCalculator();
            _stockValidator = new StockValidator();
            _receiptGenerator = new ReceiptGenerator();
        }

        /// <summary>
        /// Тест 1: Проверка расчёта итоговой стоимости заказа
        /// </summary>
        [Fact]
        public void CalculateTotalAmount_WithMultipleItems_ReturnsCorrectSum()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new() { ProductId = 1, Quantity = 2, UnitPrice = 1000 },
                new() { ProductId = 2, Quantity = 1, UnitPrice = 500 },
                new() { ProductId = 3, Quantity = 3, UnitPrice = 200 }
            };

            // Act
            var total = _priceCalculator.CalculateTotalAmount(items);

            // Assert
            Assert.Equal(3100, total);
        }

        /// <summary>
        /// Тест 2: Проверка расчёта скидки при разных суммах заказа
        /// </summary>
        [Theory]
        [InlineData(500, 0)]
        [InlineData(1000, 50)]
        [InlineData(5000, 500)]
        [InlineData(20000, 3000)]
        [InlineData(30000, 4500)]
        public void CalculateDiscount_WithDifferentAmounts_ReturnsCorrectDiscount(decimal totalAmount, decimal expectedDiscount)
        {
            // Act
            var discount = _priceCalculator.CalculateDiscount(totalAmount);

            // Assert
            Assert.Equal(expectedDiscount, discount);
        }

        /// <summary>
        /// Тест 3: Проверка наличия товара на складе
        /// </summary>
        [Theory]
        [InlineData(10, 5, true)]
        [InlineData(10, 10, true)]
        [InlineData(10, 15, false)]
        [InlineData(0, 1, false)]
        public void IsProductAvailable_WithDifferentStock_ReturnsCorrectResult(int stock, int requested, bool expected)
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Тестовый товар", StockQuantity = stock };

            // Act
            var isAvailable = _stockValidator.IsProductAvailable(product, requested);

            // Assert
            Assert.Equal(expected, isAvailable);
        }

        /// <summary>
        /// Тест 4: Проверка формирования чека
        /// </summary>
        [Fact]
        public void GenerateReceipt_WithValidOrder_ReturnsNonEmptyReceipt()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                UserName = "Тестовый пользователь",
                OrderDate = System.DateTime.Now,
                Items = new List<OrderItem>
                {
                    new() { ProductName = "Тестовый товар", Quantity = 2, UnitPrice = 1000 }
                }
            };

            // Act
            var receipt = _receiptGenerator.GenerateReceipt(order);

            // Assert
            Assert.NotNull(receipt);
            Assert.Contains("ЧЕК ЗАКАЗА", receipt);
            Assert.Contains("Тестовый товар", receipt);
        }
    }
}
