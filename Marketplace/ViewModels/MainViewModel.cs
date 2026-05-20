using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Marketplace.BusinessLogic.Models;
using Marketplace.BusinessLogic.Services;
using Marketplace.DataAccess;
using Marketplace.Views;

namespace Marketplace.ViewModels
{
    /// <summary>
    /// Главная ViewModel приложения.
    /// Управляет навигацией, командами и состоянием интерфейса.
    /// Реализует паттерн MVVM и интерфейс INotifyPropertyChanged.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // ==================== ПРИВАТНЫЕ ПОЛЯ ====================

        private object? _currentContent;
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<Order> _orders = new();
        private ObservableCollection<OrderItem> _cart = new();
        private ObservableCollection<Product> _filteredProducts = new();
        private ObservableCollection<ReturnRequest> _returnRequests = new();
        private string _searchQuery = "";
        private Category? _selectedCategory;
        private List<Category> _categories = new();
        private decimal _totalAmount;
        private decimal _discountAmount;
        private decimal _finalAmount;

        // Поля для промокодов
        private string _promoCode = "";
        private string _promoCodeMessage = "";
        private bool _promoCodeSuccess;
        private decimal _promoDiscountAmount;
        private PromoCode? _appliedPromoCode;

        // Сервисы
        private readonly PriceCalculator _priceCalculator = new();
        private readonly StockValidator _stockValidator = new();
        private readonly ReceiptGenerator _receiptGenerator = new();
        private readonly DatabaseService _databaseService = new();
        private readonly PromoCodeService _promoCodeService = new();

        // ==================== КОНСТРУКТОР ====================

        /// <summary>
        /// Инициализирует новый экземпляр MainViewModel.
        /// Загружает данные из базы данных, инициализирует команды и отображает товары.
        /// </summary>
        public MainViewModel()
        {
            LoadDataFromDatabase();
            InitializeCommands();
            ShowProducts();
        }

        // ==================== МЕТОДЫ ЗАГРУЗКИ ДАННЫХ ====================

        /// <summary>
        /// Загружает данные из базы данных: категории, товары, заказы, возвраты.
        /// </summary>
        private void LoadDataFromDatabase()
        {
            if (!_databaseService.TestConnection())
                return;

            _categories = _databaseService.GetAllCategories();
            _products = _databaseService.GetAllProducts();
            _orders = _databaseService.GetOrdersByUser(1);
            _returnRequests = _databaseService.GetReturnRequestsByUser(1);

            foreach (var product in _products)
            {
                var category = _categories.FirstOrDefault(c => c.Id == product.CategoryId);
                if (category != null)
                    product.CategoryName = category.Name;
            }

            _cart = new ObservableCollection<OrderItem>();
        }

        // ==================== СВОЙСТВА ====================

