using EFCore_Task;

using (var context = new AppDbContext())
{
    context.Database.EnsureCreated();
    
}

Console.WriteLine("تم إنشاء قاعدة البيانات بنجاح!");

