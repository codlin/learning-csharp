# Using the Forms Tag Helpers
在本章中，我描述了用于创建 HTML 表单的内置标签助手。这些标签助手确保表单提交给正确的操作或页面处理程序方法，并且元素准确地表示特定的模型属性。表 27-1 将表单标签助手放在上下文中。
Table 27-1. Putting Form Tag Helpers in Context
| Question | Answer |
|-|-|
| 是什么    | 这些内置的标签助手转换 HTML 表单元素。|
| 为什么有用 | 这些标签助手确保 HTML 表单反映应用程序的路由配置和数据模型。|
| 如何用 | 标签助手使用 asp-* 属性应用于 HTML 元素。|
| 有哪些坑和限制 | 这些标签助手可靠且可预测，不会出现严重问题。|
| 有没有其它选择 | 您不必使用标签助手，如果您愿意，可以在没有它们的情况下定义表单。|

## Understanding the Form Handling Pattern
大多数 HTML 表单都存在于一个定义良好的模式中，如图 27-2 所示。首先，浏览器发送一个 HTTP GET 请求，该请求产生一个包含表单的 HTML 响应，使用户可以向应用程序提供数据。用户单击一个按钮，该按钮使用 HTTP POST 请求提交表单数据，这允许应用程序接收和处理用户的数据。处理完数据后，将发送一个响应，将浏览器重定向到一个确认用户操作的 URL。  
这被称为 Post/Redirect/Get 模式，重定向很重要，因为它意味着用户可以单击浏览器的重新加载按钮而无需发送另一个 POST 请求，这可能会导致无意中重复操作。在接下来的部分中，我将展示如何使用控制器和 Razor Pages 遵循该模式。我从模式的基本实现开始，然后演示使用标签助手的改进，并在第 28 章中演示模型绑定功能。

### Creating a Controller to Handle Forms
处理表单的控制器是通过组合前面章节中描述的功能创建的。使用清单 27-5 中所示的代码将名为 FormController.cs 的类文件添加到 Controllers 文件夹中。
Index 操作方法选择一个名为 Form 的视图，它将向用户呈现一个 HTML 表单。当用户提交表单时，它将被 SubmitForm 操作接收，该操作已经用 HttpPost 属性进行了修饰，因此它只能接收 HTTP POST 请求。此操作方法处理通过 HttpRequest.Form 属性可用的 HTML 表单数据，以便可以使用临时数据功能存储这些数据。临时数据功能可用于将数据从一个请求传递到另一个请求，但只能用于存储简单数据类型。每个表单数据值都显示为一个字符串数组，我将其转换为单个逗号分隔的字符串进行存储。浏览器被重定向到选择默认视图的结果操作方法。  
仅显示名称不以下划线开头的表单数据值。我将在本章后面的“使用防伪功能”部分解释原因。
要为控制器提供视图，请创建 Views/Form 文件夹并向其中添加一个名为 Form.cshtml 的 Razor 视图文件。
此视图包含一个简单的 HTML 表单，该表单配置为使用 POST 请求将其数据提交到 SubmitForm 操作方法。该表单包含一个输入元素，其值是使用 Razor 表达式设置的。  
接下来，将名为 Results.cshtml 的 Razor 视图添加到 Views/Form 文件夹。
此视图将表单数据显示回用户。我将在第 31 章向您展示如何以更有用的方式处理表单数据，但本章的重点是创建表单，看到表单中包含的数据就足以开始了。 
重新启动 ASP.NET Core 并使用浏览器请求 http://localhost:5000/controllers/form 以查看 HTML 表单。在文本字段中输入一个值并单击提交以发送一个 POST 请求，该请求将由 SubmitForm 操作处理。表单数据将存储为临时数据，浏览器将被重定向，产生如图 27-3 所示的响应。

