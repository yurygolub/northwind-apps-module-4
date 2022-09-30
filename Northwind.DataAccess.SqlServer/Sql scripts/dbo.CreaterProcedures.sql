GO
CREATE PROCEDURE [dbo].[InsertProduct]
    @productName     NVARCHAR (40),
    @supplierId      INT,
    @categoryId      INT,
    @quantityPerUnit NVARCHAR (20),
    @unitPrice       MONEY,
    @unitsInStock    SMALLINT,
    @unitsOnOrder    SMALLINT,
    @reorderLevel    SMALLINT,
    @discontinued    BIT
AS
	INSERT INTO dbo.Products (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued)
    OUTPUT Inserted.ProductID
	VALUES (@productName, @supplierId, @categoryId, @quantityPerUnit, @unitPrice, @unitsInStock, @unitsOnOrder, @reorderLevel, @discontinued)

GO
CREATE PROCEDURE [dbo].[DeleteProduct]
	@productID int
AS
	DELETE FROM dbo.[Order Details]
	WHERE [Order Details].ProductID = @productID

	DELETE FROM dbo.Products WHERE ProductID = @productID SELECT @@ROWCOUNT

GO
CREATE PROCEDURE [dbo].[FindProduct]
	@productID int
AS
	SELECT * FROM dbo.Products as p
    WHERE p.ProductID = @productId

GO
CREATE PROCEDURE [dbo].[SelectProducts]
	@offset int,
	@limit int
AS
	SELECT * FROM dbo.Products as p
    ORDER BY p.ProductID
    OFFSET @offset ROWS
    FETCH FIRST @limit ROWS ONLY

GO
CREATE PROCEDURE [dbo].[SelectProductsByName]
	@productName NVARCHAR (40)
AS
	SELECT * FROM dbo.Products as p
	WHERE p.ProductName = @productName
	ORDER BY p.ProductID

GO
CREATE PROCEDURE [dbo].[SelectProductsByCategory]
	@categoryId int
AS
	SELECT * FROM dbo.Products as p
	WHERE p.CategoryID = @categoryId
	ORDER BY p.ProductID

GO
CREATE PROCEDURE [dbo].[UpdateProduct]
	@productId int,
    @productName     NVARCHAR (40),
    @supplierId      INT,
    @categoryId      INT,
    @quantityPerUnit NVARCHAR (20),
    @unitPrice       MONEY,
    @unitsInStock    SMALLINT,
    @unitsOnOrder    SMALLINT,
    @reorderLevel    SMALLINT,
    @discontinued    BIT
AS
	UPDATE dbo.Products
	SET
    ProductName = @productName,
    SupplierID = @supplierId,
    CategoryID = @categoryId,
    QuantityPerUnit = @quantityPerUnit,
    UnitPrice = @unitPrice,
    UnitsInStock = @unitsInStock,
    UnitsOnOrder = @unitsOnOrder,
    ReorderLevel = @reorderLevel,
    Discontinued = @discontinued
	WHERE ProductID = @productId
	SELECT @@ROWCOUNT

GO
CREATE PROCEDURE [dbo].[InsertProductCategory]
	@categoryName nvarchar (15),
	@description ntext,
	@picture image
AS
	INSERT INTO dbo.Categories (CategoryName, Description, Picture)
    OUTPUT Inserted.CategoryID
    VALUES (@categoryName, @description, @picture)

GO
CREATE PROCEDURE [dbo].[DeleteProductCategory]
	@categoryID int
AS
	DELETE FROM dbo.Categories
    WHERE CategoryID = @categoryID
    SELECT @@ROWCOUNT

GO
CREATE PROCEDURE [dbo].[FindProductCategory]
	@categoryId int
AS
	SELECT * FROM dbo.Categories as c
    WHERE c.CategoryID = @categoryId

GO
CREATE PROCEDURE [dbo].[SelectProductCategories]
	@offset int,
	@limit int
AS
	SELECT * FROM dbo.Categories as c
    ORDER BY c.CategoryID
    OFFSET @offset ROWS
    FETCH FIRST @limit ROWS ONLY

