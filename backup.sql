-- =====================================================
-- БАЗА ДАННЫХ: Marketplace
-- =====================================================

-- Создание базы данных (если нужно)
-- CREATE DATABASE MarketplaceDB;
-- GO

-- USE MarketplaceDB;
-- GO

-- =====================================================
-- ТАБЛИЦА: Users (Пользователи)
-- =====================================================
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NULL,
    RegistrationDate DATETIME DEFAULT GETDATE()
);

-- =====================================================
-- ТАБЛИЦА: Sellers (Продавцы)
-- =====================================================
CREATE TABLE Sellers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
    StoreName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Rating DECIMAL(3,2) DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- =====================================================
-- ТАБЛИЦА: Categories (Категории товаров)
-- =====================================================
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    ParentCategoryId INT NULL FOREIGN KEY REFERENCES Categories(Id)
);

-- =====================================================
-- ТАБЛИЦА: Products (Товары)
-- =====================================================
CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(Id),
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    StockQuantity INT NOT NULL CHECK (StockQuantity >= 0),
    SellerId INT NOT NULL FOREIGN KEY REFERENCES Sellers(Id),
    Rating DECIMAL(3,2) DEFAULT 0,
    ImageUrl NVARCHAR(500) NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- =====================================================
-- ТАБЛИЦА: PromoCodes (Промокоды)
-- =====================================================
CREATE TABLE PromoCodes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL UNIQUE,
    DiscountType NVARCHAR(20) NOT NULL CHECK (DiscountType IN ('Percentage', 'Fixed')),
    DiscountValue DECIMAL(10,2) NOT NULL CHECK (DiscountValue > 0),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    MinOrderAmount DECIMAL(10,2) DEFAULT 0,
    IsActive BIT DEFAULT 1,
    UsageLimit INT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- =====================================================
-- ТАБЛИЦА: Orders (Заказы)
-- =====================================================
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    OrderDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'pending',
    TotalAmount DECIMAL(10,2) NOT NULL,
    DiscountAmount DECIMAL(10,2) DEFAULT 0,
    FinalAmount DECIMAL(10,2) NOT NULL,
    ShippingAddress NVARCHAR(500) NULL
);

-- =====================================================
-- ТАБЛИЦА: OrderItems (Состав заказа)
-- =====================================================
CREATE TABLE OrderItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(Id) ON DELETE CASCADE,
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(Id),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL
);

-- =====================================================
-- ТАБЛИЦА: UserPromoCodes (Использование промокодов пользователями)
-- =====================================================
CREATE TABLE UserPromoCodes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PromoCodeId INT NOT NULL FOREIGN KEY REFERENCES PromoCodes(Id),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(Id),
    UsedAt DATETIME DEFAULT GETDATE(),
    DiscountApplied DECIMAL(10,2) NOT NULL,
    CONSTRAINT UQ_UserPromoCode UNIQUE (PromoCodeId, UserId)
);

-- =====================================================
-- ТАБЛИЦА: Returns (Заявки на возврат)
-- =====================================================
CREATE TABLE Returns (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(Id),
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(Id),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    Reason NVARCHAR(500) NOT NULL,
    RequestDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Создана',
    ReturnAmount DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    ResolutionDate DATETIME NULL,
    ResolutionComment NVARCHAR(500) NULL
);

-- =====================================================
-- ТАБЛИЦА: Reviews (Отзывы на товары)
-- =====================================================
CREATE TABLE Reviews (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(Id) ON DELETE CASCADE,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(1000) NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- =====================================================
-- ИНДЕКСЫ ДЛЯ ПРОИЗВОДИТЕЛЬНОСТИ
-- =====================================================
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_SellerId ON Products(SellerId);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
CREATE INDEX IX_OrderItems_ProductId ON OrderItems(ProductId);
CREATE INDEX IX_Returns_OrderId ON Returns(OrderId);
CREATE INDEX IX_Returns_UserId ON Returns(UserId);
CREATE INDEX IX_Returns_Status ON Returns(Status);
CREATE INDEX IX_Reviews_ProductId ON Reviews(ProductId);
CREATE INDEX IX_Reviews_UserId ON Reviews(UserId);
CREATE INDEX IX_PromoCodes_Code ON PromoCodes(Code);
CREATE INDEX IX_PromoCodes_Active ON PromoCodes(IsActive, StartDate, EndDate);
CREATE INDEX IX_UserPromoCodes_UserId ON UserPromoCodes(UserId);

-- =====================================================
-- ТЕСТОВЫЕ ДАННЫЕ
-- =====================================================

-- Категории
INSERT INTO Categories (Name) VALUES 
(N'Электроника'), (N'Одежда'), (N'Книги'), (N'Дом и сад');

-- Пользователи
INSERT INTO Users (Username, Email, PasswordHash, FullName) VALUES
('ivanov', 'ivanov@mail.ru', 'hash123', N'Иван Иванов'),
('petrov', 'petrov@mail.ru', 'hash456', N'Пётр Петров');

-- Продавцы
INSERT INTO Sellers (UserId, StoreName) VALUES
(1, N'TechStore'),
(2, N'FashionShop');

-- Товары
INSERT INTO Products (Name, Description, CategoryId, Price, StockQuantity, SellerId, Rating, ImageUrl) VALUES
(N'Смартфон X100', N'Мощный смартфон с камерой 108MP', 1, 29990, 50, 1, 4.5, 'https://picsum.photos/id/0/100/100'),
(N'Ноутбук Pro', N'16GB RAM, 512GB SSD', 1, 69990, 25, 1, 4.8, 'https://picsum.photos/id/1/100/100'),
(N'Футболка хлопковая', N'Качественная футболка из хлопка', 2, 990, 100, 2, 4.2, 'https://picsum.photos/id/2/100/100'),
(N'Наушники Bluetooth', N'Беспроводные наушники', 1, 4990, 30, 1, 4.6, 'https://picsum.photos/id/3/100/100');

-- Промокоды
INSERT INTO PromoCodes (Code, DiscountType, DiscountValue, StartDate, EndDate, MinOrderAmount, IsActive, UsageLimit) VALUES
('WELCOME10', 'Percentage', 10, DATEADD(day, -30, GETDATE()), DATEADD(day, 30, GETDATE()), 500, 1, 1),
('SAVE500', 'Fixed', 500, DATEADD(day, -30, GETDATE()), DATEADD(day, 30, GETDATE()), 3000, 1, NULL),
('SUMMER20', 'Percentage', 20, DATEADD(day, -30, GETDATE()), DATEADD(day, 30, GETDATE()), 1000, 1, 2),
('TEST100', 'Fixed', 100, DATEADD(day, -30, GETDATE()), DATEADD(day, 30, GETDATE()), 0, 1, NULL);

-- Проверка
SELECT COUNT(*) AS TotalProducts FROM Products;
SELECT COUNT(*) AS TotalPromoCodes FROM PromoCodes;