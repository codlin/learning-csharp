### Validating Data
当您从客户端接受数据时，您必须假设很多数据都是无效的，并准备好过滤掉应用程序无法使用的值。
为 MVC 框架控制器提供的数据验证功能在第 29 章中有详细描述，但对于这一章，我将只关注一个问题：确保客户端为在数据库中存储数据所需的属性提供值。
模型绑定的第一步是将属性应用于数据模型类的属性，如清单 19-24 所示。
```C#
public class ProductBindingTarget {
    ...
    [Required]
    public string Name { get; set; } = "";
    ...
}
```
### Applying the API Controller Atribute
ApiController 属性可应用于 Web 服务控制器类，以更改模型绑定和验证功能的行为。
使用 FromBody 属性从请求正文中选择数据并显式检查 ModelState.IsValid 属性在已使用 ApiController 属性装饰的控制器中不是必需的。
从主体获取数据和验证数据在 Web 服务中非常普遍，因此在使用属性时会自动应用它们，将控制器操作中的代码重点恢复到处理应用程序功能，如清单所示.
```C#
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase {
    ...
    [HttpPost]
    public async Task<IActionResult>
    SaveProduct(ProductBindingTarget target) {
        Product p = target.ToProduct();
        await context.Products.AddAsync(p);
        await context.SaveChangesAsync();
        return Ok(p);
    }
    ...
}
```