        /// <summary>
        /// Коллекция заявок на возврат.
        /// </summary>
        public ObservableCollection<ReturnRequest> ReturnRequests
        {
            get => _returnRequests;
            set { _returnRequests = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Отфильтрованный список товаров для отображения.
        /// </summary>
        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Текущее содержимое области контента (View).
        /// </summary>
        public object? CurrentContent
        {
            get => _currentContent;
            set { _currentContent = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Код промокода, введённый пользователем.
        /// </summary>
        public string PromoCode
        {
            get => _promoCode;
            set { _promoCode = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Сообщение о результате применения промокода.
        /// </summary>
        public string PromoCodeMessage
        {
            get => _promoCodeMessage;
            set { _promoCodeMessage = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Успешно ли применён промокод.
        /// </summary>
        public bool PromoCodeSuccess
        {
            get => _promoCodeSuccess;
            set { _promoCodeSuccess = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Сумма скидки по промокоду.
        /// </summary>
        public decimal PromoDiscountAmount
        {
            get => _promoDiscountAmount;
            set { _promoDiscountAmount = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Полный список товаров.
        /// </summary>
        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Список заказов пользователя.
        /// </summary>
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set { _orders = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Корзина пользователя.
        /// </summary>
        public ObservableCollection<OrderItem> Cart
        {
            get => _cart;
            set { _cart = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Строка поиска для фильтрации товаров.
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Выбранная категория для фильтрации.
        /// </summary>
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Список всех категорий товаров.
        /// </summary>
        public List<Category> Categories => _categories;

        /// <summary>
        /// Общая стоимость заказа без скидки.
        /// </summary>
        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Сумма скидки (от суммы заказа).
        /// </summary>
        public decimal DiscountAmount
        {
            get => _discountAmount;
            set { _discountAmount = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Итоговая стоимость с учётом всех скидок.
        /// </summary>
        public decimal FinalAmount
        {
            get => _finalAmount;
            set { _finalAmount = value; OnPropertyChanged(); }
        }

        // ==================== КОМАНДЫ ====================

        /// <summary>
        /// Команда применения промокода.
        /// </summary>
        public ICommand ApplyPromoCodeCommand { get; private set; }

        /// <summary>
        /// Команда отображения списка товаров.
        /// </summary>
        public ICommand ShowProductsCommand { get; private set; }

        /// <summary>
        /// Команда отображения корзины.
        /// </summary>
        public ICommand ShowCartCommand { get; private set; }

        /// <summary>
        /// Команда отображения истории заказов.
        /// </summary>
        public ICommand ShowOrdersCommand { get; private set; }

        /// <summary>
        /// Команда отображения формы добавления товара.
        /// </summary>
        public ICommand ShowAddProductCommand { get; private set; }

        /// <summary>
        /// Команда отображения истории возвратов.
        /// </summary>
        public ICommand ShowReturnsCommand { get; private set; }

        /// <summary>
        /// Команда добавления товара в корзину.
        /// </summary>
        public ICommand AddToCartCommand { get; private set; }

        /// <summary>
        /// Команда удаления товара из корзины.
        /// </summary>
        public ICommand RemoveFromCartCommand { get; private set; }

        /// <summary>
        /// Команда увеличения количества товара в корзине.
        /// </summary>
        public ICommand IncreaseQuantityCommand { get; private set; }

        /// <summary>
        /// Команда уменьшения количества товара в корзине.
        /// </summary>
        public ICommand DecreaseQuantityCommand { get; private set; }

        /// <summary>
        /// Команда оформления заказа.
        /// </summary>
        public ICommand PlaceOrderCommand { get; private set; }

        /// <summary>
        /// Команда поиска товаров.
        /// </summary>
        public ICommand SearchCommand { get; private set; }

        /// <summary>
        /// Команда фильтрации товаров по категории.
        /// </summary>
        public ICommand FilterByCategoryCommand { get; private set; }

        /// <summary>
        /// Команда оформления возврата товара.
        /// </summary>
        public ICommand RequestReturnCommand { get; private set; }

        // ==================== МЕТОДЫ ====================

        /// <summary>
        /// Инициализирует все команды приложения.
        /// </summary>
        private void InitializeCommands()
        {
            ShowProductsCommand = new RelayCommand(_ => ShowProducts());
            ShowCartCommand = new RelayCommand(_ => ShowCart());
            ShowOrdersCommand = new RelayCommand(_ => ShowOrders());
            ShowAddProductCommand = new RelayCommand(_ => ShowAddProduct());
            ShowReturnsCommand = new RelayCommand(_ => ShowReturns());
            AddToCartCommand = new RelayCommand(AddToCart);
            RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity);
            PlaceOrderCommand = new RelayCommand(PlaceOrder, _ => Cart.Count > 0);
            SearchCommand = new RelayCommand(_ => ApplyFilter());
            FilterByCategoryCommand = new RelayCommand(_ => ApplyFilter());
            RequestReturnCommand = new RelayCommand(RequestReturn);
            ApplyPromoCodeCommand = new RelayCommand(_ => ApplyPromoCode(), _ => !string.IsNullOrWhiteSpace(PromoCode));
        }

        /// <summary>
        /// Применяет фильтрацию товаров по поисковому запросу и категории.
        /// </summary>
        private void ApplyFilter()
        {
            var filtered = _products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
                filtered = filtered.Where(p => p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                                p.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

            if (SelectedCategory != null)
                filtered = filtered.Where(p => p.CategoryId == SelectedCategory.Id);

            FilteredProducts = new ObservableCollection<Product>(filtered);
        }

        /// <summary>
        /// Отображает каталог товаров.
        /// </summary>
        public void ShowProducts()
        {
            ApplyFilter();
            var productsView = new ProductsListView();
            productsView.DataContext = this;
            CurrentContent = productsView;
        }

        /// <summary>
        /// Отображает корзину с выбранными товарами.
        /// </summary>
        private void ShowCart()
        {
            // Сбрасываем промокод при открытии корзины
            ResetPromoCode();

            var totals = _priceCalculator.CalculateOrderTotals(Cart.ToList());
            TotalAmount = totals.TotalAmount;
            DiscountAmount = totals.Discount;
            FinalAmount = totals.FinalAmount;
            PromoDiscountAmount = 0;

            CurrentContent = new CartView(Cart, this);
        }

        /// <summary>
        /// Отображает историю заказов пользователя.
        /// </summary>
        public void ShowOrders()
        {
            CurrentContent = new OrderHistoryView(Orders, this);
        }

        /// <summary>
        /// Отображает историю возвратов пользователя.
        /// </summary>
        public void ShowReturns()
        {
            _returnRequests = _databaseService.GetReturnRequestsByUser(1);
            var returnHistoryView = new ReturnHistoryView();
            returnHistoryView.DataContext = this;
            CurrentContent = returnHistoryView;
        }

        /// <summary>
        /// Отображает форму добавления нового товара.
        /// </summary>
        private void ShowAddProduct()
        {
            CurrentContent = new AddEditProductView(null, Categories, this);
        }

        /// <summary>
        /// Добавляет товар в корзину.
        /// </summary>
        /// <param name="parameter">Товар для добавления.</param>
        private void AddToCart(object? parameter)
        {
            if (parameter is Product product)
            {
                var existingItem = Cart.FirstOrDefault(i => i.ProductId == product.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    Cart.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        UnitPrice = product.Price
                    });
                }
            }
        }

        /// <summary>
        /// Удаляет товар из корзины.
        /// </summary>
        /// <param name="parameter">Товар для удаления.</param>
        private void RemoveFromCart(object? parameter)
        {
            if (parameter is OrderItem item)
            {
                Cart.Remove(item);
                ShowCart();
            }
        }

        /// <summary>
        /// Увеличивает количество товара в корзине.
        /// </summary>
        /// <param name="parameter">Товар для увеличения количества.</param>
        private void IncreaseQuantity(object? parameter)
        {
            if (parameter is OrderItem item)
            {
                item.Quantity++;
                ShowCart();
            }
        }

        /// <summary>
        /// Уменьшает количество товара в корзине.
        /// </summary>
        /// <param name="parameter">Товар для уменьшения количества.</param>
        private void DecreaseQuantity(object? parameter)
        {
            if (parameter is OrderItem item && item.Quantity > 1)
            {
                item.Quantity--;
                ShowCart();
            }
            else if (parameter is OrderItem item2 && item2.Quantity == 1)
            {
                Cart.Remove(item2);
                ShowCart();
            }
        }

        /// <summary>
        /// Применяет промокод к текущей корзине.
        /// </summary>
        private void ApplyPromoCode()
        {
            // Получаем промокод из БД
            var promoCode = _databaseService.GetPromoCodeByCode(PromoCode);

            if (promoCode == null)
            {
                PromoCodeMessage = "Промокод не найден";
                PromoCodeSuccess = false;
                PromoDiscountAmount = 0;
                _appliedPromoCode = null;
                UpdateFinalAmount();
                return;
            }

            // Получаем количество использований промокода пользователем
            var usageCount = _databaseService.GetUserPromoCodeUsageCount(promoCode.Id, 1);

            // Валидация промокода
            var validation = _promoCodeService.ValidatePromoCode(promoCode, TotalAmount, 1, usageCount);

            if (!validation.IsValid)
            {
                PromoCodeMessage = validation.Message;
                PromoCodeSuccess = false;
                PromoDiscountAmount = 0;
                _appliedPromoCode = null;
                UpdateFinalAmount();
                return;
            }

            // Рассчитываем скидку по промокоду
            var promoDiscount = _promoCodeService.CalculateDiscount(promoCode, TotalAmount);
            PromoDiscountAmount = promoDiscount;
            _appliedPromoCode = promoCode;

            // Обновляем итоговую сумму
            UpdateFinalAmount();

            // Формируем сообщение
            PromoCodeMessage = _promoCodeService.GenerateMessage(promoCode, promoDiscount);
            PromoCodeSuccess = true;
        }

        /// <summary>
        /// Обновляет итоговую стоимость с учётом всех скидок.
        /// </summary>
        private void UpdateFinalAmount()
        {
            FinalAmount = TotalAmount - DiscountAmount - PromoDiscountAmount;
            if (FinalAmount < 0) FinalAmount = 0;
        }

        /// <summary>
        /// Сбрасывает применённый промокод.
        /// </summary>
        private void ResetPromoCode()
        {
            PromoCode = "";
            PromoCodeMessage = "";
            PromoCodeSuccess = false;
            PromoDiscountAmount = 0;
            _appliedPromoCode = null;
        }

        /// <summary>
        /// Оформляет заказ: проверяет наличие товаров, рассчитывает скидку,
        /// сохраняет заказ в БД, обновляет остатки и формирует чек.
        /// </summary>
        /// <param name="parameter">Не используется.</param>
        private void PlaceOrder(object? parameter)
        {
            if (Cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var productDict = _products.ToDictionary(p => p.Id, p => p);
            var isAvailable = _stockValidator.IsOrderAvailable(Cart.ToList(), productDict);

            if (!isAvailable)
            {
                MessageBox.Show("Некоторые товары отсутствуют на складе", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var totals = _priceCalculator.CalculateOrderTotals(Cart.ToList());
            var order = new Order
            {
                UserId = 1,
                UserName = "Иван Иванов",
                OrderDate = DateTime.Now,
                Items = Cart.ToList(),
                Status = "delivered",
                TotalAmount = totals.TotalAmount,
                DiscountAmount = totals.Discount,
                FinalAmount = totals.FinalAmount - PromoDiscountAmount,
                ShippingAddress = "Москва, ул. Примерная, д.1"
            };

            if (order.FinalAmount < 0) order.FinalAmount = 0;

            _databaseService.AddOrder(order);
            Orders.Add(order);

            // Сохраняем использованный промокод
            if (_appliedPromoCode != null)
            {
                _databaseService.SavePromoCodeUsage(_appliedPromoCode.Id, 1, order.Id, PromoDiscountAmount);
            }

            foreach (var item in Cart)
            {
                var product = _products.First(p => p.Id == item.ProductId);
                product.StockQuantity -= item.Quantity;
                _databaseService.UpdateStock(product.Id, product.StockQuantity);
            }

            var receipt = _receiptGenerator.GenerateReceipt(order);
            MessageBox.Show(receipt, "Чек заказа", MessageBoxButton.OK, MessageBoxImage.Information);

            Cart.Clear();
            LoadDataFromDatabase();
            ShowProducts();
        }

        /// <summary>
        /// Обрабатывает запрос на возврат товара.
        /// </summary>
        /// <param name="parameter">Заказ, из которого выполняется возврат.</param>
        private void RequestReturn(object? parameter)
        {
            if (parameter is Order order)
            {
                if (order.Items.Count == 1)
                {
                    var orderItem = order.Items.First();
                    var validation = new ReturnValidator().CanReturnProduct(order, orderItem, orderItem.Quantity);

                    if (!validation.IsValid)
                    {
                        MessageBox.Show(validation.Message, "Возврат невозможен", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    CurrentContent = new ReturnRequestView(new ReturnRequestViewModel(order, orderItem, this));
                }
                else
                {
                    var selectProductView = new SelectProductForReturnView(order, this);
                    CurrentContent = selectProductView;
                }
            }
        }

        /// <summary>
        /// Сохраняет товар (добавляет новый или обновляет существующий).
        /// </summary>
        /// <param name="product">Товар для сохранения.</param>
        public void SaveProduct(Product product)
        {
            if (product.Id == 0)
            {
                product.SellerId = 1;
                _databaseService.AddProduct(product);
            }
            else
            {
                _databaseService.UpdateProduct(product);
            }

            LoadDataFromDatabase();
            ShowProducts();
        }

        // ==================== INotifyPropertyChanged ====================

        /// <summary>
        /// Событие уведомления об изменении свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged.
        /// </summary>
        /// <param name="name">Имя изменившегося свойства.</param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name ?? ""));
    }

    // ==================== КЛАСС RELAYCOMMAND ====================

    /// <summary>
    /// Реализация интерфейса ICommand для использования в MVVM.
    /// Позволяет связывать действия пользователя с методами ViewModel.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// Инициализирует новый экземпляр RelayCommand.
        /// </summary>
        /// <param name="execute">Метод, выполняемый командой.</param>
        /// <param name="canExecute">Метод, определяющий возможность выполнения команды (опционально).</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Определяет, может ли команда выполняться в текущем состоянии.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        /// <returns>true - если команда может быть выполнена, иначе false.</returns>
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        /// <summary>
        /// Выполняет команду.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        public void Execute(object? parameter) => _execute(parameter);

        /// <summary>
        /// Событие, возникающее при изменении возможности выполнения команды.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}