### Creating a Razor Page to Handle Forms
可以使用 Razor Pages 实现相同的模式。需要一个页面来呈现和处理表单数据，第二个页面显示结果。将名为 FormHandler.cshtml 的 Razor 页面添加到 Pages 文件夹。
OnGetAsync 处理程序方法从数据库中检索产品，视图使用该产品为 HTML 表单中的输入元素设置值。该表单配置为发送 HTTP POST 请求，该请求将由 OnPost 处理程序方法处理。表单数据存储为临时数据，浏览器将重定向到名为 FormResults 的表单。要创建浏览器将被重定向到的页面，请将名为 FormResults.cshtml 的 Razor 页面添加到 Pages 文件夹。  
清单 27-8 中的页面模型类装饰有 IgnoreAntiforgeryToken 属性，这在“使用防伪功能”部分中有描述。

## Using Tag Helpers to Improve HTML Forms
### Working with Form Elements
FormTagHelper 类是表单元素的内置标签助手，用于管理 HTML 表单的配置，以便它们以正确的操作或页面处理程序为目标，而无需对 URL 进行硬编码。这个标签助手支持表 27-3 中描述的属性。
Table 27-3. The Built-in Tag Helper Attributes for Form Elements
| Name | Description |
|-|-|
| aspcontroller | 该属性用于为动作属性 URL 指定路由系统的控制器值。如果省略，则将使用呈现视图的控制器。|
| asp-action | 该属性用于为动作属性URL指定动作值到路由系统的动作方法。如果省略，则将使用呈现视图的动作。|
| asp-page | 此属性用于指定 Razor 页面的名称。|
| asp-pagehandler | 此属性用于指定将用于处理请求的处理程序方法的名称。您可以在第 9 章的 SportsStore 应用程序中看到此属性的示例。|
| asp-route-* | 名称以 asp-route- 开头的属性用于为操作属性 URL 指定附加值，以便 asp-route-id 属性用于向路由系统提供 id 段的值。|
| asp-route | 此属性用于指定将用于为 action 属性生成 URL 的路由名称。|
| aspantiforgery | 此属性控制是否将防伪信息添加到视图中，如“使用防伪功能”部分所述。|
| aspfragment | 此属性指定生成的 URL 的片段。 |

**Setting the Form Target**
FormTagHelper 转换表单元素，以便它们以操作方法或 Razor 页面为目标，而无需硬编码 URL。此标签助手支持的属性的工作方式与第 26 章中描述的锚元素相同，并使用属性提供有助于通过 ASP.NET Core 路由系统生成 URL 的值。清单 27-10 修改了 Form 视图中的 form 元素以应用标签助手。
Listing 27-10. Using a Tag Helper in the Form.cshtml File in the Views/Form Folder
```html
<form asp-action="submitform" method="post">
```
asp-action 属性用于指定将接收 HTTP 请求的操作的名称。路由系统用于生成 URL，就像第 26 章中描述的锚元素一样。示例 27-10 中没有使用 asp-controller 属性，这意味着呈现视图的控制器将在 URL 中使用. asp-page 属性还用于选择一个 Razor 页面作为表单的目标，如清单 27-11 所示。
Listing 27-11. Setting the Form Target in the FormHandler.cshtml File in the Pages Folder
```html
<form asp-page="FormHandler" method="post">
```

### Transforming Form Buttons
发送表单的按钮可以在表单元素之外定义。在这些情况下，按钮有一个 form 属性，其值对应于它相关的 form 元素的 id 属性，以及一个指定表单目标 URL 的 formaction 属性。标签助手将通过 asp-action、asp-controller 或 asppage 属性生成 formaction 属性，如清单 27-12 所示。
Listing 27-12. Transforming a Button in the Form.cshtml File in the Views/Form Folder
```html
<form asp-action="submitform" method="post" id="htmlform">
...
<button form="htmlform" asp-action="submitform" class="btn btn-primary mt-2">
...
```
添加到 form 元素的 id 属性的值被按钮用作 form 属性的值，它告诉浏览器在单击按钮时提交哪个表单。表 27-3 中描述的属性用于标识表单的目标，标签助手将在呈现视图时使用路由系统生成 URL。清单 27-13 将相同的技术应用于 Razor 页面。

