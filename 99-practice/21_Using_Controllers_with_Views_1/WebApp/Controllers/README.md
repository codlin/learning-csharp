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
