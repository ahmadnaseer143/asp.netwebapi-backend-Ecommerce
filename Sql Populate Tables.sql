-- Populating Users table
INSERT INTO Users (FirstName, LastName, Email, Address, Mobile, Password, role, CreatedAt, ModifiedAt)
VALUES ('Naseer', 'Ahmad', 'nas@gmail.com', '123 Main St', '1234567890', '12345678', 'employee' '2023-06-16', '2023-06-16'),
('Naseer', 'Ahmad', 'naseer@gmail.com', '123 Main St', '1234567890', '12345678', 'admin', '2023-06-16', '2023-06-16');

-- Populating PaymentMethods table
INSERT INTO PaymentMethods (Type, Provider, Available, Reason)
VALUES ('Credit Card', 'Visa', 'Yes', NULL),
       ('PayPal', 'PayPal', 'Yes', NULL),
       ('Debit Card', 'Mastercard', 'Yes', NULL);

-- Populating Offers table
INSERT INTO Offers (Title, Discount)
VALUES ('Summer Sale', 20),
       ('Clearance', 30),
       ('Holiday Special', 15);

-- Populating ProductCategories table
INSERT INTO ProductCategories (Category, SubCategory)
VALUES ('Electronics', 'Mobiles'),
       ('Electronics', 'Laptops'),
       ('Furniture', 'Chairs'),
       ('Furniture', 'Tables');

-- Populating Products table
INSERT INTO Products (Title, Description, CategoryId, OfferId, Price, Quantity, ImageName)
VALUES ('iPhone 12', 'Latest smartphone from Apple', 1, 1, 999, 10, 'iphone12.jpg'),
       ('Samsung Galaxy S21', 'Flagship Android smartphone', 1, 1, 899, 15, 'galaxys21.jpg'),
       ('Dell XPS 13', 'Powerful and portable laptop', 2, 2, 1299, 5, 'xps13.jpg'),
       ('HP Pavilion', 'Affordable laptop for everyday use', 2, 2, 699, 8, 'pavilion.jpg'),
       ('Dining Chair', 'Comfortable and stylish', 3, 3, 19, 20, 'mens_tshirt.jpg'),
       ('Office Chair', 'Elegant and fashionable', 3, 3, 49, 12, 'womens_dress.jpg');

-- Populating Carts table
INSERT INTO Carts (UserId, Ordered, OrderedOn)
VALUES (1, 'true', '2023-06-16');

-- Populating CartItems table
INSERT INTO CartItems (CartId, ProductId)
VALUES (1, 1),
       (1, 3);

-- Populating Reviews table
INSERT INTO Reviews (UserId, ProductId, Review, CreatedAt)
VALUES (1, 1, 'Great phone!', '2023-06-16'),
       (1, 5, 'Nice Chair', '2023-06-16');