## Working with input Elements
input 元素是 HTML 表单的支柱，它提供了用户向应用程序提供非结构化数据的主要方式。 InputTagHelper 类用于转换输入元素，以便它们反映它们用于收集的视图模型属性的数据类型和格式，使用表 27-4 中描述的属性。
Table 27-4. The Built-in Tag Helper Attributes for input Elements
| Name | Description |
|-|-|
| asp-for | 此属性用于指定输入元素表示的视图模型属性。|
| aspformat | 此属性用于指定用于输入元素表示的视图模型属性值的格式。|
asp-for 属性设置为视图模型属性的名称，然后用于设置输入元素的名称、ID、类型和值属性。清单 27-14 修改了控制器视图中的输入元素以使用 asp-for 属性。
Listing 27-14. Configuring an Input Element in the Form.cshtml File in the Views/Form Folder
```html
<input class="form-control" asp-for="Name" />
```
这个标签助手使用一个模型表达式，在第 25 章中描述，这就是为什么在指定 aspfor 属性的值时没有使用 @ 字符。如果您重新启动 ASP.NET Core 并检查应用程序在使用浏览器请求 http://localhost:5000/controllers/form 时返回的 HTML，您将看到标签助手已将输入元素转换为如下所示：
```html
...
<div class="form-group">
    <label>Name</label>
    <input class="form-control" type="text" data-val="true"
        data-val-required="The Name field is required." id="Name"
        name="Name" value="Kayak">
</div>
...
```
id 和 name 属性的值**是通过模型表达式获得的**，确保您在创建表单时不会引入拼写错误。其他属性更复杂，在后面的部分或第 29 章中进行了描述，我在第 29 章中解释了 ASP.NET Core 对验证数据的支持。

**SELECTING MODEL PROPERTIES IN RAZOR PAGES**
此属性和本章中描述的其他标签助手的 asp-for 属性可用于 Razor 页面，但转换元素中的 name 和 id 属性的值包括页面模型属性的名称。例如，此元素通过页面模型的 Product 属性选择 Name 属性：
```html
<input class="form-control" asp-for="Product.Name" />
```
转换后的元素将具有以下 id 和 name 属性：
```html
<input class="form-control" type="text" id="Product_Name" name="Product.Name" >
```
如第 28 章所述，当使用模型绑定功能接收表单数据时，这种差异很重要。

### Transforming the input Element type Attribute
input 元素的 type 属性告诉浏览器如何显示该元素以及它应该如何限制用户输入的值。清单 27-14 中的 input 元素被配置为文本类型，这是默认的 input 元素类型并且没有提供任何限制。清单 27-15 向表单添加了另一个输入元素，这将提供一个更有用的演示，说明如何处理 type 属性。
Listing 27-15. Adding an input Element in the Form.cshtml File in the Views/Form Folder
```html
<div class="form-group">
    <label>Id</label>
    <input class="form-control" asp-for="ProductId" />
</div>
```
新元素使用 asp-for 属性来选择视图模型的 ProductId 属性。重新启动 ASP。 NET Core 和浏览器请求 http://localhost:5000/controllers/form 以查看标签助手如何转换元素。
```html
...
<div class="form-group">
    <label>Id</label>
    <input class="form-control" type="number" data-val="true"
        data-val-required="The ProductId field is required."
        id="ProductId" name="ProductId" value="1">
</div>
...
```
type 属性的值由 asp-for 属性指定的视图模型属性的类型决定。 ProductId 属性的类型是 C# long 类型，这导致标签助手将输入元素的类型属性设置为数字，这限制了该元素，使其只能接受数字字符。 data-val 和 data-val-required 属性被添加到输入元素以协助验证，这在第 29 章中进行了描述。表 27-5 描述了如何使用不同的 C# 类型来设置输入元素的类型属性。浏览器如何解释 type 属性是有自由度的。并非所有浏览器都响应 HTML 规范中定义的所有类型值，当它们响应时，它们的实现方式也有所不同。 type 属性对于您期望表单中的数据类型可能是一个有用的提示，但是您应该使用模型验证功能来确保用户提供可用的数据，如第 29 章所述。
Table 27-5. C# Property Types and the Input Type Elements They Generate
| C# Type | input Element type Attribute |
|-|-|
| byte, sbyte, int, uint, short, ushort, long, ulong | number |
| float, double, decimal | text, 具有用于模型验证的附加属性，如第 29 章所述 |
| bool | checkbox |
| string | text |
| DateTime | datetime |

