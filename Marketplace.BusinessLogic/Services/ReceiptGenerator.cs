using System.Text;
using Marketplace.BusinessLogic.Models;

namespace Marketplace.BusinessLogic.Services
{
    public class ReceiptGenerator
    {
        private readonly PriceCalculator _priceCalculator;

        public ReceiptGenerator()
        {
            _priceCalculator = new PriceCalculator();
        }

        public string GenerateReceipt(Order order)
        {
            if (order == null)
                return "Ошибка: заказ не найден";

            var sb = new StringBuilder();
            var totals = _priceCalculator.CalculateOrderTotals(order.Items);

            sb.AppendLine("╔════════════════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine($"║                         ЧЕК ЗАКАЗА № {order.Id,-35}║");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine($"║  Дата: {order.OrderDate:dd.MM.yyyy HH:mm}".PadRight(78) + "║");
            sb.AppendLine($"║  Покупатель: {order.UserName}".PadRight(78) + "║");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine("║  Товары:                                                                       ║");

            foreach (var item in order.Items)
            {
                sb.AppendLine($"║    • {item.ProductName,-55} ║");
                sb.AppendLine($"║      {item.Quantity} шт × {item.UnitPrice:F2} ₽ = {item.Subtotal:F2} ₽".PadRight(78) + "║");
            }

            sb.AppendLine("╠════════════════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine($"║  Итого: {totals.TotalAmount,62:F2} ₽ ║");
            sb.AppendLine($"║  Скидка: {totals.Discount,61:F2} ₽ ║");
            sb.AppendLine($"║  К оплате: {totals.FinalAmount,60:F2} ₽ ║");
            sb.AppendLine("╚════════════════════════════════════════════════════════════════════════════════╝");
            sb.AppendLine();
            sb.AppendLine("                     Спасибо за покупку!");
            sb.AppendLine("              Ждём вас снова в нашем маркетплейсе!");

            return sb.ToString();
        }

        public string GenerateShortReceipt(Order order)
        {
            if (order == null)
                return "Ошибка: заказ не найден";

            var totals = _priceCalculator.CalculateOrderTotals(order.Items);

            return $"Заказ №{order.Id} от {order.OrderDate:dd.MM.yyyy}\n" +
                   $"Товаров: {order.Items.Count}\n" +
                   $"Итого: {totals.FinalAmount:F2} ₽";
        }
    }
}