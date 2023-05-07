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

### Binding to a Property
使用参数进行模型绑定不符合 Razor Pages 开发风格，因为参数通常会重复页面模型类定义的属性，如清单 28-11 所示。
Listing 28-11. Binding a Complex Type in the FormHandler.cshtml File in the Pages Folder
```cs
public IActionResult OnPost(Product product) {
    TempData["product"] = System.Text.Json.JsonSerializer.Serialize(product);
    return RedirectToPage("FormResults");
}
```
此代码有效，但 OnPost 处理程序方法有自己的 Product 对象版本，镜像 OnGetAsync 处理程序使用的属性。一种更优雅的方法是使用现有属性进行模型绑定，如清单 28-12 所示。
Listing 28-12. Using a Property for Model Binding in the FormHandler.cshtml File in the Pages Folder
```cs
<input class="form-control" asp-for="Product.Name" />
...
<input class="form-control" asp-for="Product.Price" />
...
[BindProperty]
public Product Product { get; set; } = new();
...
public IActionResult OnPost() {
    TempData["product"] = System.Text.Json.JsonSerializer.Serialize(Product);
    return RedirectToPage("FormResults");
}
```
**用 BindProperty 特性装饰一个属性表明它的属性应该服从模型绑定过程，这意味着 OnPost 处理程序方法可以在不声明参数的情况下获取它需要的数据。当使用 BindProperty 属性时，模型绑定器在定位数据值时使用属性名称，因此不需要添加到输入元素的显式名称属性**。默认情况下，BindProperty 不会为 GET 请求绑定数据，但这可以通过将 BindProperty 属性的 SupportsGet 参数设置为 true 来更改。   
BindProperties 特性可应用于需要对其定义的所有公共属性进行模型绑定过程的类，这比将 BindProperty 应用于许多单独的属性更方便。使用 BindNever 属性修饰属性以将它们从模型绑定中排除。

### Binding Nested Complex Types
如果受模型绑定约束的属性是使用复杂类型定义的，则使用属性名称作为前缀重复模型绑定过程。例如，Product 类定义了 Category 属性，其类型是复杂的 Category 类型。清单 28-13 向 HTML 表单添加元素，为模型绑定器提供 Category 类定义的属性值。  
Listing 28-13. Adding Nested Form Elements in the Form.cshtml File in the Views/Form Folder
```html
<div class="form-group">
    <label>Category Name</label>
    @{
        #pragma warning disable CS8602
    }
    <input class="form-control" name="Category.Name" value="@Model.Category.Name" />
    @{
        #pragma warning restore CS8602
    }
</div>
```
name 属性组合了属性名称，以句点分隔。在这种情况下，元素是为分配给视图模型的 Category 属性的对象的 Name 属性，因此 name 属性设置为Category.Name。当应用 asp-for 属性时，输入元素标签助手将自动使用此格式作为 name 属性，如清单 28-14 所示。  
Listing 28-14. Using a Tag Helper in the Form.cshtml File in the Views/Form Folder
```html
<input class="form-control" asp-for="Category.Name" />
```
标签助手是一种更可靠的为嵌套属性创建元素的方法，并且避免了拼写错误产生被模型绑定过程忽略的元素的风险。要查看新元素的效果，请请求 http://localhost:5000/controllers/Form 并单击提交按钮。  
在模型绑定过程中，将创建一个新的 Category 对象并将其分配给 Product 对象的 Category 属性。模型绑定器找到了 Category 对象的 Name 属性的值，如图所示，但是 CategoryId 属性没有值，保留为默认值。