float、double 和 decimal 类型产生类型为文本的输入元素，因为并非所有浏览器都允许可用于表示此类型的合法值的所有字符范围。为了向用户提供反馈，标签助手将属性添加到输入元素，这些属性与第 29 章中描述的验证功能一起使用。  
您可以通过在输入元素上显式定义类型属性来覆盖表 27-5 中显示的默认映射。标签助手不会覆盖您定义的值，它允许您指定类型属性值。  
这种方法的缺点是您必须记住在为给定模型属性生成输入元素的所有视图中设置类型属性。一种更优雅且更可靠的方法是将表 27-6 中描述的属性之一应用于 C# 模型类中的属性。  
如果模型属性不是表 27-5 中的类型之一并且没有用属性修饰，标签助手会将输入元素的类型属性设置为文本。

Table 27-6. The Input Type Elements Attributes
| Attribute | input Element type Attribute |
|-|-|
| [HiddenInput] | hidden |
| [Text] | text  |
| [Phone] | tel  |
| [Url] | url  |
| [EmailAddress] | email  |
| [DataType(DataType.Password)] | password  |
| [DataType(DataType.Time)] | time |
| [DataType(DataType.Date)] | date |

### Formatting input Element Values
当 action 方法为视图提供视图模型对象时，标签助手使用赋予 asp-for 属性的属性值来设置输入元素的 value 属性。 asp-format 属性用于指定数据值的格式。为了演示默认格式，清单 27-16 向 Form 视图添加了一个新的输入元素。
Listing 27-16. Adding an Element in the Form.cshtml File in the Views/Form Folder
重新启动 ASP.NET Core，使用浏览器导航到 http://localhost:5000/controllers/form/index/5，并检查浏览器接收到的 HTML。默认情况下，输入元素的值是使用模型属性的值设置的，如下所示： 
```html
...
<input class="form-control" type="text" data-val="true"
    data-val-number="The field Price must be a number."
    data-val-required="The Price field is required."
    id="Price" name="Price" value="79500.00">
...
```
这种具有两位小数的格式是值在数据库中的存储方式。在第 26 章中，我使用 Column 属性选择了一个 SQL 类型来存储 Price 值，如下所示：
```cs
...
[Column(TypeName = "decimal(8, 2)")]
public decimal Price { get; set; }
...
``` 
这种类型指定了八位的最大精度，其中两位将出现在小数位之后。这允许最大值为 999,999.99，这足以代表大多数在线商店的价格。 
asp-format 属性接受将传递给标准 C# 字符串格式化系统的格式字符串，如清单 27-17 所示。
Listing 27-17. Formatting a Data Value in the Form.cshtml File in the Views/Form Folder
```html
<input class="form-control" asp-for="Price" asp-format="{0:#,###.00}" />
```
属性值是逐字使用的，这意味着您必须包括大括号字符和 0: 引用，以及您需要的格式。刷新浏览器，您将看到输入元素的值已被格式化，如下所示： 
```html
<input ... value="79,500.00">
```
应谨慎使用此功能，因为您必须确保应用程序的其余部分配置为支持您使用的格式并且您创建的格式仅包含输入元素类型的合法字符。

