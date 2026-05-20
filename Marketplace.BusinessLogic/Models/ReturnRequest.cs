using System;

namespace Marketplace.BusinessLogic.Models
{
    /// <summary>
    /// Представляет заявку на возврат товара
    /// </summary>
    public class ReturnRequest
    {
        /// <summary>
        /// Уникальный идентификатор заявки
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Номер заказа (для отображения)
        /// </summary>
        public int OrderNumber { get; set; }

        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Количество товара для возврата
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Причина возврата
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Дата создания заявки
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Статус заявки (Создана, Одобрена, Отклонена)
        /// </summary>
        public string Status { get; set; } = "Создана";

        /// <summary>
        /// Сумма возврата
        /// </summary>
        public decimal ReturnAmount { get; set; }

        /// <summary>
        /// Цена товара на момент покупки
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Дата рассмотрения заявки
        /// </summary>
        public DateTime? ResolutionDate { get; set; }

        /// <summary>
        /// Комментарий при рассмотрении
        /// </summary>
        public string ResolutionComment { get; set; } = string.Empty;
    }

    /// <summary>
    /// Статусы заявки на возврат
    /// </summary>
    public static class ReturnStatuses
    {
        public const string Created = "Создана";
        public const string Approved = "Одобрена";
        public const string Rejected = "Отклонена";
    }
}