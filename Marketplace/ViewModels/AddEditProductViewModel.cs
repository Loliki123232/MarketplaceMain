using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Marketplace.BusinessLogic.Models;

namespace Marketplace.ViewModels
{
    /// <summary>
    /// ViewModel для формы добавления и редактирования товара.
    /// Обеспечивает привязку данных и команды для работы с формой.
    /// </summary>
    public class AddEditProductViewModel
    {
        private readonly MainViewModel _mainViewModel;

        /// <summary>
        /// Товар, который редактируется или создаётся.
        /// </summary>
        public Product EditingProduct { get; set; }

        /// <summary>
        /// Список доступных категорий товаров.
        /// </summary>
        public List<Category> Categories { get; set; }

        /// <summary>
        /// Заголовок формы (зависит от режима: добавление или редактирование).
        /// </summary>
        public string FormTitle => EditingProduct.Id == 0 ? "➕ Добавление товара" : "✏️ Редактирование товара";

        /// <summary>
        /// Флаг, указывающий, является ли товар новым (Id == 0).
        /// </summary>
        public bool IsNewProduct => EditingProduct.Id == 0;

        /// <summary>
        /// Команда сохранения товара.
        /// </summary>
        public ICommand SaveProductCommand { get; }

        /// <summary>
        /// Команда отмены редактирования.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр ViewModel для добавления/редактирования товара.
        /// </summary>
        /// <param name="product">Редактируемый товар (null для создания нового).</param>
        /// <param name="categories">Список категорий для выбора.</param>
        /// <param name="mainViewModel">Главная ViewModel приложения.</param>
        public AddEditProductViewModel(Product? product, List<Category> categories, MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Categories = categories;

            if (product != null && product.Id != 0)
            {
                // Режим редактирования - копируем данные существующего товара
                EditingProduct = new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    Rating = product.Rating,
                    SellerId = product.SellerId,
                    StoreName = product.StoreName,
                    CategoryName = product.CategoryName,
                    ImageUrl = product.ImageUrl
                };
            }
            else
            {
                // Режим добавления - создаём новый товар со значениями по умолчанию
                EditingProduct = new Product
                {
                    Id = 0,
                    Name = "",
                    Description = "",
                    CategoryId = categories.First().Id,
                    Price = 0,
                    StockQuantity = 0,
                    Rating = 0,
                    SellerId = 1,
                    StoreName = "",
                    CategoryName = categories.First().Name,
                    ImageUrl = ""
                };
            }

            SaveProductCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        /// <summary>
        /// Сохраняет товар (добавляет новый или обновляет существующий).
        /// </summary>
        private void Save()
        {
            _mainViewModel.SaveProduct(EditingProduct);
        }

        /// <summary>
        /// Отменяет редактирование и возвращает пользователя к списку товаров.
        /// </summary>
        private void Cancel()
        {
            _mainViewModel.ShowProducts();
        }
    }
}