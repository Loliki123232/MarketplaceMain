using System;

namespace Marketplace.BusinessLogic.Models
{
    /// <summary>
    /// Представляет товар в системе маркетплейса
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Уникальный идентификатор товара
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание товара
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор категории
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Цена товара в рублях
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Количество на складе
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Идентификатор продавца
        /// </summary>
        public int SellerId { get; set; }

        /// <summary>
        /// Название магазина продавца
        /// </summary>
        public string StoreName { get; set; } = string.Empty;

        /// <summary>
        /// Рейтинг товара (от 0 до 5)
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// URL изображения товара
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
    }
}