using System;
using System.Linq;
using System.Globalization;
using Marketplace.BusinessLogic.Models;

namespace Marketplace.BusinessLogic.Services
{
    /// <summary>
    /// Сервис для управления промокодами и купонами
    /// </summary>
    public class PromoCodeService
    {
        /// <summary>
        /// Проверяет действительность промокода
        /// </summary>
        public (bool IsValid, string Message) ValidatePromoCode(PromoCode promoCode, decimal totalAmount, int userId, int userUsageCount)
        {
            // Проверка: промокод существует
            if (promoCode == null)
                return (false, "Промокод не найден");

            // Проверка: активен ли промокод
            if (!promoCode.IsActive)
                return (false, "Промокод не активен");

            // Проверка: дата начала действия
            if (DateTime.Now.Date < promoCode.StartDate.Date)
                return (false, $"Промокод начнёт действовать с {promoCode.StartDate:dd.MM.yyyy}");

            // Проверка: дата окончания действия
            if (DateTime.Now.Date > promoCode.EndDate.Date)
                return (false, "Срок действия промокода истёк");

            // Проверка: минимальная сумма заказа
            if (totalAmount < promoCode.MinOrderAmount)
                return (false, $"Минимальная сумма заказа: {promoCode.MinOrderAmount:N0} ₽");

            // Проверка: повторное использование одним пользователем (сначала эта проверка)
            if (userUsageCount > 0)
                return (false, "Вы уже использовали этот промокод");

            // Проверка: лимит использования
            if (promoCode.UsageLimit.HasValue && userUsageCount >= promoCode.UsageLimit.Value)
                return (false, "Промокод больше не действителен");

            return (true, "Промокод применён успешно");
        }

        /// <summary>
        /// Рассчитывает скидку по промокоду
        /// </summary>
        public decimal CalculateDiscount(PromoCode promoCode, decimal totalAmount)
        {
            if (promoCode == null || totalAmount <= 0)
                return 0;

            if (promoCode.DiscountType == DiscountTypes.Percentage)
            {
                var discount = totalAmount * (promoCode.DiscountValue / 100);
                return Math.Round(discount, 2);
            }
            else if (promoCode.DiscountType == DiscountTypes.Fixed)
            {
                return Math.Min(promoCode.DiscountValue, totalAmount);
            }

            return 0;
        }

        /// <summary>
        /// Применяет промокод к заказу
        /// </summary>
        public decimal ApplyPromoCode(PromoCode promoCode, decimal totalAmount, decimal currentDiscount)
        {
            var promoDiscount = CalculateDiscount(promoCode, totalAmount);
            return currentDiscount + promoDiscount;
        }

        /// <summary>
        /// Формирует сообщение о результате применения промокода
        /// </summary>
        public string GenerateMessage(PromoCode promoCode, decimal discountAmount)
        {
            if (promoCode == null)
                return "Промокод не найден";

            var discountStr = discountAmount.ToString("N0", CultureInfo.InvariantCulture).Replace(",", "");

            if (promoCode.DiscountType == DiscountTypes.Percentage)
            {
                return $"Промокод {promoCode.Code} применён! Скидка {promoCode.DiscountValue}% = {discountStr} ₽";
            }
            else
            {
                return $"Промокод {promoCode.Code} применён! Скидка {discountStr} ₽";
            }
        }
    }
}