# Using Model Binding
模型绑定是使用 HTTP 请求中的值创建 .NET 对象的过程，以便轻松访问操作方法和 Razor 页面所需的数据。在本章中，我描述了模型绑定系统的工作方式；展示它如何绑定简单类型、复杂类型和集合；并演示如何控制流程以指定请求的哪一部分提供应用程序所需的数据值。表 28-1 将模型绑定置于上下文中。
Table 28-1. Putting Model Binding in Context
| Question | Answer |
|--|--|
| 是什么 | 模型绑定是使用从 HTTP 请求获得的数据值创建操作方法和页面处理程序所需的对象的过程。|
| 为什么有用 |  模型绑定允许控制器或页面处理程序使用 C# 类型声明方法参数或属性，并自动从请求中接收数据，而无需直接检查、解析和处理数据。|
| 如何使用 | 在最简单的形式中，方法声明参数，或者类定义属性，其名称用于从 HTTP 请求中检索数据值。可以通过将属性应用于方法参数或属性来配置用于获取数据的请求部分。 | 
| 有没有坑和限制 | 主要的陷阱是从请求的错误部分获取数据。我在“了解模型绑定”部分解释了请求搜索数据的方式，并且可以使用我在“指定模型绑定源”部分中描述的属性明确指定搜索位置。 |
| 有没有其他选择？ | 可以使用上下文对象在没有模型绑定的情况下获取数据。然而，结果是更复杂的代码难以阅读和维护。 |

## Understanding Model Binding
模型绑定是 HTTP 请求和操作或页面处理程序方法之间的优雅桥梁。大多数 ASP.NET Core 应用程序都在某种程度上依赖于模型绑定。
您可以通过使用浏览器请求 http://localhost:5000/controllers/form/index/5 来查看模型绑定的工作情况。  
这个 URL 包含了我要查看的 Product 对象的 ProductId 属性的值，像这样：   
http://localhost:5000/controllers/form/index/**5**  
这部分 URL 对应于控制器路由模式定义的 id 段变量，并匹配 Form 定义的参数名称控制器的 Index 动作：  
```cs
...
public async Task<IActionResult> Index(long id = 1) {
...
```
在 MVC 框架调用动作方法之前需要 id 参数的值，**找到合适的值是模型绑定系统的责任**。**模型绑定系统依赖于模型绑定器，模型绑定器是负责从请求或应用程序的一部分提供数据值的组件**。默认模型绑定器在这四个地方查找数据值：  
- 表单数据 Form data
- 请求主体 The request body（仅适用于使用 ApiController 装饰的控制器） 
- 路由段变量 Routing segment variables
- 查询字符串 Query strings

按顺序检查每个数据源，直到参数的值是成立。示例应用程序中没有表单数据，因此不会在那里找到任何值，并且表单控制器未使用 ApiController 属性进行修饰，因此不会检查请求主体。下一步是检查路由数据，其中包含一个名为 id 的段变量。这允许模型绑定系统提供一个允许调用 Index 操作方法的值。**搜索在找到合适的数据值后停止**，这意味着不会在查询字符串中搜索数据值。

> 在“指定模型绑定源”部分，我解释了如何使用属性指定模型绑定数据的源。这允许您指定从例如查询字符串中获取数据值，即使在路由数据中也有合适的数据。  

了解查找数据值的顺序很重要，因为一个请求可以包含多个值，例如这个 URL：   
http://localhost:5000/controllers/Form/Index/5?id=1    
路由系统将处理请求并将 URL 模板中的 id 段与值 5 匹配，查询字符串包含一个id 值为 1。由于路由数据在查询字符串之前搜索数据，因此 Index 操作方法将收到值 5，而查询字符串值将被忽略。    
另一方面，如果你请求一个没有 id 段的 URL，那么查询字符串将被检查，这意味着像这样的 URL 也将允许模型绑定系统为 id 参数提供一个值以便它可以调用 Index 方法。    
http://localhost:5000/controllers/Form/Index?id=4

