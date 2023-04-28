using Microsoft.AspNetCore.JsonPatch;
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

    /// <summary>
    /// 对于简单的数据类型，可以通过使用 PUT 方法替换现有对象来处理编辑操作，这是我在第 19 章中采用的方法。
    /// 即使您只需要更改 Product 类中的单个属性值，例如，它使用 PUT 方法并包含所有其他 Product 属性的值并不太麻烦。
    /// 并不是所有的数据类型都那么容易使用，要么是因为它们定义了太多的属性，要么是因为客户端只收到了选定属性的值。
    /// 解决方案是使用 PATCH 请求，它只发送对 Web 服务的更改，而不是完整的替换对象。 
    /// ASP.NET Core 支持使用 JSON 补丁标准，这允许以统一的方式指定更改。 
    /// JSON 补丁标准允许描述一组复杂的更改，但在本章中，我将只关注更改属性值的能力。 
    /// JSON 补丁文档表示为一组操作。每个操作都有一个 op 属性，它指定操作的类型，还有一个 path 属性，它指定操作将应用到哪里。
    /// 对于示例应用程序——事实上，对于大多数应用程序——只需要替换操作，它用于更改属性的值。
    /// 此 JSON 补丁文档为 City 属性设置了新值。 JSON Patch 文档中未提及的 Supplier 类定义的属性将不会被修改。
    /// [
    ///   { "op": "replace", "path": "City", "value": “Los Angeles”},
    /// ]
    /// patch command
    /// Invoke-RestMethod http://localhost:5000/api/suppliers/1 -Method PATCH -ContentType "application/json" -Body '[{"op":"replace","path":"City","value":"Los Angeles"}]'
    /// </summary>
    /// <param name="id"></param>
    /// <param name="patchDoc"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<Supplier?> PatchSupplier(long id, JsonPatchDocument<Supplier> patchDoc)
    {
        Supplier? s = await context.Suppliers.FindAsync(id);
        if (s != null)
        {
            patchDoc.ApplyTo(s);
            await context.SaveChangesAsync();
        }
        return s;
    }
}
