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
    public class MainViewModel : INotifyPropertyChanged
    {
        private object? _currentContent;
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<Order> _orders = new();
        private ObservableCollection<OrderItem> _cart = new();
        private ObservableCollection<Product> _filteredProducts = new();
        private string _searchQuery = "";
        private Category? _selectedCategory;
        private List<Category> _categories = new();
        private decimal _totalAmount;
        private decimal _discountAmount;
        private decimal _finalAmount;

        private readonly PriceCalculator _priceCalculator = new();
        private readonly StockValidator _stockValidator = new();
        private readonly ReceiptGenerator _receiptGenerator = new();
        private readonly DatabaseService _databaseService = new();

        public MainViewModel()
        {
            LoadDataFromDatabase();
            InitializeCommands();
            ShowProducts();
        }

        private void LoadDataFromDatabase()
        {
            if (!_databaseService.TestConnection())
                return;

            _categories = _databaseService.GetAllCategories();
            _products = _databaseService.GetAllProducts();
            _orders = _databaseService.GetOrdersByUser(1);

            foreach (var product in _products)
            {
                var category = _categories.FirstOrDefault(c => c.Id == product.CategoryId);
                if (category != null)
                    product.CategoryName = category.Name;
            }

            _cart = new ObservableCollection<OrderItem>();
        }

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; OnPropertyChanged(); }
        }

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

        private void InitializeCommands()
        {
            ShowProductsCommand = new RelayCommand(_ => ShowProducts());
            ShowCartCommand = new RelayCommand(_ => ShowCart());
            ShowOrdersCommand = new RelayCommand(_ => ShowOrders());
            ShowAddProductCommand = new RelayCommand(_ => ShowAddProduct());
            AddToCartCommand = new RelayCommand(AddToCart);
            RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity);
            PlaceOrderCommand = new RelayCommand(PlaceOrder, _ => Cart.Count > 0);
            SearchCommand = new RelayCommand(_ => ApplyFilter());
            FilterByCategoryCommand = new RelayCommand(_ => ApplyFilter());
        }

        public void ShowProducts()
        {
            ApplyFilter();
            var productsView = new ProductsListView();
            productsView.DataContext = this;
            CurrentContent = productsView;
        }

        private void ShowCart()
        {
            var totals = _priceCalculator.CalculateOrderTotals(Cart.ToList());
            TotalAmount = totals.TotalAmount;
            DiscountAmount = totals.Discount;
            FinalAmount = totals.FinalAmount;
            CurrentContent = new CartView(Cart, this);
        }

        private void ShowOrders()
        {
            CurrentContent = new OrderHistoryView(Orders, this);
        }

        private void ShowAddProduct()
        {
            CurrentContent = new AddEditProductView(null, Categories, this);
        }

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

        private void RemoveFromCart(object? parameter)
        {
            if (parameter is OrderItem item)
            {
                Cart.Remove(item);
                ShowCart();
            }
        }

        private void IncreaseQuantity(object? parameter)
        {
            if (parameter is OrderItem item)
            {
                item.Quantity++;
                ShowCart();
            }
        }

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
                Status = "pending",
                TotalAmount = totals.TotalAmount,
                DiscountAmount = totals.Discount,
                FinalAmount = totals.FinalAmount,
                ShippingAddress = "Москва, ул. Примерная, д.1"
            };

            _databaseService.AddOrder(order);
            Orders.Add(order);

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

        // Свойства
        public object? CurrentContent
        {
            get => _currentContent;
            set { _currentContent = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set { _orders = value; OnPropertyChanged(); }
        }

        public ObservableCollection<OrderItem> Cart
        {
            get => _cart;
            set { _cart = value; OnPropertyChanged(); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); }
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        public List<Category> Categories => _categories;

        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }

        public decimal DiscountAmount
        {
            get => _discountAmount;
            set { _discountAmount = value; OnPropertyChanged(); }
        }

        public decimal FinalAmount
        {
            get => _finalAmount;
            set { _finalAmount = value; OnPropertyChanged(); }
        }

        // Команды
        public ICommand ShowProductsCommand { get; private set; }
        public ICommand ShowCartCommand { get; private set; }
        public ICommand ShowOrdersCommand { get; private set; }
        public ICommand ShowAddProductCommand { get; private set; }
        public ICommand AddToCartCommand { get; private set; }
        public ICommand RemoveFromCartCommand { get; private set; }
        public ICommand IncreaseQuantityCommand { get; private set; }
        public ICommand DecreaseQuantityCommand { get; private set; }
        public ICommand PlaceOrderCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand FilterByCategoryCommand { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name ?? ""));
    }

    // ==================== КЛАСС RELAYCOMMAND ====================
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}