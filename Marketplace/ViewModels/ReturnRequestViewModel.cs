using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Marketplace.BusinessLogic.Models;
using Marketplace.BusinessLogic.Services;
using Marketplace.DataAccess;

namespace Marketplace.ViewModels
{
    public class ReturnRequestViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private readonly DatabaseService _databaseService;
        private readonly ReturnValidator _returnValidator;
        private readonly ReturnCalculator _returnCalculator;

        private Order _order;
        private OrderItem _orderItem;
        private int _quantity;
        private string _reason = "";
        private decimal _returnAmount;

        public ReturnRequestViewModel(Order order, OrderItem orderItem, MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _databaseService = new DatabaseService();
            _returnValidator = new ReturnValidator();
            _returnCalculator = new ReturnCalculator();

            _order = order;
            _orderItem = orderItem;
            _quantity = orderItem.Quantity;
            _returnAmount = _returnCalculator.CalculateReturnAmount(orderItem, orderItem.Quantity);

            SubmitCommand = new RelayCommand(_ => SubmitReturn(), _ => CanSubmit());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        public string OrderInfo => $"Заказ №{_order.Id} от {_order.OrderDate:dd.MM.yyyy}";
        public string ProductInfo => $"{_orderItem.ProductName} — {_orderItem.UnitPrice:N2} ₽";

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value >= 1 && value <= _orderItem.Quantity)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    UpdateReturnAmount();
                }
            }
        }

        public int MaxQuantity => _orderItem.Quantity;

        public decimal ReturnAmount
        {
            get => _returnAmount;
            set { _returnAmount = value; OnPropertyChanged(); }
        }

        public string Reason
        {
            get => _reason;
            set { _reason = value; OnPropertyChanged(); }
        }

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        private void UpdateReturnAmount()
        {
            ReturnAmount = _returnCalculator.CalculateReturnAmount(_orderItem, _quantity);
        }

        private bool CanSubmit()
        {
            return !string.IsNullOrWhiteSpace(Reason) && Quantity > 0;
        }

        private void SubmitReturn()
        {
            // Проверка возможности возврата
            var validation = _returnValidator.CanReturnProduct(_order, _orderItem, Quantity);
            if (!validation.IsValid)
            {
                MessageBox.Show(validation.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создание заявки
            var returnRequest = new ReturnRequest
            {
                OrderId = _order.Id,
                ProductId = _orderItem.ProductId,
                UserId = 1, // Текущий пользователь
                Reason = Reason,
                Quantity = Quantity,
                ReturnAmount = ReturnAmount,
                Status = ReturnStatuses.Created,
                RequestDate = DateTime.Now,
                ProductName = _orderItem.ProductName,
                UnitPrice = _orderItem.UnitPrice
            };

            _databaseService.CreateReturnRequest(returnRequest);

            MessageBox.Show($"Заявка на возврат №{returnRequest.Id} создана!\n\n" +
                            $"Сумма возврата: {ReturnAmount:N2} ₽\n" +
                            $"Статус: {returnRequest.Status}\n\n" +
                            "Ожидайте рассмотрения заявки администратором.",
                            "Заявка создана", MessageBoxButton.OK, MessageBoxImage.Information);

            // Возврат к списку заказов
            _mainViewModel.ShowOrders();
        }

        private void Cancel()
        {
            _mainViewModel.ShowOrders();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name ?? ""));
    }
}