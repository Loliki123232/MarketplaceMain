-- Таблица Categories
CREATE TABLE IF NOT EXISTS Categories (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    ParentCategoryId INT NULL,
    FOREIGN KEY (ParentCategoryId) REFERENCES Categories(Id) ON DELETE SET NULL
);

-- Таблица Users
CREATE TABLE IF NOT EXISTS Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Phone VARCHAR(20),
    RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Таблица Sellers
CREATE TABLE IF NOT EXISTS Sellers (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL UNIQUE,
    StoreName VARCHAR(100) NOT NULL,
    Description TEXT,
    Rating DECIMAL(3,2) DEFAULT 0.00,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Таблица Products
CREATE TABLE IF NOT EXISTS Products (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(200) NOT NULL,
    Description TEXT,
    CategoryId INT,
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    StockQuantity INT NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    SellerId INT NOT NULL,
    Rating DECIMAL(3,2) DEFAULT 0.00,
    ImageUrl VARCHAR(500),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE SET NULL,
    FOREIGN KEY (SellerId) REFERENCES Sellers(Id) ON DELETE CASCADE
);

-- Таблица Orders
CREATE TABLE IF NOT EXISTS Orders (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Status ENUM('pending', 'processing', 'shipped', 'delivered', 'cancelled') DEFAULT 'pending',
    TotalAmount DECIMAL(10,2) NOT NULL CHECK (TotalAmount >= 0),
    DiscountAmount DECIMAL(10,2) DEFAULT 0.00 CHECK (DiscountAmount >= 0),
    FinalAmount DECIMAL(10,2) NOT NULL CHECK (FinalAmount >= 0),
    ShippingAddress TEXT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Таблица Reviews
CREATE TABLE IF NOT EXISTS Reviews (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    ProductId INT NOT NULL,
    UserId INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_review (ProductId, UserId)
);

-- Таблица Returns
CREATE TABLE IF NOT EXISTS Returns (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    UserId INT NOT NULL,
    Reason TEXT NOT NULL,
    RequestDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Status ENUM('pending', 'approved', 'rejected', 'completed') DEFAULT 'pending',
    ReturnAmount DECIMAL(10,2) NOT NULL CHECK (ReturnAmount >= 0),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    ResolutionDate DATETIME,
    ResolutionComment TEXT,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);


CREATE INDEX idx_products_category ON Products(CategoryId);
CREATE INDEX idx_products_seller ON Products(SellerId);
CREATE INDEX idx_products_price ON Products(Price);
CREATE INDEX idx_orders_user ON Orders(UserId);
CREATE INDEX idx_orders_status ON Orders(Status);
CREATE INDEX idx_orders_date ON Orders(OrderDate);
CREATE INDEX idx_reviews_product ON Reviews(ProductId);
CREATE INDEX idx_reviews_user ON Reviews(UserId);
CREATE INDEX idx_returns_order ON Returns(OrderId);
CREATE INDEX idx_returns_user ON Returns(UserId);
CREATE INDEX idx_returns_status ON Returns(Status);
CREATE INDEX idx_users_email ON Users(Email);
CREATE INDEX idx_sellers_user ON Sellers(UserId);