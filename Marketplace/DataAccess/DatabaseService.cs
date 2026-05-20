using System.Data.SqlClient;
using System.Collections.ObjectModel;
using Marketplace.BusinessLogic.Models;
using System.Windows;

namespace Marketplace.DataAccess
{
    /// <summary>
    /// Сервис для работы с базой данных SQL Server.
    /// Предоставляет методы для CRUD операций с товарами, заказами, категориями и возвратами.
    /// </summary>
    public class DatabaseService
    {
        private const string ConnectionString = "Server=172.16.1.101,33678;Database=marketplace;User Id=Afanasyev;Password=87DU(Nw;TrustServerCertificate=True;";

        // ==================== ТОВАРЫ ====================

        /// <summary>
        /// Получает все товары из базы данных.
        /// </summary>
        /// <returns>Коллекция ObservableCollection, содержащая все товары.</returns>
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
                            ImageUrl = reader.IsDBNull(8) ? "" : reader.GetString(8)
                        });
                    }
                }
            }

            return products;
        }

        /// <summary>
        /// Добавляет новый товар в базу данных.
        /// </summary>
        /// <param name="product">Объект товара для добавления.</param>
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

        /// <summary>
        /// Обновляет информацию о существующем товаре.
        /// </summary>
        /// <param name="product">Объект товара с обновлёнными данными.</param>
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

        /// <summary>
        /// Обновляет количество товара на складе.
        /// </summary>
        /// <param name="productId">Идентификатор товара.</param>
        /// <param name="newQuantity">Новое количество на складе.</param>
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

        /// <summary>
        /// Получает список всех категорий товаров.
        /// </summary>
        /// <returns>Список категорий.</returns>
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

        /// <summary>
        /// Добавляет новый заказ в базу данных, включая все позиции заказа.
        /// </summary>
        /// <param name="order">Объект заказа для добавления.</param>
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

        /// <summary>
        /// Получает все заказы пользователя с их позициями.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Коллекция заказов пользователя.</returns>
        public ObservableCollection<Order> GetOrdersByUser(int userId)
        {
            var orders = new ObservableCollection<Order>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    @"SELECT o.*, 
                     oi.ProductId, oi.Quantity, oi.UnitPrice,
                     p.Name as ProductName
              FROM Orders o
              LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
              LEFT JOIN Products p ON oi.ProductId = p.Id
              WHERE o.UserId = @UserId
              ORDER BY o.OrderDate DESC, oi.Id",
                    connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    Order currentOrder = null;

                    while (reader.Read())
                    {
                        var orderId = reader.GetInt32(0);

                        if (currentOrder == null || currentOrder.Id != orderId)
                        {
                            currentOrder = new Order
                            {
                                Id = orderId,
                                UserId = reader.GetInt32(1),
                                OrderDate = reader.GetDateTime(2),
                                Status = reader.GetString(3),
                                TotalAmount = reader.GetDecimal(4),
                                DiscountAmount = reader.GetDecimal(5),
                                FinalAmount = reader.GetDecimal(6),
                                ShippingAddress = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                Items = new List<OrderItem>()
                            };
                            orders.Add(currentOrder);
                        }

                        // Добавляем товар, если он есть
                        if (!reader.IsDBNull(8))
                        {
                            currentOrder.Items.Add(new OrderItem
                            {
                                ProductId = reader.GetInt32(8),
                                Quantity = reader.GetInt32(9),
                                UnitPrice = reader.GetDecimal(10),
                                ProductName = reader.GetString(11)
                            });
                        }
                    }
                }
            }

            return orders;
        }

        /// <summary>
        /// Получает позиции заказа по идентификатору заказа.
        /// </summary>
        /// <param name="orderId">Идентификатор заказа.</param>
        /// <returns>Список позиций заказа.</returns>
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

        /// <summary>
        /// Получает название товара по его идентификатору.
        /// </summary>
        /// <param name="productId">Идентификатор товара.</param>
        /// <returns>Название товара или пустая строка, если товар не найден.</returns>
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

        // ==================== ВОЗВРАТЫ ====================

        /// <summary>
        /// Создаёт новую заявку на возврат товара.
        /// </summary>
        /// <param name="returnRequest">Объект заявки на возврат.</param>
        public void CreateReturnRequest(ReturnRequest returnRequest)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    @"INSERT INTO Returns (OrderId, ProductId, UserId, Reason, Quantity, ReturnAmount, Status, RequestDate) 
              VALUES (@OrderId, @ProductId, @UserId, @Reason, @Quantity, @ReturnAmount, @Status, @RequestDate);
              SELECT SCOPE_IDENTITY();",
                    connection);

                command.Parameters.AddWithValue("@OrderId", returnRequest.OrderId);
                command.Parameters.AddWithValue("@ProductId", returnRequest.ProductId);
                command.Parameters.AddWithValue("@UserId", returnRequest.UserId);
                command.Parameters.AddWithValue("@Reason", returnRequest.Reason);
                command.Parameters.AddWithValue("@Quantity", returnRequest.Quantity);
                command.Parameters.AddWithValue("@ReturnAmount", returnRequest.ReturnAmount);
                command.Parameters.AddWithValue("@Status", returnRequest.Status);
                command.Parameters.AddWithValue("@RequestDate", returnRequest.RequestDate);

                var newId = command.ExecuteScalar();
                returnRequest.Id = Convert.ToInt32(newId);
            }
        }

        /// <summary>
        /// Получает все заявки на возврат для указанного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Коллекция заявок на возврат.</returns>
        public ObservableCollection<ReturnRequest> GetReturnRequestsByUser(int userId)
        {
            var returns = new ObservableCollection<ReturnRequest>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    @"SELECT r.*, p.Name as ProductName, o.TotalAmount as OrderTotal 
              FROM Returns r
              JOIN Products p ON r.ProductId = p.Id
              JOIN Orders o ON r.OrderId = o.Id
              WHERE r.UserId = @UserId
              ORDER BY r.RequestDate DESC",
                    connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        returns.Add(new ReturnRequest
                        {
                            Id = reader.GetInt32(0),
                            OrderId = reader.GetInt32(1),
                            ProductId = reader.GetInt32(2),
                            UserId = reader.GetInt32(3),
                            Reason = reader.GetString(4),
                            RequestDate = reader.GetDateTime(5),
                            Status = reader.GetString(6),
                            ReturnAmount = reader.GetDecimal(7),
                            Quantity = reader.GetInt32(8),
                            ResolutionDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                            ResolutionComment = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            ProductName = reader.GetString(11)
                        });
                    }
                }
            }

            return returns;
        }

        /// <summary>
        /// Обновляет статус заявки на возврат.
        /// </summary>
        /// <param name="returnId">Идентификатор заявки.</param>
        /// <param name="status">Новый статус (Создана, Одобрена, Отклонена).</param>
        /// <param name="comment">Комментарий к решению (опционально).</param>
        public void UpdateReturnStatus(int returnId, string status, string comment = null)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    @"UPDATE Returns 
              SET Status = @Status, 
                  ResolutionDate = @ResolutionDate,
                  ResolutionComment = @ResolutionComment
              WHERE Id = @Id",
                    connection);

                command.Parameters.AddWithValue("@Id", returnId);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@ResolutionDate", DateTime.Now);
                command.Parameters.AddWithValue("@ResolutionComment", comment ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Получает позицию заказа по идентификаторам заказа и товара.
        /// </summary>
        /// <param name="orderId">Идентификатор заказа.</param>
        /// <param name="productId">Идентификатор товара.</param>
        /// <returns>Позиция заказа или null, если не найдена.</returns>
        public OrderItem GetOrderItem(int orderId, int productId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    @"SELECT oi.ProductId, p.Name, oi.Quantity, oi.UnitPrice, oi.Subtotal 
              FROM OrderItems oi
              JOIN Products p ON oi.ProductId = p.Id
              WHERE oi.OrderId = @OrderId AND oi.ProductId = @ProductId",
                    connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@ProductId", productId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new OrderItem
                        {
                            ProductId = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Quantity = reader.GetInt32(2),
                            UnitPrice = reader.GetDecimal(3)
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Восстанавливает количество товара на складе после одобрения возврата.
        /// </summary>
        /// <param name="productId">Идентификатор товара.</param>
        /// <param name="quantity">Количество для восстановления.</param>
        public void RestoreStock(int productId, int quantity)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE Products SET StockQuantity = StockQuantity + @Quantity WHERE Id = @ProductId",
                    connection);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.ExecuteNonQuery();
            }
        }

        // ==================== ПРОВЕРКА ПОДКЛЮЧЕНИЯ ====================

        /// <summary>
        /// Проверяет возможность подключения к базе данных.
        /// </summary>
        /// <returns>true - если подключение успешно, false - в случае ошибки.</returns>
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