**Specifying Custom Prefixes for Nested Complex Types**
有时您生成的 HTML 与一种类型的对象有关，但您希望将其绑定到另一种对象。这意味着包含视图的前缀将不符合模型绑定器期望的结构，并且您的数据将无法正确处理。清单 28-15 通过更改控制器的 SubmitForm 操作方法定义的参数类型演示了这个问题。  
Listing 28-15. Changing a Parameter in the FormController.cs File in the Controllers Folder
```cs
...
[HttpPost]
public IActionResult SubmitForm(Category category) {
    TempData["category"] = System.Text.Json.JsonSerializer.Serialize(category);
    return RedirectToAction(nameof(Results));
}
...
```
新参数是Category，但模型绑定过程将无法正确挑选出数据值，即使表单视图发送的表单数据将包含类别对象的名称属性的值。相反，模型绑定器将找到 Product 对象的 Name 值并使用它，您可以通过重新启动 ASP.NET Core、请求 http://localhost:5000/controllers/ 表单并提交表单数据来查看，这将产生如图 28-10 所示的第一个响应。  
**这个问题通过将 Bind 属性应用于参数并使用 Prefix 参数为模型绑定器指定前缀来解决**，如清单 28-16 所示。  
Listing 28-16. Setting a Prefix in the FormController.cs File in the Controllers Folder
```cs
...
[HttpPost]
public IActionResult SubmitForm([Bind(Prefix ="Category")] Category category) {
    TempData["category"] = System.Text.Json.JsonSerializer.Serialize(category);
    return RedirectToAction(nameof(Results));
}
...
```
语法很笨拙，但属性确保模型绑定器可以找到操作方法所需的数据。在这种情况下，将前缀设置为 Category 可确保使用正确的数据值来绑定 Category 参数。重新启动 ASP.NET Core，请求 http://localhost:5000/controllers/form，并提交表单，这会产生如图 28-10 所示的第二个响应。  
使用 BindProperty 属性时，使用 Name 参数指定前缀，如清单 28-17 所示。  
Listing 28-17. Specifying a Model Binding Prefix in the FormHandler.cshtml File in the Pages Folder
```html
<div class="form-group">
    <label>Category Name</label>
        @{ #pragma warning disable CS8602 }
    <input class="form-control" asp-for="Product.Category.Name" />
        @{ #pragma warning restore CS8602 }
</div>
...
[BindProperty(Name = "Product.Category")]
public Category Category { get; set; } = new();
...
TempData["category"] = System.Text.Json.JsonSerializer.Serialize(Category);
```
此清单添加了一个输入元素，该元素使用 asp-for 属性来选择 Product.Category 属性。页面处理程序类定义了一个 Category 属性，该属性使用 BindProperty 属性进行修饰并使用 Name 参数进行配置。要查看模型绑定过程的结果，请重新启动 ASP.NET Core，使用浏览器请求 http://localhost:5000/pages/form，然后单击提交按钮。模型绑定会为两个装饰属性找到值，这会产生如图 28-11 所示的响应。

### Selectively Binding Properties
一些模型类定义了敏感的属性，用户不应为其指定值。例如，用户可以更改产品对象的类别，但不能更改价格。您可能会想简单地创建省略敏感属性 HTML 元素的视图，但这不会阻止恶意用户制作包含值的 HTTP 请求，这被称为**过度绑定攻击**。**为了防止模型绑定器使用敏感属性的值，可以指定应该绑定的属性列表**，如清单 28-18 所示。
Listing 28-18. Selectively Binding Properties in the FormController.cs File in the Controllers Folder
```cs
[HttpPost]
public IActionResult SubmitForm([Bind("Name", "Category")] Product product) {
TempData["name"] = product.Name;
TempData["price"] = product.Price.ToString();
TempData["category name"] = product.Category?.Name;
return RedirectToAction(nameof(Results));
}
```
我已返回到操作方法参数的 Product 类型，该参数已使用 Bind 属性进行修饰以指定应包含在模型绑定过程中的属性的名称。此示例告诉模型绑定功能查找 Name 和 Category 属性的值，这从过程中排除了任何其他属性。重新启动 ASP.NET Core，导航到 http://localhost:5000/controllers/Form，然后提交表单。即使浏览器将 Price 属性的值作为 HTTP POST 请求的一部分发送，它也会被模型绑定器忽略，如图 28-12 所示。

**Selectively Binding in the Model Class**
如果你正在使用 Razor Pages 或者你想在整个应用程序中使用相同的属性集来进行模型绑定，你可以将 BindNever 属性直接应用于模型类，如清单 28-19 所示。   
Listing 28-19. Decorating a Property in the Product.cs File in the Models Folder 
```cs
using Microsoft.AspNetCore.Mvc.ModelBinding;
...
[Column(TypeName = "decimal(8, 2)")]
[BindNever]
public decimal Price { get; set; }
...
```
BindNever 属性从模型绑定器中排除一个属性，这与从上一节中使用的列表中省略它具有相同的效果。要查看效果，请重新启动 ASP.NET Core 以使对 Product 类的更改生效，请求 http://localhost:5000/pages/form 并提交表单。与前面的示例一样，模型绑定器忽略 Price 属性的值，如图 28-13 所示。  
还有一个 BindRequired 属性，它告诉模型绑定过程请求必须包含属性值。如果请求没有所需的值，则会产生模型验证错误，如第 29 章所述。

