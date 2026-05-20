using System;
using Xunit;
using Marketplace.BusinessLogic.Models;
using Marketplace.BusinessLogic.Services;

namespace Marketplace.Tests
{
    public class PromoCodeTests
    {
        private readonly PromoCodeService _promoCodeService;

        public PromoCodeTests()
        {
            _promoCodeService = new PromoCodeService();
        }

        /// <summary>
        /// Тест 1: Проверка действительности промокода
        /// </summary>
        [Fact]
        public void Test1_ValidatePromoCode_ShouldValidateCorrectly()
        {
            // Arrange
            var validPromoCode = new PromoCode
            {
                Code = "TEST10",
                DiscountType = DiscountTypes.Percentage,
                DiscountValue = 10,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1),
                MinOrderAmount = 500,
                IsActive = true
            };

            var expiredPromoCode = new PromoCode
            {
                Code = "EXPIRED",
                DiscountType = DiscountTypes.Percentage,
                DiscountValue = 10,
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-1),
                MinOrderAmount = 0,
                IsActive = true
            };

            var inactivePromoCode = new PromoCode
            {
                Code = "INACTIVE",
                DiscountType = DiscountTypes.Percentage,
                DiscountValue = 10,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1),
                MinOrderAmount = 0,
                IsActive = false
            };

            // Act
            var validResult = _promoCodeService.ValidatePromoCode(validPromoCode, 1000, 1, 0);
            var expiredResult = _promoCodeService.ValidatePromoCode(expiredPromoCode, 1000, 1, 0);
            var inactiveResult = _promoCodeService.ValidatePromoCode(inactivePromoCode, 1000, 1, 0);

            // Assert
            Assert.True(validResult.IsValid);
            Assert.False(expiredResult.IsValid);
            Assert.Contains("истёк", expiredResult.Message);
            Assert.False(inactiveResult.IsValid);
            Assert.Contains("активен", inactiveResult.Message);
        }

        /// <summary>
        /// Тест 2: Проверка минимальной суммы заказа
        /// </summary>
        [Fact]
        public void Test2_ValidatePromoCode_MinOrderAmountCheck()
        {
            // Arrange
            var promoCode = new PromoCode
            {
                Code = "MIN500",
                DiscountType = DiscountTypes.Percentage,
                DiscountValue = 10,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1),
                MinOrderAmount = 5000,
                IsActive = true
            };

            // Act
            var smallOrderResult = _promoCodeService.ValidatePromoCode(promoCode, 1000, 1, 0);
            var exactOrderResult = _promoCodeService.ValidatePromoCode(promoCode, 5000, 1, 0);
            var largeOrderResult = _promoCodeService.ValidatePromoCode(promoCode, 10000, 1, 0);

            // Assert
            Assert.False(smallOrderResult.IsValid);
            Assert.True(exactOrderResult.IsValid);
            Assert.True(largeOrderResult.IsValid);
        }

        /// <summary>
        /// Тест 3: Проверка применения скидки
        /// </summary>
        [Theory]
        [InlineData(DiscountTypes.Percentage, 10, 1000, 100)]
        [InlineData(DiscountTypes.Percentage, 25, 1000, 250)]
        [InlineData(DiscountTypes.Fixed, 500, 1000, 500)]
        [InlineData(DiscountTypes.Fixed, 1500, 1000, 1000)]
        [InlineData(DiscountTypes.Percentage, 50, 0, 0)]
        [InlineData(DiscountTypes.Fixed, 100, 0, 0)]
        public void Test3_CalculateDiscount_ShouldReturnCorrectAmount(
            string discountType, decimal discountValue, decimal totalAmount, decimal expectedDiscount)
        {
            // Arrange
            var promoCode = new PromoCode
            {
                Code = "TEST",
                DiscountType = discountType,
                DiscountValue = discountValue
            };

            // Act
            var discount = _promoCodeService.CalculateDiscount(promoCode, totalAmount);

            // Assert
            Assert.Equal(expectedDiscount, discount);
        }

        /// <summary>
        /// Тест 4: Запрет повторного использования промокода одним пользователем
        /// </summary>
        [Fact]
        public void Test4_ValidatePromoCode_ShouldPreventDuplicateUsage()
        {
            // Arrange
            var promoCode = new PromoCode
            {
                Code = "ONCE",
                DiscountType = DiscountTypes.Percentage,
                DiscountValue = 10,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1),
                MinOrderAmount = 0,
                IsActive = true,
                UsageLimit = 1
            };

            // Act
            var firstUsageResult = _promoCodeService.ValidatePromoCode(promoCode, 1000, 1, 0);
            var secondUsageResult = _promoCodeService.ValidatePromoCode(promoCode, 1000, 1, 1);

            // Assert
            Assert.True(firstUsageResult.IsValid, "Первое использование должно быть разрешено");
            Assert.False(secondUsageResult.IsValid, "Второе использование должно быть запрещено");
            Assert.Contains("уже использовали", secondUsageResult.Message);
        }

        /// <summary>
        /// Тест 5: Формирование сообщения о результате применения промокода
        /// </summary>
        [Theory]
        [InlineData(DiscountTypes.Percentage, 10, 100, "10% = 100")]
        [InlineData(DiscountTypes.Percentage, 15, 300, "15% = 300")]
        [InlineData(DiscountTypes.Fixed, 500, 500, "500")]
        [InlineData(DiscountTypes.Fixed, 1000, 1000, "1000")]
        public void Test5_GenerateMessage_ShouldReturnCorrectMessage(
            string discountType, decimal discountValue, decimal discountAmount, string expectedPart)
        {
            // Arrange
            var promoCode = new PromoCode
            {
                Code = "TEST",
                DiscountType = discountType,
                DiscountValue = discountValue
            };

            // Act
            var message = _promoCodeService.GenerateMessage(promoCode, discountAmount);

            // Assert
            Assert.Contains(promoCode.Code, message);
            Assert.Contains(expectedPart, message);
        }

    }
}