# Using the View Bag
操作方法为视图提供数据以与视图模型一起显示，但有时需要额外的信息。操作方法可以使用视图包`view bag`来提供带有额外数据的视图。
```cs
public async Task<IActionResult> Index(long id = 1) {
    ViewBag.AveragePrice = await context.Products.AverageAsync(p => p.Price);
    return View(await context.Products.FindAsync(id));
}
```
ViewBag 属性继承自 Controller 基类，返回一个动态对象。这允许操作方法仅通过为它们分配值来创建新属性，如清单中所示。
通过 action 方法分配给 ViewBag 属性的值可通过也称为 ViewBag 的属性提供给视图，如清单 22-6 所示。
```cs
// Index.cshtml
<td>
    @Model?.Price.ToString("c")
    (@(((Model?.Price / ViewBag.AveragePrice)
        * 100).ToString("F2"))% of average price)
</td>
```
## WHEN TO USE THE VIEW BAG
视图包在用于为视图提供少量补充数据而无需为每个操作方法创建新的视图模型类时效果最佳。
视图包的问题是编译器无法检查动态对象属性的使用，就像不使用@model 表达式的视图一样。
很难判断何时应该使用新的视图模型`View Model`类，我的经验是当多个操作`actions`（如Index）使用相同的视图模型属性或当一个操作方法在`View Bag`中添加两个或三个以上属性。

# Using Temp Data
临时数据功能允许控制器保留从一个请求到另一个请求的数据，这在执行重定向时很有用。
除非在将临时数据存储为会话数据时启用会话状态，否则使用 cookie 存储临时数据。与会话数据不同，临时数据值在读取时被标记为删除，并在处理完请求后被删除。
由于临时数据功能构建在会话功能之上，因此只能存储可以序列化为字符串的值。
`CubedController.cs`：
```cs
public IActionResult Cube(double num)
{
    TempData["value"] = num.ToString();
    TempData["result"] = Math.Pow(num, 3).ToString();
    return RedirectToAction(nameof(Index));
}
```
`Cubed.cshtml`:
```html
@if (TempData["result"] != null)
{
    <div class="bg-info text-white m-2 p-2">
        The cube of @TempData["value"] is @TempData["result"]
    </div>
}
```
读取临时数据值不会立即将其删除，这意味着可以在同一视图中重复读取值。只有在请求被处理后，标记的值才会被删除。
TempData 属性返回的对象提供了一个 Peek 方法，允许您获取数据值而不将其标记为删除，还提供一个 Keep 方法，可用于防止先前读取的值被删除。 
Keep 方法不会永远保护一个值。如果再次读取该值，它将再次标记为删除。如果要存储项目，请使用会话数据，以便在处理请求时不会删除它们。

## USING THE TEMP DATA ATTRIBUTE
```cs
using Microsoft.AspNetCore.Mvc;
namespace WebApp.Controllers;

public class CubedController: Controller {
    public IActionResult Index() {
        return View("Cubed");
    }

    public IActionResult Cube(double num) {
        Value = num.ToString();
        Result = Math.Pow(num, 3).ToString();
        return RedirectToAction(nameof(Index));
    }

    [TempData]
    public string? Value { get; set; }

    [TempData]
    public string? Result { get; set; }
}
```
分配给这些属性的值会自动添加到临时数据存储中，并且在视图中访问它们的方式没有区别。  
**我的偏好是使用 TempData 字典来存储值，因为它使操作方法的意图对其他开发人员显而易见。**  
然而，这两种方法都是完全有效的，在它们之间进行选择是一个偏好问题。
