using EFCore_Task; 

using var context = new AppDbContext();
context.Database.EnsureCreated();

Console.WriteLine("--- Register New User ---");

Console.Write("Enter Username: ");
string username = Console.ReadLine();

Console.Write("Enter Email: ");
string email = Console.ReadLine();

var NewUser = new User
{
    Username = username,
     Email = email
};

context.Users.Add(NewUser);
context.SaveChanges();

Console.WriteLine("User registered and saved to database successfully!");

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
    
    Console.WriteLine($"Login Successful! Welcome, {foundUser.Username}");
}
else
{
    Console.WriteLine("Error: User not found or incorrect email.");
}