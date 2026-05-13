namespace Marketplace.BusinessLogic.Models
{
    /// <summary>
    /// Представляет категорию товаров
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Уникальный идентификатор категории
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор родительской категории
        /// </summary>
        public int? ParentCategoryId { get; set; }
    }
}