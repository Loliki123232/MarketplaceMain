using Marketplace.BusinessLogic.Models;

namespace Marketplace.BusinessLogic.Services
{
    /// <summary>
    /// Сервис для расчёта суммы возврата
    /// </summary>
    public class ReturnCalculator
    {
        /// <summary>
        /// Рассчитывает сумму возврата за товар
        /// </summary>
        /// <param name="orderItem">Товар в заказе</param>
        /// <param name="quantity">Количество для возврата</param>
        /// <returns>Сумма возврата</returns>
        public decimal CalculateReturnAmount(OrderItem orderItem, int quantity)
        {
            if (orderItem == null || quantity <= 0)
                return 0;

            // Сумма возврата = цена товара × количество
            return orderItem.UnitPrice * quantity;
        }

        /// <summary>
        /// Рассчитывает сумму возврата с учётом скидки на весь заказ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="orderItem">Товар</param>
        /// <param name="quantity">Количество</param>
        /// <returns>Сумма возврата с учётом пропорциональной скидки</returns>
        public decimal CalculateReturnAmountWithDiscount(Order order, OrderItem orderItem, int quantity)
        {
            if (order == null || orderItem == null || quantity <= 0)
                return 0;

            // Если скидки нет, возвращаем полную стоимость
            if (order.DiscountAmount == 0)
                return orderItem.UnitPrice * quantity;

            // Пропорциональное распределение скидки
            var itemSubtotal = orderItem.UnitPrice * quantity;
            var orderSubtotal = order.TotalAmount;

            if (orderSubtotal == 0)
                return itemSubtotal;

            var discountShare = order.DiscountAmount * (itemSubtotal / orderSubtotal);
            return itemSubtotal - discountShare;
        }
    }
}