**Applying Formatting via the Model Class**
如果您始终希望对模型属性使用相同的格式，则可以使用 DisplayFormat 属性修饰 C# 类，该属性在 System.ComponentModel.DataAnnotations 命名空间中定义。 DisplayFormat 属性需要两个参数来格式化数据值：DataFormatString 参数指定格式化字符串，将 ApplyFormatInEditMode 设置为 true 指定在将值应用于用于编辑的元素（包括输入元素）时应使用格式化。  
清单 27-18 将该属性应用于 Product 类的 Price 属性，指定了与前面示例不同的格式化字符串。
Listing 27-18. Applying a Formatting Attribute to the Product.cs File in the Models Folder
```cs
using System.ComponentModel.DataAnnotations;
...
[DisplayFormat(DataFormatString = "{0:c2}", ApplyFormatInEditMode = true)]
public decimal Price { get; set; }
```
asp-format 属性优先于 DisplayFormat 属性，因此我从视图中删除了该属性，如清单 27-19 所示。
Listing 27-19. Removing an Attribute in the Form.cshtml File in the Views/Form Folder
```html
<input class="form-control" asp-for="Price" />
```

### Displaying Values from Related Data in input Elements
使用 Entity Framework Core 时，您经常需要显示从相关数据中获取的数据值，这可以使用 asp-for 属性轻松完成，因为模型表达式允许选择嵌套的导航属性。首先，清单 27-20 在提供给视图的视图模型对象中包含相关数据。
Listing 27-20. Including Related Data in the FormController.cs File in the Controllers Folder
```cs
using Microsoft.EntityFrameworkCore;
...
public async Task<IActionResult> Index(long id = 1) {
    return View("Form", await context.Products.Include(p => p.Category)
        .Include(p => p.Supplier).FirstAsync(p => p.ProductId == id));
}
```
请注意，我不需要担心处理相关数据中的循环引用，因为视图模型对象未序列化。循环引用问题仅对 Web 服务控制器很重要。在清单 27-21 中，我更新了 Form 视图以包含使用 asp-for 属性选择相关数据的输入元素。
Listing 27-21. Displaying Related Data in the Form.cshtml File in the Views/Form Folder
```html
...
<div class="form-group">
    <label>Category</label>
    @{
        #pragma warning disable CS8602
    }
    <input class="form-control" asp-for="Category.Name" />
    @{
        #pragma warning restore CS8602
    }
</div>
<div class="form-group">
    <label>Supplier</label>
    @{
        #pragma warning disable CS8602
    }
    <input class="form-control" asp-for="Supplier.Name" />
    @{
        #pragma warning restore CS8602
    }
</div>
...
```
asp-for 属性的值是相对于视图模型对象表示的，可以包含嵌套属性，允许我选择 Entity Framework Core 已分配给 Category 和 Supplier 导航属性的相关对象的 Name 属性。  
正如我在第 25 章中解释的那样，空条件运算符不能在模型表达式中使用。这在选择可空相关数据属性（例如 Product.Category 和 Product.Supplier 属性）时会出现问题。在第 25 章中，我通过更改属性的类型使其不可为空来解决此限制，但这并不总是可行的，尤其是当可空属性已用于指示特定条件时。  
在清单 27-21 中，我使用了 #pragma warning 表达式来禁用警告 CS8602 的代码分析，这是在未安全访问可为 null 的值时生成的警告。标签助手在处理 asp-for 属性时能够处理空值，这意味着警告并不表示潜在的问题。  
您可以选择简单地忽略编译器产生的警告，但我更愿意解决潜在的问题或明确禁用警告，以便很明显警告已被调查并故意忽略，而不是没有被注意到。   
Razor Pages 中使用了相同的技术，只是属性是相对于页面模型对象表示的，如清单 27-22 所示。
Listing 27-22. Displayed Related Data in the FormHandler.cshtml File in the Pages Folder
```html
@using Microsoft.EntityFrameworkCore
...
<input class="form-control" asp-for="Product.Name" />
...
<div class="form-group">
<label>Price</label>
<input class="form-control" asp-for="Product.Price" />
</div>
<div class="form-group">
<label>Category</label>
@{ #pragma warning disable CS8602 }
<input class="form-control" asp-for="Product.Category.Name" />
@{ #pragma warning restore CS8602 }
</div>
<div class="form-group">
<label>Supplier</label>
@{ #pragma warning disable CS8602 }
<input class="form-control" asp-for="Product.Supplier.Name" />
@{ #pragma warning restore CS8602 }
</div>
```