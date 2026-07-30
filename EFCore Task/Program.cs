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
    Console.WriteLine("Error: You must be logged in to place an order.\n");
}
else
{
    var allProducts = context.Products.ToList();
    if (allProducts.Count > 0)
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

            if (prodId == 0) break;

            Console.Write("Enter Quantity: ");
            int qty = int.Parse(Console.ReadLine());

            newOrder.OrderProducts.Add(new OrderProduct
            {
                ProductId = prodId,
                Quantity = qty
            });

            Console.Write("Add another product? (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y") addingProducts = false;
        }

        if (newOrder.OrderProducts.Count > 0)
        {
            context.Orders.Add(newOrder);
            context.SaveChanges();
            Console.WriteLine("Order placed and saved successfully!\n");
        }
    }
}


Console.WriteLine("=== View My Orders ===");
if (loggedInUserId == 0)
{
    Console.WriteLine("Error: You must be logged in to view your orders.\n");
}
else
{
    var myOrders = context.Orders
        .Where(o => o.UserId == loggedInUserId)
        .Include(o => o.OrderProducts)
        .ThenInclude(op => op.Product)
        .ToList();

    if (myOrders.Count == 0)
    {
        Console.WriteLine("You have no orders yet.\n");
    }
    else
    {
        foreach (var order in myOrders)
        {
            Console.WriteLine($"----------------------------------------");
            Console.WriteLine($"Order ID: {order.Id} | Date: {order.OrderDate}");
            Console.WriteLine("Products in this order:");
            
            foreach (var op in order.OrderProducts)
            {
                string pName = op.Product != null ? op.Product.Name : "Unknown Product";
                decimal pPrice = op.Product != null ? op.Product.Price : 0;
                Console.WriteLine($"  - {pName} | Price: ${pPrice} | Quantity: {op.Quantity}");
            }
        }
        Console.WriteLine("----------------------------------------\n");
    }
}


Console.WriteLine("=== View Order Details ===");
Console.Write("Enter Order ID to view details: ");
if (int.TryParse(Console.ReadLine(), out int targetOrderId))
{
    var orderDetails = context.Orders
        .Include(o => o.OrderProducts)
        .ThenInclude(op => op.Product)
        .Include(o => o.Review) 
        .FirstOrDefault(o => o.Id == targetOrderId);

    if (orderDetails == null)
    {
        Console.WriteLine("Error: Order not found.");
    }
    else
    {
        Console.WriteLine("----------------------------------------");
        Console.WriteLine($"Order ID: {orderDetails.Id}");
        Console.WriteLine($"Order Date: {orderDetails.OrderDate}");
        Console.WriteLine("Products:");

        decimal orderTotal = 0;

        foreach (var op in orderDetails.OrderProducts)
        {
            string productName = op.Product != null ? op.Product.Name : "Unknown";
            decimal productPrice = op.Product != null ? op.Product.Price : 0;
            decimal subTotal = productPrice * op.Quantity;
            orderTotal += subTotal;

            Console.WriteLine($"  * {productName} | Price: ${productPrice} | Qty: {op.Quantity} | Subtotal: ${subTotal}");
        }

        Console.WriteLine($"----------------------------------------");
        Console.WriteLine($"Order Total: ${orderTotal}");

        if (orderDetails.Review != null)
        {
            Console.WriteLine($"Review Rating: {orderDetails.Review.Rating} / 5");
            Console.WriteLine($"Review Comment: {orderDetails.Review.Comment}");
        }
        else
        {
            Console.WriteLine("Review: No review submitted for this order yet.");
        }
        Console.WriteLine("----------------------------------------");
    }
}
else
{
    Console.WriteLine("Invalid Order ID format.");
}

Console.WriteLine("=== Add a Review for an Order ===");
if (loggedInUserId == 0)
{
    Console.WriteLine("Error: You must be logged in to add a review.");
}
else
{
    Console.Write("Enter Order ID to review: ");
    if (int.TryParse(Console.ReadLine(), out int reviewOrderId))
    {
        var targetOrder = context.Orders
            .Include(o => o.Review)
            .FirstOrDefault(o => o.Id == reviewOrderId && o.UserId == loggedInUserId);

        if (targetOrder == null)
        {
            Console.WriteLine("Error: Order not found or does not belong to you.");
        }
        else if (targetOrder.Review != null)
        {
            Console.WriteLine("Error: This order already has a review (1:1 constraint).");
        }
        else
        {
            Console.Write("Enter Rating (1 to 5): ");
            if (int.TryParse(Console.ReadLine(), out int rating) && rating >= 1 && rating <= 5)
            {
                Console.Write("Enter Comment: ");
                string comment = Console.ReadLine();

                var newReview = new Review
                {
                    OrderId = reviewOrderId,
                    Rating = rating,
                    Comment = comment
                };

                context.Reviews.Add(newReview);
                context.SaveChanges();
                Console.WriteLine("Review added successfully!");
            }
            else
            {
                Console.WriteLine("Invalid rating. Must be a number between 1 and 5.");
            }
        }
    }
    else
    {
        Console.WriteLine("Invalid Order ID format.");
    }
}

Console.WriteLine("=== View All Reviews for a Product ===");
var allProductsForReview = context.Products.ToList();

if (allProductsForReview.Count == 0)
{
    Console.WriteLine("No products available.");
}
else
{
    Console.WriteLine("Available Products:");
    foreach (var prod in allProductsForReview)
    {
        Console.WriteLine($"ID: {prod.Id} | Name: {prod.Name}");
    }

    Console.Write("Enter Product ID to view its reviews: ");
    if (int.TryParse(Console.ReadLine(), out int targetProductId))
    {
        var productExists = allProductsForReview.Any(p => p.Id == targetProductId);
        if (!productExists)
        {
            Console.WriteLine("Error: Product not found.");
        }
        else
        {
            var ordersWithProduct = context.Orders
                .Where(o => o.OrderProducts.Any(op => op.ProductId == targetProductId))
                .Include(o => o.Review)
                .Include(o => o.User)
                .ToList();

            var reviewsList = ordersWithProduct
                .Where(o => o.Review != null)
                .ToList();

            if (reviewsList.Count == 0)
            {
                Console.WriteLine("No reviews found for this product.");
            }
            else
            {
                Console.WriteLine($"----------------------------------------");
                Console.WriteLine($"Reviews for Product ID {targetProductId}:");
                foreach (var order in reviewsList)
                {
                    string userName = order.User != null ? order.User.Username : "Unknown User";
                    Console.WriteLine($"  - Order ID: {order.Id} | User: {userName} | Rating: {order.Review.Rating}/5 | Comment: {order.Review.Comment}");
                }
                Console.WriteLine($"----------------------------------------");
            }
        }
    }
    else
    {
        Console.WriteLine("Invalid Product ID format.");
    }
}

Console.WriteLine("=== Logout ===");
if (loggedInUserId == 0)
{
    Console.WriteLine("No user is currently logged in.");
}
else
{
    loggedInUserId = 0;
    Console.WriteLine("Logged out successfully! Current session is cleared.");
}