## Binding Simple Data Types
请求数据值必须转换为 C# 值，以便它们可用于调用操作或页面处理程序方法。**简单类型是源自请求中可以从字符串解析的一项数据的值**。这包括数值、布尔值、日期，当然还有字符串值。    
简单类型的数据绑定使得从请求中提取单个数据项变得容易，而无需通过上下文数据来找出它的定义位置。清单 28-5 将参数添加到由 Form 控制器方法定义的 SubmitForm 操作方法，以便模型绑定器将用于提供 `name` 和 `price` 的值。  
Listing 28-5. Adding Method Parameters in the FormController.cs File in the Controllers Folder
```cs
[HttpPost]
public IActionResult SubmitForm(string name, decimal price) {
    TempData["name param"] = name;
    TempData["price param"] = price.ToString();
    return RedirectToAction(nameof(Results));
}
```
当 ASP.NET Core 收到将由 SubmitForm 操作方法处理的请求时，模型绑定系统将用于获取名称和价格值。参数的使用简化了操作方法，并负责将请求数据转换为 C# 数据类型，以便在调用操作方法之前将价格值转换为 C# 小数类型。 （在此示例中，我必须将小数转换回字符串以将其存储为临时数据。我在第 31 章中演示了处理表单数据的更多有用方法。）重新启动 ASP.NET Core 并使用浏览器请求 http:/ /localhost:5000/controllers/ 形式。单击提交按钮，您将看到模型绑定功能从请求中提取的值，如图 28-3 所示。

### Binding Simple Data Types in Razor Pages
Razor Pages 可以使用模型绑定，但必须注意**确保表单元素的`名称属性`的值与处理程序方法`参数的名称`相匹配**，如果使用 asp-for 属性选择一个嵌套属性。  
为确保名称匹配，可以显式定义名称属性，如清单 28-6 所示，这也简化了 HTML 表单，使其与控制器示例匹配。  
Listing 28-6. Using Model Binding in the FormHandler.cshtml File in the Pages Folder
```html
<input class="form-control" asp-for="Product.Name" name="name"/>
...
<input class="form-control" asp-for="Product.Price" name="price" />

public IActionResult OnPost(string name, decimal price) {
    TempData["name param"] = name;
    TempData["price param"] = price.ToString();
    ...
}
```
标签助手会将输入元素的名称属性设置为 Product.Name 和 Product.Price，防止模型绑定器匹配值。显式设置 name 属性会覆盖标签助手并确保模型绑定过程正常工作。  
使用浏览器请求http://localhost:5000/pages/form 并点击提交按钮，您将看到模型绑定器找到的值。

