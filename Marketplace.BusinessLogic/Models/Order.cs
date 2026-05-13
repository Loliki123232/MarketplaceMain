using System;
using System.Collections.Generic;

namespace Marketplace.BusinessLogic.Models
{
    /// <summary>
    /// Представляет заказ пользователя
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Уникальный идентификатор заказа
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Дата создания заказа
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Список товаров в заказе
        /// </summary>
        public List<OrderItem> Items { get; set; } = new();

        /// <summary>
        /// Статус заказа (pending, confirmed, shipped, delivered, cancelled)
        /// </summary>
        public string Status { get; set; } = "pending";

        /// <summary>
        /// Общая стоимость без скидки
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Сумма скидки
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Итоговая стоимость со скидкой
        /// </summary>
        public decimal FinalAmount { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        public string ShippingAddress { get; set; } = string.Empty;
    }
}