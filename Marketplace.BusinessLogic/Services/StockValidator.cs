using System.Collections.Generic;
using System.Linq;
using Marketplace.BusinessLogic.Models;

namespace Marketplace.BusinessLogic.Services
{
    /// <summary>
    /// Сервис для проверки наличия товаров на складе
    /// </summary>
    public class StockValidator
    {
        /// <summary>
        /// Проверяет, есть ли товар в указанном количестве на складе
        /// </summary>
        /// <param name="product">Проверяемый товар</param>
        /// <param name="requestedQuantity">Запрашиваемое количество</param>
        /// <returns>true - товар есть в наличии, false - недостаточно</returns>
        public bool IsProductAvailable(Product product, int requestedQuantity)
        {
            if (product == null)
                return false;

            return product.StockQuantity >= requestedQuantity && requestedQuantity > 0;
        }

        /// <summary>
        /// Проверяет наличие всех товаров в заказе
        /// </summary>
        /// <param name="items">Список товаров в заказе</param>
        /// <param name="products">Словарь товаров по ID</param>
        /// <returns>Словарь с результатами проверки для каждого товара</returns>
        public Dictionary<int, bool> CheckOrderAvailability(List<OrderItem> items, Dictionary<int, Product> products)
        {
            var result = new Dictionary<int, bool>();

            foreach (var item in items)
            {
                if (products.TryGetValue(item.ProductId, out var product))
                {
                    result[item.ProductId] = IsProductAvailable(product, item.Quantity);
                }
                else
                {
                    result[item.ProductId] = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Проверяет, все ли товары в заказе доступны
        /// </summary>
        /// <param name="items">Список товаров в заказе</param>
        /// <param name="products">Словарь товаров по ID</param>
        /// <returns>true - все товары доступны, false - есть недоступные</returns>
        public bool IsOrderAvailable(List<OrderItem> items, Dictionary<int, Product> products)
        {
            var checkResult = CheckOrderAvailability(items, products);
            return checkResult.All(x => x.Value);
        }
    }
}