### Understanding Default Binding Values
模型绑定是一个尽力而为的功能，这意味着**模型绑定器将尝试获取方法参数的值，但如果无法找到数据值，仍将调用该方法**。您可以通过删除 Form 控制器的 Index 操作方法中 id 参数的默认值来查看其工作原理，如清单 28-7 所示。  
Listing 28-7. Removing a Default Parameter Value in the FormController.cs File in the Controllers Folder
```cs
...
public async Task<IActionResult> Index(long id)
```
重新启动 ASP.NET Core 并请求 http://localhost:5000/controllers/Form。 URL 不包含模型绑定器可用于 id 参数的值，并且没有查询字符串或表单数据，但该方法仍被调用，产生如图 28-5 所示的错误。  
```
An unhandled exception occurred while processing the request.
InvalidOperationException: Sequence contains no elements.
...
WebApp.Controllers.FormController.Index(long id) in FormController.cs
    return View("Form", await context.Products.Include(p => p.Category)
```
模型绑定系统不报告此异常。相反，它发生在执行 Entity Framework Core 查询时。 MVC 框架必须为 id 参数提供一些值来调用 Index 操作方法，因此它使用默认值并希望最好。对于长参数，默认值为 0，这就是导致异常的原因。 Index 操作方法使用 id 值作为键来查询数据库中的 Product 对象，如下所示：  
```cs
.FirstAsync(p => p.ProductId == id));
```
当没有可用于模型绑定的值时，操作方法尝试使用 id 为零来查询数据库。没有这样的对象，导致 Entity Framework Core 尝试处理结果时出现如图所示的错误。  
必须编写应用程序来处理默认参数值，这可以通过多种方式完成。您可以将回退值添加到控制器（如第 21 章所示）或页面（如第 23 章所示）使用的路由 URL 模式。您可以在操作或页面处理程序方法中定义参数时分配默认值，这是我目前在本书的这一部分所采用的方法。或者您可以简单地编写适应默认值的方法而不会导致错误，如清单 28-8 所示。  
Listing 28-8. Avoiding a Query Error in the FormController.cs File in the Controllers Folder
```cs
.FirstOrDefaultAsync(p => p.ProductId == id));
```
如果数据库中没有匹配的对象，Entity Framework Core FirstOrDefaultAsync 方法将返回 null，并且不会尝试加载相关数据。标签助手处理空值并显示空字段，您可以通过重新启动 ASP.NET Core 并请求 http://localhost:5000/controllers/Form 来查看。   
一些应用程序需要区分缺失值和用户提供的任何值。在这些情况下，可以使用可为 null 的参数类型，如清单 28-9 所示。
Listing 28-9. Using a Nullable Parameter in the FormController.cs File in the Controllers Folder
```cs
...
public async Task<IActionResult> Index(long? id) {
    ViewBag.Categories = new SelectList(context.Categories, "CategoryId", "Name");
    return View("Form", await context.Products.Include(p => p.Category)
    .Include(p => p.Supplier)
    .FirstOrDefaultAsync(p => id == null || p.ProductId == id));
}
...
```
只有当请求不包含合适的值时，id 参数才会为 null，这允许传递给 FirstOrDefaultAsync 方法的表达式在没有值时默认为数据库中的第一个对象，并查询任何其他值。要查看效果，请重启 ASP.NET Core 并请求 http://localhost:5000/controllers/Form 和 http://localhost:5000/controllers/Form/index/0。第一个 URL 不包含 id 值，因此选择了数据库中的第一个对象。第二个 URL 提供的 id 值为零，不对应于数据库中的任何对象。

## Binding Complex Types
模型绑定系统在处理复杂类型时大放异彩，复杂类型是无法从单个字符串值解析的任何类型。模型绑定过程检查复杂类型并对它定义的每个公共属性执行绑定过程。这意味着我可以使用活页夹创建完整的 Product 对象，而不是处理名称和价格等单个值，如清单 28-10 所示。
Listing 28-10. Binding a Complex Type in the FormController.cs File in the Controllers Folder
```cs
[HttpPost]
public IActionResult SubmitForm(Product product) {
    TempData["product"] = System.Text.Json.JsonSerializer.Serialize(product);
    return RedirectToAction(nameof(Results));
}
```
该清单更改了 SubmitForm 操作方法，以便它定义 Product 参数。在调用 action 方法之前，创建一个新的 Product 对象，并将模型绑定过程应用于它的每个公共属性。然后使用 Product 对象作为参数调用 SubmitForm 方法。要查看模型绑定过程，请重新启动 ASP.NET Core，导航至 http://localhost:5000/controllers/Form，然后单击“提交”按钮。模型绑定过程将从请求中提取数据值并产生如图 28-8 所示的结果。模型绑定过程创建的 Product 对象被序列化为 JSON 数据，以便可以将其存储为临时数据，以便于查看请求数据。
复杂类型的数据绑定过程仍然是尽力而为的功能，这意味着将为 Product 类定义的每个公共属性寻找一个值，但缺少值不会阻止调用操作方法。相反，找不到值的属性将保留为属性类型的默认值。该示例为 Name 和 Price 属性提供了值，但 ProductId、CategoryId 和 SupplierId 属性为零，而 Category 和 Supplier 属性为空。