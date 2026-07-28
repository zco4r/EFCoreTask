namespace EFCore_Task;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; }
}