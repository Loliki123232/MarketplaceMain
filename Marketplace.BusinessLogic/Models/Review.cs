using System;

namespace Marketplace.BusinessLogic.Models
{
    /// <summary>
    /// Представляет отзыв на товар
    /// </summary>
    public class Review
    {
        /// <summary>
        /// Уникальный идентификатор отзыва
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Оценка (1-5)
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Текст отзыва
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}