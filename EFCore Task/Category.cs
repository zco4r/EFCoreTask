namespace EFCore_Task;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    // العلاقة: تصنيف واحد يحتوي على عدة منتجات (1:N)
    public ICollection<Product> Products { get; set; } = new List<Product>();
}