GO    
CREATE PROCEDURE [dbo].[SelectProductCategoriesByName]
	@categoryName NVARCHAR (15)
AS
	SELECT * FROM dbo.Categories as c
	WHERE c.CategoryName = @categoryName
	ORDER BY c.CategoryID

GO
CREATE PROCEDURE [dbo].[UpdateProductCategory]
	@categoryId int,
	@categoryName nvarchar (15),
	@description ntext,
	@picture image
AS
	UPDATE dbo.Categories SET CategoryName = @categoryName, Description = @description, Picture = @picture
    WHERE CategoryID = @categoryId
    SELECT @@ROWCOUNT
    
GO
CREATE PROCEDURE [dbo].[InsertEmployee]
    @lastName        NVARCHAR (20),
    @firstName       NVARCHAR (10),
    @title           NVARCHAR (30),
    @titleOfCourtesy NVARCHAR (25),
    @birthDate       DATETIME,
    @hireDate        DATETIME,
    @address         NVARCHAR (60),
    @city            NVARCHAR (15),
    @region          NVARCHAR (15),
    @postalCode      NVARCHAR (10),
    @country         NVARCHAR (15),
    @homePhone       NVARCHAR (24),
    @extension       NVARCHAR (4),
    @photo           IMAGE,
    @notes           NTEXT,
    @reportsTo       INT,
    @photoPath       NVARCHAR (255)
AS
	INSERT INTO dbo.Employees (LastName, FirstName, Title, TitleOfCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, HomePhone, Extension, Photo, Notes, ReportsTo, PhotoPath) OUTPUT Inserted.EmployeeID
    VALUES (@lastName, @firstName, @title, @titleOfCourtesy, @birthDate, @hireDate, @address, @city, @region, @postalCode, @country, @homePhone, @extension, @photo, @notes, @reportsTo, @photoPath)

GO
CREATE PROCEDURE [dbo].[DeleteEmployee]
	@employeeID int
AS
	DELETE FROM dbo.Employees
    WHERE EmployeeID = @employeeID
    SELECT @@ROWCOUNT

GO
CREATE PROCEDURE [dbo].[FindEmployee]
	@employeeID int
AS
	SELECT * FROM dbo.Employees as e
    WHERE e.EmployeeID = @employeeID

GO
CREATE PROCEDURE [dbo].[SelectEmployees]
	@offset int,
	@limit int
AS
	SELECT * FROM dbo.Employees as e
    ORDER BY e.EmployeeID
    OFFSET @offset ROWS
    FETCH FIRST @limit ROWS ONLY
    
GO
CREATE PROCEDURE [dbo].[UpdateEmployee]
	@employeeID int,
    @lastName        NVARCHAR (20),
    @firstName       NVARCHAR (10),
    @title           NVARCHAR (30),
    @titleOfCourtesy NVARCHAR (25),
    @birthDate       DATETIME,
    @hireDate        DATETIME,
    @address         NVARCHAR (60),
    @city            NVARCHAR (15),
    @region          NVARCHAR (15),
    @postalCode      NVARCHAR (10),
    @country         NVARCHAR (15),
    @homePhone       NVARCHAR (24),
    @extension       NVARCHAR (4),
    @photo           IMAGE,
    @notes           NTEXT,
    @reportsTo       INT,
    @photoPath       NVARCHAR (255)
AS
	UPDATE dbo.Employees
    SET
    LastName = @lastName,
    FirstName = @firstName,
    Title = @title,
    TitleOfCourtesy = @titleOfCourtesy,
    BirthDate = @birthDate,
    HireDate = @hireDate,
    Address = @address,
    City = @city,
    Region = @region,
    PostalCode = @postalCode,
    Country = @country,
    HomePhone = @homePhone,
    Extension = @extension,
    Photo = @photo,
    Notes = @notes,
    ReportsTo = @reportsTo,
    PhotoPath = @photoPath
    WHERE EmployeeID = @employeeID
    SELECT @@ROWCOUNT