## Binding to Arrays and Collections
### Binding to Arrays
默认模型绑定器的一个优雅特性是它如何支持数组。要查看此功能的工作原理，请将名为 Bindings.cshtml 的 Razor 页面添加到 Pages 文件夹。
数组的模型绑定需要将提供数组值的所有元素的名称属性设置为相同的值。此页面显示三个输入元素，所有这些元素的名称属性值为数据。为了让模型联编程序找到数组值，我用 BindProperty 属性修饰了页面模型的 Data 属性，并使用了 Name 参数。提交 HTML 表单时，将创建一个新数组并使用所有三个输入元素的值填充这些值，这些值将显示给用户。要查看绑定过程，请重新启动 ASP.NET Core，请求 http://localhost:5000/pages/bindings，编辑表单字段，然后单击“提交”按钮。 Data 数组的内容使用@foreach 表达式显示在列表中。

**Specifying Index Positions for Array Values**
默认情况下，数组按照从浏览器接收表单值的顺序填充，这通常是定义 HTML 元素的顺序。如果需要覆盖默认值，可以使用 name 属性指定值在数组中的位置，如清单 28-21 所示。
Listing 28-21. Specifying Array Position in the Bindings.cshtml File in the Pages Folder
```html
<input class="form-control" name="Data[1]" value="Item 1" />
...
<input class="form-control" name="Data[0]" value="Item 2" />
...
<input class="form-control" name="Data[2]" value="Item 3" /
...
```

### Binding to Simple Collections
模型绑定过程可以创建集合和数组。对于列表和集合等序列集合，仅更改模型绑定器使用的属性或参数的类型，如清单 28-22 所示。
Listing 28-22. Binding to a List in the Bindings.cshtml File in the Pages Folder
```cs
public SortedSet<string> Data { get; set; } = new SortedSet<string>();
```
我将 Data 属性的类型更改为 SortedSet<string>。模型绑定过程将使用输入元素的值填充集合，这些值将按字母顺序排序。我在输入元素名称属性上保留了索引符号，但它们没有任何效果，因为集合类将按字母顺序对其值进行排序。要查看效果，请重启 ASP.NET Core ，使用浏览器请求 http://localhost:5000/pages/bindings ，编辑文本字段，然后单击提交按钮。模型绑定过程将使用表单值填充排序集，这些值将按顺序显示，如图 28-16 所示。

### Binding to Dictionaries
对于 name 属性使用索引表示法表示的元素，模型绑定器在绑定到 Dictionary 时将使用索引作为键，从而允许将一系列元素转换为键值对，如示例 28-23 所示。  
Listing 28-23. Binding to a Dictionary in the Bindings.cshtml File in the Pages Folder
```html
<input class="form-control" name="Data[first]" value="Item 1" />
<input class="form-control" name="Data[second]" value="Item 2" />
<input class="form-control" name="Data[third]" value="Item 3" />
...
<table class="table table-sm table-striped">
    <tbody>
        @foreach (string key in Model.Data.Keys) {
        <tr>
        <th>@key</th><td>@Model.Data[key]</td>
        </tr>
        }
    </tbody>
</table>
```
为集合提供值的所有元素必须共享一个公共前缀，在此示例中为 Data，后跟方括号中的键值。此示例的键是字符串 first、second 和 third，它们将用作用户在文本字段中提供的内容的键。要查看绑定过程，请重新启动 ASP.NET Core，请求 http://localhost:5000/pages/bindings，编辑文本字段，然后提交表单。来自表单数据的键和值将显示在一个表中，如图 28-17 所示。

### Binding to Collections of Complex Types
本节中的示例都是简单类型的集合，但同样的过程也可用于复杂类型。为了演示，清单 28-24 修改了 Razor 页面以收集用于绑定到 Product 对象数组的详细信息。   
Listing 28-24. Binding to Complex Types in the Bindings.cshtml File in the Pages Folder
```html
@for (int i = 0; i < 2; i++)
{
    <div class="form-group">
        <label>Name #@i</label>
        <input class="form-control" name="Data[@i].Name" value="Product-@i" />
    </div>
    <div class="form-group">
        <label>Price #@i</label>
        <input class="form-control" name="Data[@i].Price" value="@(100 + i)" />
    </div>
}
...
<tr>
    <th>Name</th>
    <th>Price</th>
</tr>
@foreach (Product p in Model.Data)
{
    <tr>
        <td>@p.Name</td>
        <td>@p.Price</td>
    </tr>
}
```
输入元素的名称属性使用数组表示法，后跟句点，然后是它们表示的复杂类型属性的名称。要为 Name 和 Price 属性定义元素，这需要这样的元素：
```
...
<input class="form-control" name="Data[0].Name" />
...
<input class="form-control" name="Data[0].Price" />
...
```
在绑定过程中，模型绑定器将尝试为目标类型定义的所有公共属性定位值，为表格数据。此示例依赖于 Product 类定义的 Price 属性的模型绑定，该属性被排除在具有 BindNever 属性的绑定过程之外。从属性`property`,中删除属性`attribute`，如清单 28-25 所示。    
Listing 28-25. Removing an Attribute in the Product.cs File in the Models Folder
```cs
//[BindNever]
public decimal Price { get; set; }
```
重启 ASP.NET Core 并使用浏览器请求 http://localhost:5000/pages/bindings。在文本字段中输入名称和价格并提交表单，您将看到根据显示在表中的数据创建的 Product 对象的详细信息，如图 28-18 所示。

