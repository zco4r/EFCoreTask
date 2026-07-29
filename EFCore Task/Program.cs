using EFCore_Task; 

using var context = new AppDbContext();
context.Database.EnsureCreated();

Console.WriteLine("--- Register New User ---");

Console.Write("Enter Username: ");
string username = Console.ReadLine();

Console.Write("Enter Email: ");
string email = Console.ReadLine();

var newUser = new User
{
    Name = username,
     Email = email
};

context.Users.Add(newUser);
context.SaveChanges();

Console.WriteLine("User registered and saved to database successfully!");