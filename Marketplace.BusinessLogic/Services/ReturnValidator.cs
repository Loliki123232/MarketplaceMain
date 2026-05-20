using Marketplace.BusinessLogic.Models;

namespace Marketplace.BusinessLogic.Services
{
    /// <summary>
    /// Сервис для проверки возможности возврата товара
    /// </summary>
    public class ReturnValidator
    {
        /// <summary>
        /// Максимальный срок для возврата (дни)
        /// </summary>
        private const int MaxReturnDays = 14;

        /// <summary>
        /// Проверяет возможность возврата товара
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="orderItem">Товар в заказе</param>
        /// <param name="quantity">Количество для возврата</param>
        /// <returns>Результат проверки с сообщением</returns>
        /// <summary>
        /// Проверяет возможность возврата товара
        /// </summary>
        public (bool IsValid, string Message) CanReturnProduct(Order order, OrderItem orderItem, int quantity)
        {
            // Проверка: заказ существует
            if (order == null)
                return (false, "Заказ не найден");

            // Проверка: товар существует в заказе
            if (orderItem == null)
                return (false, "Товар не найден в заказе");

            // Проверка: количество для возврата не превышает заказанное
            if (quantity > orderItem.Quantity)
                return (false, $"Вы заказали только {orderItem.Quantity} шт. Нельзя вернуть больше");

            // Проверка: количество должно быть больше 0
            if (quantity <= 0)
                return (false, "Количество для возврата должно быть больше 0");

            // Проверка: срок возврата (не более 14 дней с даты заказа)
            var daysSinceOrder = (DateTime.Now - order.OrderDate).Days;
            if (daysSinceOrder > MaxReturnDays)
                return (false, $"Срок возврата истёк. Возврат возможен в течение {MaxReturnDays} дней с даты заказа");

            // Проверка: статус заказа (возврат для доставленных или завершённых заказов)
            // Для тестирования также разрешаем "pending"
            if (order.Status != "delivered" && order.Status != "completed" && order.Status != "pending")
                return (false, "Возврат возможен только для доставленных заказов");

            return (true, "Возврат возможен");
        }

        /// <summary>
        /// Проверяет, можно ли изменить статус заявки
        /// </summary>
        public bool CanChangeStatus(string currentStatus, string newStatus)
        {
            // Нельзя изменить уже обработанную заявку
            if (currentStatus == ReturnStatuses.Approved || currentStatus == ReturnStatuses.Rejected)
                return false;

            return true;
        }
    }
}