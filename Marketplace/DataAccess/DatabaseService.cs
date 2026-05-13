using System.Data.SqlClient;
using System.Collections.ObjectModel;
using Marketplace.BusinessLogic.Models;
using System.Windows;

namespace Marketplace.DataAccess
{
    public class DatabaseService
    {
        private const string ConnectionString = "Server=172.16.1.101,33678;Database=marketplace;User Id=Afanasyev;Password=87DU(Nw;TrustServerCertificate=True;";

        // ==================== ТОВАРЫ ====================

        public ObservableCollection<Product> GetAllProducts()
        {
            var products = new ObservableCollection<Product>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Products", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            CategoryId = reader.GetInt32(3),
                            Price = reader.GetDecimal(4),
                            StockQuantity = reader.GetInt32(5),
                            SellerId = reader.GetInt32(6),
                            Rating = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7),
                            ImageUrl = reader.IsDBNull(8) ? "" : reader.GetString(8)  // Добавлено
                        });
                    }
                }
            }

            return products;
        }

        public void AddProduct(Product product)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "INSERT INTO Products (Name, Description, CategoryId, Price, StockQuantity, SellerId, Rating, ImageUrl) " +
                    "VALUES (@Name, @Description, @CategoryId, @Price, @StockQuantity, @SellerId, @Rating, @ImageUrl)",
                    connection);

                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                command.Parameters.AddWithValue("@SellerId", product.SellerId);
                command.Parameters.AddWithValue("@Rating", product.Rating);
                command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl ?? "");

                command.ExecuteNonQuery();
            }
        }

        public void UpdateProduct(Product product)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE Products SET Name=@Name, Description=@Description, CategoryId=@CategoryId, " +
                    "Price=@Price, StockQuantity=@StockQuantity, Rating=@Rating, ImageUrl=@ImageUrl WHERE Id=@Id",
                    connection);

                command.Parameters.AddWithValue("@Id", product.Id);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                command.Parameters.AddWithValue("@Rating", product.Rating);
                command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl ?? "");

                command.ExecuteNonQuery();
            }
        }

        public void UpdateStock(int productId, int newQuantity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Products SET StockQuantity=@StockQuantity WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", productId);
                command.Parameters.AddWithValue("@StockQuantity", newQuantity);
                command.ExecuteNonQuery();
            }
        }

        // ==================== КАТЕГОРИИ ====================

        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Categories", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            ParentCategoryId = reader.IsDBNull(2) ? null : reader.GetInt32(2)
                        });
                    }
                }
            }

            return categories;
        }

        // ==================== ЗАКАЗЫ ====================

        public void AddOrder(Order order)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                // Добавляем заказ
                var orderCommand = new SqlCommand(
                    "INSERT INTO Orders (UserId, OrderDate, Status, TotalAmount, DiscountAmount, FinalAmount, ShippingAddress) " +
                    "VALUES (@UserId, @OrderDate, @Status, @TotalAmount, @DiscountAmount, @FinalAmount, @ShippingAddress); SELECT SCOPE_IDENTITY();",
                    connection);

                orderCommand.Parameters.AddWithValue("@UserId", order.UserId);
                orderCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                orderCommand.Parameters.AddWithValue("@Status", order.Status);
                orderCommand.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                orderCommand.Parameters.AddWithValue("@DiscountAmount", order.DiscountAmount);
                orderCommand.Parameters.AddWithValue("@FinalAmount", order.FinalAmount);
                orderCommand.Parameters.AddWithValue("@ShippingAddress", order.ShippingAddress ?? "");

                var orderId = Convert.ToInt32(orderCommand.ExecuteScalar());
                order.Id = orderId;

                // Добавляем позиции заказа
                foreach (var item in order.Items)
                {
                    var itemCommand = new SqlCommand(
                        "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice, Subtotal) " +
                        "VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @Subtotal)",
                        connection);

                    itemCommand.Parameters.AddWithValue("@OrderId", orderId);
                    itemCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                    itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    itemCommand.Parameters.AddWithValue("@Subtotal", item.Subtotal);
                    itemCommand.ExecuteNonQuery();
                }
            }
        }

        public ObservableCollection<Order> GetOrdersByUser(int userId)
        {
            var orders = new ObservableCollection<Order>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Orders WHERE UserId = @UserId ORDER BY OrderDate DESC", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new Order
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            OrderDate = reader.GetDateTime(2),
                            Status = reader.GetString(3),
                            TotalAmount = reader.GetDecimal(4),
                            DiscountAmount = reader.GetDecimal(5),
                            FinalAmount = reader.GetDecimal(6),
                            ShippingAddress = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            Items = new List<OrderItem>()
                        };

                        orders.Add(order);
                    }
                }
            }

            return orders;
        }

        public List<OrderItem> GetOrderItems(int orderId)
        {
            var items = new List<OrderItem>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM OrderItems WHERE OrderId = @OrderId", connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new OrderItem
                        {
                            ProductId = reader.GetInt32(2),
                            ProductName = GetProductName(reader.GetInt32(2)),
                            Quantity = reader.GetInt32(3),
                            UnitPrice = reader.GetDecimal(4)
                        });
                    }
                }
            }

            return items;
        }

        private string GetProductName(int productId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Name FROM Products WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", productId);
                return command.ExecuteScalar()?.ToString() ?? "";
            }
        }

        // ==================== ПРОВЕРКА ПОДКЛЮЧЕНИЯ ====================

        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка БД",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}