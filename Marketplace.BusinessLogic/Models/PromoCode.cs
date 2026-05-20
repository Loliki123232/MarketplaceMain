using System;

namespace Marketplace.BusinessLogic.Models
{
    /// <summary>
    /// Тип скидки промокода
    /// </summary>
    public static class DiscountTypes
    {
        public const string Percentage = "Percentage"; // Процентная скидка
        public const string Fixed = "Fixed";           // Фиксированная скидка
    }

    /// <summary>
    /// Представляет промокод для скидки на заказ
    /// </summary>
    public class PromoCode
    {
        /// <summary>
        /// Уникальный идентификатор промокода
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Код промокода для ввода пользователем
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Тип скидки (Percentage - процентная, Fixed - фиксированная)
        /// </summary>
        public string DiscountType { get; set; } = string.Empty;

        /// <summary>
        /// Значение скидки (процент или сумма в рублях)
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// Дата начала действия промокода
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания действия промокода
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Минимальная сумма заказа для применения промокода
        /// </summary>
        public decimal MinOrderAmount { get; set; }

        /// <summary>
        /// Активен ли промокод
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Лимит использования промокода (null - безлимитный)
        /// </summary>
        public int? UsageLimit { get; set; }

        /// <summary>
        /// Дата создания промокода
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Проверяет, действителен ли промокод на текущий момент
        /// </summary>
        public bool IsValid()
        {
            var now = DateTime.Now;
            return IsActive && now >= StartDate && now <= EndDate;
        }

        /// <summary>
        /// Проверяет, превышен ли лимит использования
        /// </summary>
        /// <param name="currentUsageCount">Текущее количество использований</param>
        public bool IsUsageLimitExceeded(int currentUsageCount)
        {
            return UsageLimit.HasValue && currentUsageCount >= UsageLimit.Value;
        }
    }

    /// <summary>
    /// Представляет использование промокода пользователем
    /// </summary>
    public class UserPromoCode
    {
        public int Id { get; set; }
        public int PromoCodeId { get; set; }
        public int UserId { get; set; }
        public int? OrderId { get; set; }
        public DateTime UsedAt { get; set; }
        public decimal DiscountApplied { get; set; }
    }

    /// <summary>
    /// Результат применения промокода
    /// </summary>
    public class PromoCodeResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public PromoCode? AppliedPromoCode { get; set; }
    }
}