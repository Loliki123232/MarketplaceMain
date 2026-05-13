using System.Collections.Generic;
using System.Linq;
using Marketplace.BusinessLogic.Models;

namespace Marketplace.BusinessLogic.Services
{
    /// <summary>
    /// Сервис для расчёта стоимости заказов и скидок
    /// </summary>
    public class PriceCalculator
    {
        /// <summary>
        /// Рассчитывает общую стоимость заказа на основе списка товаров
        /// </summary>
        /// <param name="items">Список товаров в заказе</param>
        /// <returns>Общая стоимость без учёта скидки</returns>
        public decimal CalculateTotalAmount(List<OrderItem> items)
        {
            if (items == null || !items.Any())
                return 0;

            return items.Sum(item => item.Subtotal);
        }

        /// <summary>
        /// Рассчитывает скидку на заказ
        /// </summary>
        /// <param name="totalAmount">Общая стоимость заказа</param>
        /// <returns>Сумма скидки</returns>
        /// <remarks>
        /// Правила начисления скидки:
        /// - Скидка 5% при сумме заказа от 1000 до 5000 ₽
        /// - Скидка 10% при сумме заказа от 5000 до 20000 ₽
        /// - Скидка 15% при сумме заказа от 20000 ₽
        /// </remarks>
        public decimal CalculateDiscount(decimal totalAmount)
        {
            if (totalAmount >= 20000)
                return totalAmount * 0.15m;
            else if (totalAmount >= 5000)
                return totalAmount * 0.10m;
            else if (totalAmount >= 1000)
                return totalAmount * 0.05m;
            else
                return 0;
        }

        /// <summary>
        /// Рассчитывает итоговую стоимость заказа с учётом скидки
        /// </summary>
        /// <param name="totalAmount">Общая стоимость</param>
        /// <returns>Итоговая стоимость</returns>
        public decimal CalculateFinalAmount(decimal totalAmount)
        {
            var discount = CalculateDiscount(totalAmount);
            return totalAmount - discount;
        }

        /// <summary>
        /// Рассчитывает все параметры заказа
        /// </summary>
        /// <param name="items">Список товаров</param>
        /// <returns>Кортеж с общей стоимостью, скидкой и итоговой суммой</returns>
        public (decimal TotalAmount, decimal Discount, decimal FinalAmount) CalculateOrderTotals(List<OrderItem> items)
        {
            var totalAmount = CalculateTotalAmount(items);
            var discount = CalculateDiscount(totalAmount);
            var finalAmount = totalAmount - discount;

            return (totalAmount, discount, finalAmount);
        }
    }
}