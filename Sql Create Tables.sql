CREATE DATABASE IF NOT EXISTS ECommerce;
USE ECommerce;
CREATE TABLE Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Address VARCHAR(100) NOT NULL,
    Mobile VARCHAR(15) NOT NULL,
    Password VARCHAR(50) NOT NULL,
    role VARCHAR(255) NOT NULL,
    CreatedAt TEXT NOT NULL,
    ModifiedAt TEXT NOT NULL
);

CREATE TABLE PaymentMethods (
    PaymentMethodId INT AUTO_INCREMENT PRIMARY KEY,
    Type TEXT,
    Provider TEXT,
    Available VARCHAR(50),
    Reason TEXT
);

CREATE TABLE Offers (
    OfferId INT AUTO_INCREMENT PRIMARY KEY,
    Title TEXT NOT NULL,
    Discount INT NOT NULL
);

CREATE TABLE ProductCategories (
    CategoryId INT AUTO_INCREMENT PRIMARY KEY,
    Category VARCHAR(50) NOT NULL,
    SubCategory VARCHAR(50) NOT NULL
);

CREATE TABLE Products (
    ProductId INT AUTO_INCREMENT PRIMARY KEY,
    Title TEXT NOT NULL,
    Description TEXT NOT NULL,
    CategoryId INT NOT NULL,
    OfferId INT NOT NULL,
    Price FLOAT NOT NULL,
    Quantity INT NOT NULL,
    ImageName TEXT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES ProductCategories(CategoryId),
    FOREIGN KEY (OfferId) REFERENCES Offers(OfferId)
);

CREATE TABLE Carts (
    CartId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Ordered ENUM('true','false') NOT NULL,
    OrderedOn TEXT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE CartItems (
    CartItemId INT AUTO_INCREMENT PRIMARY KEY,
    CartId INT NOT NULL,
    ProductId INT NOT NULL,
    FOREIGN KEY (CartId) REFERENCES Carts(CartId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE Reviews (
    ReviewId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    ProductId INT NOT NULL,
    Review TEXT NOT NULL,
    CreatedAt VARCHAR(100) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE Payments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    PaymentMethodId INT NOT NULL,
    TotalAmount INT NOT NULL,
    ShippingCharges INT NOT NULL,
    AmountReduced INT NOT NULL,
    AmountPaid INT NOT NULL,
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethods(PaymentMethodId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE Orders (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CartId INT NOT NULL,
    PaymentId INT NOT NULL,
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY (CartId) REFERENCES Carts(CartId),
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
