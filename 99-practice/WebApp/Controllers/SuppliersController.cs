using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApp.Models;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private DataContext context;

    public SuppliersController(DataContext ctx)
    {
        context = ctx;
    }

    [HttpGet("{id}")]
    public async Task<Supplier?> GetSupplier(long id)
    {
        // return await context.Suppliers.FindAsync(id);
        // 第 18 章中定义的数据模型类包括导航属性，当使用 Include 方法时，
        // Entity Framework Core 可以通过以下数据库中的关系来填充这些导航属性。
        // 下面的代码会造成异常：JsonException: A possible object cycle was detected.
        // 该问题是由 Entity Framework Core 功能引起的，该功能试图最大限度地减少从数据库读取的数据量，
        // 但会导致 ASP.NET Core 应用程序出现问题。它在 Supplier 和 Product 对象的导航属性之间创建了一个循环引用。
        // return await context.Suppliers.Include(s => s.Products).FirstAsync(s => s.SupplierId == id);

        // 通过下面的方法避免循环引用
        Supplier supplier = await context.Suppliers.Include(s => s.Products).FirstAsync(s => s.SupplierId == id);
        if (supplier.Products != null)
        {
            foreach (Product p in supplier.Products)
            {
                p.Supplier = null;
            }
        }
        return supplier;
    }
}