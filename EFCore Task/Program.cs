using EFCore_Task;
using Microsoft.EntityFrameworkCore;

using var context = new AppDbContext();
context.Database.EnsureCreated();

int loggedInUserId = 0;

Console.WriteLine("=== Register New User ===");
Console.Write("Enter Username: ");
string regUsername = Console.ReadLine();

Console.Write("Enter Email: ");
string regEmail = Console.ReadLine();

var newUser = new User
{
    Username = regUsername,
    Email = regEmail
};

context.Users.Add(newUser);
context.SaveChanges();
Console.WriteLine("User registered successfully!\n");


Console.WriteLine("=== Login ===");
Console.Write("Enter your Email to login: ");
string loginEmail = Console.ReadLine();

var foundUser = context.Users.FirstOrDefault(u => u.Email == loginEmail);

if (foundUser != null)
{
    loggedInUserId = foundUser.Id; 
    Console.WriteLine($"Login Successful! Welcome, {foundUser.Username}\n");
}
else
{
    Console.WriteLine("Error: User not found or incorrect email.\n");
}


Console.WriteLine("=== Add New Category ===");
Console.Write("Enter Category Name: ");
string categoryName = Console.ReadLine();

var newCategory = new Category
{
    Name = categoryName
};

context.Categories.Add(newCategory);
context.SaveChanges();
Console.WriteLine("Category added successfully!\n");


Console.WriteLine("=== Add New Product ===");
var categories = context.Categories.ToList();

if (categories.Count > 0)
{
    Console.WriteLine("Available Categories:");
    foreach (var cat in categories)
    {
        Console.WriteLine($"ID: {cat.Id} | Name: {cat.Name}");
    }

    Console.Write("Enter Product Name: ");
    string productName = Console.ReadLine();

    Console.Write("Enter Product Price: ");
    decimal productPrice = decimal.Parse(Console.ReadLine());

    Console.Write("Enter Category ID for this product: ");
    int selectedCategoryId = int.Parse(Console.ReadLine());

    var newProduct = new Product
    {
        Name = productName,
        Price = productPrice,
        CategoryId = selectedCategoryId
    };

    context.Products.Add(newProduct);
    context.SaveChanges();
    Console.WriteLine("Product added successfully!\n");
}


Console.WriteLine("=== View All Products ===");
var productsList = context.Products.Include(p => p.Category).ToList();

if (productsList.Count == 0)
{
    Console.WriteLine("No products found.\n");
}
else
{
    Console.WriteLine("----------------------------------------");
    foreach (var p in productsList)
    {
        string catName = p.Category != null ? p.Category.Name : "No Category";
        Console.WriteLine($"ID: {p.Id} | Name: {p.Name} | Price: ${p.Price} | Category: {catName}");
    }
    Console.WriteLine("----------------------------------------\n");
}


Console.WriteLine("=== Place an Order ===");

if (loggedInUserId == 0)
{
    Console.WriteLine("Error: You must be logged in to place an order.");
}
else
{
    var allProducts = context.Products.ToList();
    if (allProducts.Count == 0)
    {
        Console.WriteLine("No products available to order.");
    }
    else
    {
        var newOrder = new Order
        {
            UserId = loggedInUserId,
            OrderDate = DateTime.Now,
            OrderProducts = new List<OrderProduct>()
        };

        bool addingProducts = true;
        while (addingProducts)
        {
            Console.Write("Enter Product ID to add to order (or type 0 to finish): ");
            int prodId = int.Parse(Console.ReadLine());

            if (prodId == 0)
            {
                break;
            }

            var productExists = allProducts.Any(p => p.Id == prodId);
            if (!productExists)
            {
                Console.WriteLine("Invalid Product ID. Try again.");
                continue;
            }

            Console.Write("Enter Quantity: ");
            int qty = int.Parse(Console.ReadLine());

            newOrder.OrderProducts.Add(new OrderProduct
            {
                ProductId = prodId,
                Quantity = qty
            });

            Console.Write("Add another product? (y/n): ");
            string choice = Console.ReadLine();
            if (choice?.ToLower() != "y")
            {
                addingProducts = false;
            }
        }

        if (newOrder.OrderProducts.Count > 0)
        {
            context.Orders.Add(newOrder);
            context.SaveChanges();
            Console.WriteLine("Order placed and saved successfully!");
        }
        else
        {
            Console.WriteLine("Order cancelled because no products were selected.");
        }
    }
}