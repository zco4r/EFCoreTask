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

    // العلاقة: تصنيف واحد يحتوي على عدة منتجات (1:N)
    public ICollection<Product> Products { get; set; } = new List<Product>();
}



class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}