## Specifying a Model Binding Source
正如我在本章开头所解释的，默认模型绑定过程在四个位置查找数据：表单数据值、请求主体（仅适用于 Web 服务控制器）、路由数据和请求查询字符串。    
默认搜索顺序并不总是有用，因为您总是希望数据来自请求的特定部分，或者因为您想要使用默认不搜索的数据源。模型绑定功能包括一组用于覆盖默认搜索行为的属性，如表 28-3 中所述。
Table 28-3. The Model Binding Source Attributes
| Name | Description |
|-|-|
| FromForm | 该属性用于选择表单数据作为绑定数据的来源。默认情况下，参数的名称用于定位表单值，但这可以使用 Name 属性进行更改，该属性允许指定不同的名称。|
| FromRoute | 该属性用于选择路由系统作为绑定数据的来源。默认情况下，参数的名称用于定位路由数据值，但这可以使用 Name 属性进行更改，该属性允许指定不同的名称。|
| FromQuery | 该属性用于选择查询字符串作为绑定数据的来源。默认情况下，参数的名称用于定位查询字符串值，但这可以使用 Name 属性更改，这允许指定不同的查询字符串键。|
| FromHeader | 此属性用于选择请求标头作为绑定数据的来源。默认情况下，参数的名称用作标头名称，但这可以使用 Name 属性进行更改，该属性允许指定不同的标头名称。|
| FromBody | 此属性用于指定请求正文应用作绑定数据的来源，当您希望从非表单编码的请求中接收数据时需要此属性，例如在提供 Web 服务的 API 控制器中。|

FromForm、FromRoute 和 FromQuery 属性允许您指定模型绑定数据将从标准位置之一获取，但没有正常的搜索顺序。在本章的前面，我使用了这个 URL：
```shell
http://localhost:5000/controllers/Form/Index/5?id=1
```
这个 URL 包含两个可能的值，可用于 Form 控制器上的 Index 操作方法的 id 参数。路由系统会将 URL 的最后一段分配给一个名为 id 的变量，该变量在控制器的默认 URL 模式中定义，查询字符串也包含一个 id 值。默认搜索模式意味着模型绑定数据将从路由数据中获取，查询字符串将被忽略。    
在清单 28-26 中，我将 FromQuery 属性应用于 Index 操作方法定义的 id 参数，它覆盖了默认的搜索顺序。  
Listing 28-26. Selecting the Query String in the FormController.cs File in the Controllers Folder
```cs
public async Task<IActionResult> Index([FromQuery] long? id) 
```
该属性指定模型绑定过程的源，您可以通过重新启动 ASP 查看它。 NET Core 并使用浏览器请求 http://localhost:5000/controllers/Form/Index/5?id=1。将使用查询字符串代替路由系统匹配的值，生成如图 28-19 所示的响应。如果查询字符串不包含适合模型绑定过程的值，则不会使用其他位置。  
在指定模型绑定源（例如查询字符串）时，您仍然可以绑定复杂类型。对于参数类型中的每个简单属性，模型绑定过程将查找具有相同名称的查询字符串键。

### Selecting a Binding Source for a Property
相同的属性可用于对页面模型或控制器定义的绑定属性进行建模，如清单 28-27 所示。   
Listing 28-27. Selecting the Query String in the Bindings.cshtml File in the Pages Folder
```cs
//[BindProperty(Name = "Data")]
[FromQuery(Name = "Data")]
```
使用 FromQuery 属性意味着查询字符串在创建 Product 数组时用作模型绑定器的值源，您可以通过启动 ASP.NET Core 并请求 http://
localhost:5000/pages/bindings?data[0].name=Skis&data[0].price=500 来查看它，产生如图 28-20 所示的响应。  
在此示例中，我使用了 GET 请求，因为它允许轻松设置查询字符串。尽管在这样一个简单的示例中它是无害的，但在发送修改应用程序状态的 GET 请求时必须小心。如前所述，在 GET 请求中进行更改可能会导致问题。
