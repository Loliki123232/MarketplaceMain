using System;

namespace Marketplace.BusinessLogic.Models
{
    /// <summary>
    /// Представляет продавца в системе маркетплейса
    /// </summary>
    public class Seller
    {
        /// <summary>
        /// Уникальный идентификатор продавца
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Название магазина
        /// </summary>
        public string StoreName { get; set; } = string.Empty;

        /// <summary>
        /// Описание магазина
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Рейтинг продавца
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}