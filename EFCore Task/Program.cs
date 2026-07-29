using EFCore_Task; 

using var context = new AppDbContext();
context.Database.EnsureCreated();

Console.WriteLine("=== Register ===");
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

Console.WriteLine("Category added and saved to database successfully!");