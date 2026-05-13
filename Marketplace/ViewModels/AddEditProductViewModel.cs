using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Marketplace.BusinessLogic.Models;


namespace Marketplace.ViewModels
{
    public class AddEditProductViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public Product EditingProduct { get; set; }
        public List<Category> Categories { get; set; }
        public string FormTitle => EditingProduct.Id == 0 ? "➕ Добавление товара" : "✏️ Редактирование товара";
        public bool IsNewProduct => EditingProduct.Id == 0;

        public ICommand SaveProductCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditProductViewModel(Product? product, List<Category> categories, MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Categories = categories;

            if (product != null && product.Id != 0)
            {
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

        private void Save()
        {
            _mainViewModel.SaveProduct(EditingProduct);
        }

        private void Cancel()
        {
            _mainViewModel.ShowProducts();
        }
    }
}