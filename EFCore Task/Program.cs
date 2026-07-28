namespace EFCore_Task;


public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public Review Review { get; set; }

    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}
