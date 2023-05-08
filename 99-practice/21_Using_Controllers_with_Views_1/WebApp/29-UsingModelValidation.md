# Using Model Validation
在上一章中，我向您展示了模型绑定过程如何根据 HTTP 请求创建对象。在那一章中，我只是简单地显示了应用程序收到的数据。这是因为用户提供的数据在经过检查以确保应用程序能够使用之前不应使用。事实上，用户经常会输入无效且无法使用的数据，这引出了本章的主题：模型验证。模型验证是确保应用程序收到的数据适合绑定到模型的过程，如果不是这种情况，则向用户提供有助于解释问题的有用信息。该过程的第一部分是检查收到的数据，这是保持应用程序数据完整性的最重要方法之一。拒绝不能使用的数据可以防止在应用程序中出现奇怪和不需要的状态。验证过程的第二部分是帮助用户纠正问题，这同样重要。如果没有纠正问题所需的反馈，用户会感到沮丧和困惑。在面向公众的应用程序中，这意味着用户将简单地停止使用该应用程序。在企业应用程序中，这意味着用户的工作流程将受到阻碍。这两种结果都不可取，但幸运的是，ASP.NET Core 为模型验证提供了广泛的支持。表 29-1 将模型验证放在上下文中。
Table 29-1. Putting Model Validation in Context
| Question | Answer |
|-|-|
| 是什么 |  模型验证是确保请求中提供的数据对于在应用程序中使用有效的过程。|
| 为什么有用 | 用户并不总是输入有效数据，使用未经验证的数据会产生意想不到的错误。|
| 如何用 | 控制器和 Razor 页面检查验证过程的结果，标签助手用于在显示给用户的视图中包含验证反馈。验证可以在模型绑定过程中自动执行，并且可以补充自定义验证。|
| 有没有坑和限制 | 测试验证代码的有效性非常重要，以确保它涵盖应用程序可以接收的所有值范围。|
| 有没有别的选择 | 模型验证是可选的，但最好在使用模型绑定时使用它。|

## Understanding the Need for Model Validation
模型验证是强制执行应用程序对其从客户端接收的数据的要求的过程。如果没有验证，应用程序将尝试对其接收到的任何数据进行操作，这可能会导致立即出现的异常和意外行为，或者随着数据库中填充了不良、不完整或恶意数据而逐渐出现的长期问题。  
目前，接收表单数据的操作和处理程序方法将接受用户提交的任何数据，这就是示例仅显示表单数据而不将其存储在数据库中的原因。  
大多数数据值都有某种约束。这可能涉及要求提供一个值，要求该值是特定类型，并要求该值落在特定范围内。  
例如，在我可以安全地将 Product 对象存储在数据库中之前，我需要确保用户提供了 Name、Price、CategoryId 和 SupplierId 属性的值。 Name 值可以是任何有效的字符串，Price 属性必须是有效的货币金额，CategoryId 和 SupplierId 属性必须对应于数据库中现有的 Supplier 和 Category 产品。在以下部分中，我将演示如何使用模型验证来执行这些要求，方法是检查应用程序接收的数据并在应用程序无法使用用户提交的数据时向用户提供反馈。  

### Validating Data
虽然不明显，但 ASP.NET Core 已经在模型绑定过程中执行了一些基本的数据验证，但它检测到的错误正在被丢弃。清单 29-5 检查验证过程的结果，以便用户提供的数据值只有在有效时才会被使用。  
Listing 29-5. Checking the Validation Outcome in the FormController.cs File in the Controllers Folder
```cs
if (ModelState.IsValid)
```
我使用从 ControllerBase 类继承的 ModelState 属性返回的 ModelStateDictionary 对象来确定用户提供的数据是否有效。  
顾名思义，ModelStateDictionary 类是一个字典，用于跟踪模型对象状态的详细信息，重点是验证错误。表 29-3 描述了最重要的 ModelStateDictionary 成员。  
Table 29-3. Selected ModelStateDictionary Members
**略**  

如果验证过程检测到问题，则 IsValid 属性将返回 false。 SubmitForm 操作方法通过返回相同的视图来处理无效数据，如下所示：
```cs
return View("Form");
```
通过调用 View 方法处理验证错误可能看起来很奇怪，但提供给视图的上下文数据包含模型验证错误的详细信息；标签助手使用这些细节来转换输入元素。  
要查看其工作原理，请重新启动 ASP.NET Core 并使用浏览器请求 http://localhost:5000/controllers/form。清除名称字段的内容并单击提交按钮。浏览器显示的内容不会有任何可见的变化，但如果您检查 Name 字段的输入元素，您会看到该元素已被转换。这是提交表单之前的输入元素： 
```html
<input class="form-control" type="text" data-val="true"
    data-val-required="The Name field is required." id="Name"
    name="Name" value="Kayak">
```
这是提交表单之后的输入元素： 
```html
<input class="form-control input-validation-error" type="text" data-val="true"
data-val-required="The Name field is required." id="Name"
name="Name" value="">
```
标签助手将值验证失败的元素添加到 `input-validation-error` 类，然后可以设置样式以突出显示问题给用户。  
您可以通过在样式表中定义自定义 CSS 样式来做到这一点，但如果您想使用 Bootstrap 等 CSS 库提供的内置验证样式，则需要做一些额外的工作。添加到输入元素的类的名称不能更改，这意味着需要一些 JavaScript 代码来映射 ASP.NET Core 使用的名称和 Bootstrap 提供的 CSS 错误类。  
像这样使用 JavaScript 代码可能很尴尬，使用自定义 CSS 样式可能很诱人，即使在使用像 Bootstrap 这样的 CSS 库时也是如此。但是，Bootstrap 中用于验证类的颜色可以通过使用主题或通过自定义包和定义自己的样式来覆盖，这意味着您必须确保对主题的任何更改与您定义的任何自定义样式的相应更改相匹配。理想情况下，Microsoft 将在 ASP.NET Core 的未来版本中使验证类名称可配置，但在那之前，使用 JavaScript 应用 Bootstrap 样式是比创建自定义样式表更可靠的方法。  
要定义 JavaScript 代码以便控制器和 Razor 页面都可以使用它，请使用 Visual Studio 中的 Razor View - Empty 模板将名为 _Validation.cshtml 的文件添加到 Views/Shared 文件夹，其内容如清单 29- 6.   
Listing 29-6. The Contents of the _Validation.cshtml File in the Views/Shared Folder  

我将使用新文件作为局部视图，其中包含一个脚本元素，该元素使用浏览器的 JavaScript 文档对象模型 (DOM) API 来定位属于 inputvalidation-error 类成员的输入元素，并将它们添加到 is-invalid 类（Bootstrap 使用它来设置表单元素的错误颜色）。清单 29-7 使用 partial 标签助手将新的分部视图合并到 HTML 表单中，以便突出显示验证错误的字段。 
Listing 29-7. Including a Partial View in the Form.cshtml File in the Views/Form Folder  
```html
<partial name="_Validation" />
```
JavaScript 代码在浏览器解析完 HTML 文档中的所有元素时运行，其效果是突出显示已分配给 inputvalidaton-error 类的输入元素。您可以通过重新启动 ASP.NET Core、导航到 http://localhost:5000/controllers/form、清除 Name 字段的内容并提交表单来查看效果，结果如图 29-2 所示。  

### Displaying Validation Messages
图 29-2 清楚地表明 Name 字段有问题，但没有提供有关检测到的问题的任何详细信息。为用户提供更多信息需要使用不同的标签助手，它将问题的摘要添加到视图中，如清单 29-8 所示。   
Listing 29-8. Displaying a Summary in the Form.cshtml File in the Views/Form Folder
```html
<form asp-action="submitform" method="post" id="htmlform">
    <div asp-validation-summary="All" class="text-danger"></div>
</form>
```
ValidationSummaryTagHelper 类检测 div 元素上的 asp-validation-summary 属性，并通过添加描述已记录的任何验证错误的消息进行响应。 asp-validation-summary 属性的值是来自 ValidationSummary 枚举的一个值，它定义了表 29-4 中显示的值，我稍后将对此进行演示。
Table 29-4. The ValidationSummary Values  
| Name | Description |
|-|-|
| All | 该值用于显示已记录的所有验证错误。|
| ModelOnly | 此值仅用于显示整个模型的验证错误，不包括为单个属性记录的错误，如“显示模型级消息”部分所述。|
| None | 该值用于禁用标签助手，这样它就不会转换 HTML 元素。|

显示错误消息有助于用户理解无法处理表单的原因。重新启动 ASP.NET Core，请求 http://localhost:5000/controllers/form，清除 Name 字段，然后提交表单。如图 29-3 所示，现在有一条错误消息说明已检测到问题。  

### Understanding the Implicit Validation Checks
图 29-3 中显示的错误消息是由隐式验证过程生成的，该过程在模型绑定期间自动执行。  
隐式验证简单但有效，有两个基本检查：用户必须为所有使用不可空类型定义的属性提供值，ASP.NET Core 必须能够解析在 HTTP 中接收到的字符串值请求到相应的属性类型。  
提醒一下，这里是Product类的定义，它是用来接收表单数据的类： `Models/Product.cs`  
Name属性的类型是一个不可为空的字符串，这就是清除字段时报验证错误的原因在上一节中。 Name 属性没有解析问题，因为从 HTTP 请求接收的字符串值不需要任何类型转换。在 Price 字段中输入 10 并提交表格；您将看到一个错误，表明 ASP.NET Core 无法将 HTTP 请求中的字符串解析为 Price 属性所需的十进制值。

### Performing Explicit Validation
隐式验证处理基础问题，但大多数应用程序需要额外的检查以确保它们接收到有用的数据。这称为**显式验证**，它是使用表 29-4 中描述的 ModelStateDictionary 方法完成的。  
为避免显示冲突的错误消息，显式验证通常仅在用户提供已通过隐式检查的值时进行。 ModelStateDictionary.GetValidationState 方法用于查看是否为模型属性记录了任何错误。 GetValidationState 方法从 ModelValidationState 枚举返回一个值，该枚举定义表 29-5 中描述的值。
Table 29-5. The ModelValidationState Values  
| Name | Description |
|-|-|
| Unvalidated | 该值意味着没有对模型属性执行任何验证，通常是因为请求中没有与属性名称对应的值。|
| Valid | 该值表示与该属性相关联的请求值是有效的。|
| Invalid | 此值表示与该属性关联的请求值无效，不应使用。|
| Skipped | 这个值意味着模型属性还没有被处理，这通常意味着有太多的验证错误以至于没有必要继续执行验证检查。|

清单 29-9 为 Product 类定义的一些属性定义了显式验证检查。  
Listing 29-9. Performing Explicit Validation in the FormController.cs File in the Controllers Folder  
```cs
using Microsoft.AspNetCore.Mvc.ModelBinding;
...
if (ModelState.GetValidationState(nameof(Product.Price))
    == ModelValidationState.Valid && product.Price <= 0)
{
    ModelState.AddModelError(nameof(Product.Price), "Enter a positive price");
}
if (ModelState.GetValidationState(nameof(Product.CategoryId))
    == ModelValidationState.Valid && !context.Categories.Any(c => c.CategoryId == product.CategoryId))
{
    ModelState.AddModelError(nameof(Product.CategoryId), "Enter an existing category ID");
}
if (ModelState.GetValidationState(nameof(Product.SupplierId))
    == ModelValidationState.Valid && !context.Suppliers.Any(s => s.SupplierId == product.SupplierId))
{
    ModelState.AddModelError(nameof(Product.SupplierId), "Enter an existing supplier ID");
}
```
作为使用 ModelStateDictionary 的示例，请考虑如何验证 Price 属性。 Product 类的验证要求之一是确保用户为 Price 属性提供正值。这是 ASP.NET Core 无法从 Product 类推断出来的东西，因此需要显式验证。  
我首先确保 Price 属性不存在验证错误：
```cs
...
ModelState.GetValidationState(nameof(Product.Price)) == ModelValidationState.Valid
...
```
我想确保用户提供的 Price 值大于零，但如果用户提供了模型联编程序无法转换为十进制值的值。在执行我自己的验证检查之前，我使用 GetValidationState 方法确定 Price 属性的验证状态：
```cs
...
product.Price <= 0
...
```
如果用户提供的值小于或等于零，那么我将使用 AddModelError 方法记录验证错误： 
```cs
ModelState.AddModelError(nameof(Product.Price), "Enter a positive price");
```
AddModelError 方法的参数是属性名称和将显示给用户的字符串描述验证问题。对于 CategoryId 和 SupplierId 属性，我遵循类似的过程并使用 Entity Framework Core 来确保用户提供的值与存储在数据库中的 ID 相对应。  
执行显式验证检查后，我使用 ModelState.IsValid 属性查看是否有错误，这意味着隐式或显式验证错误将以相同的方式报告。  
要查看显式验证的效果，请重新启动 ASP.NET Core，请求 http://localhost:5000/controllers/form，然后在 Price、CategoryId 和 SupplierId 字段中输入 0。提交表单，您将看到如图 29-5 所示的验证错误。  

### Configuring the Default Validation Error Messages
当涉及到显示的验证消息时，验证过程有一些不一致。并非模型绑定器生成的所有验证消息都对用户有帮助，您可以通过清除价格字段并提交表单来查看。空字段会产生以下消息：
```log
The value '' is invalid
```
隐式验证过程在找不到属性值时将此消息添加到 ModelStateDictionary 。例如，与字符串属性的缺失值相比，小数属性的缺失值会导致不同的消息，而且用处不大。这是因为执行验证检查的方式不同。一些验证错误的默认消息可以使用 DefaultModelBindingMessageProvider 类定义的方法替换为自定义消息，表 29-6 中描述了其中最有用的方法。  
Table 29-6. Useful DefaultModelBindingMessageProvider Methods  
**忽略**

表中描述的每个方法都接受一个函数，该函数被调用以获取验证消息以显示给用户。这些方法是通过 Program.cs 文件中的选项模式应用的，如清单 29-10 所示，我在其中替换了当值为 null 或无法转换时显示的默认消息。  
Listing 29-10. Changing a Validation Message in the Program.cs File in the WebApp Folder
```cs
using Microsoft.AspNetCore.Mvc;
...
builder.Services.Configure<MvcOptions>(opts => opts.ModelBindingMessageProvider
    .SetValueMustNotBeNullAccessor(value => "Please enter a value"));
```
您指定的函数接收用户提供的值，尽管这在处理空值时不是特别有用。要查看自定义消息，请重新启动 ASP.NET Core，使用浏览器请求 http://localhost:5000/controllers/form，然后提交带有空价格字段的表单。响应将包含自定义错误消息，如图 29-6 所示。
图 29-6 还显示了缺少名称字段时显示的消息，该消息不受表 29-6 中设置的影响。这是验证不可为 null 的模型属性的方式的一个怪癖，其行为就像 Required 属性已应用于不可为 null 的属性一样。我将在本章后面描述 Required 属性，并解释如何使用它来更改不可为空属性的错误消息。

### Displaying Property-Level Validation Messages
尽管自定义错误消息比默认错误消息更有意义，但它仍然没有那么有用，因为它没有清楚地指出问题与哪个字段有关。对于此类错误，将验证错误消息与包含问题数据的 HTML 元素一起显示更有用。这可以使用 ValidationMessageTag 标签助手来完成，它查找具有 asp-validationfor 属性的 span 元素，该属性用于指定应显示错误消息的属性。  
在清单 29-11 中，我为表单中的每个输入元素添加了属性级验证消息元素。  
Listing 29-11. Adding Property-Level Messages in the Form.cshtml File in the Views/Form Folder
```html
<div><span asp-validation-for="Name" class="text-danger"></span></div>
...
<div><span asp-validation-for="Price" class="text-danger"></span></div>
...
<div><span asp-validation-for="CategoryId" class="text-danger"></span></div>
...
<div><span asp-validation-for="SupplierId" class="text-danger"></span></div>
```
由于 span 元素是内联显示的，因此必须小心呈现验证消息，以明确消息与哪个元素相关。您可以通过重新启动 ASP.NET Core、请求 http://localhost:5000/controllers/form、清除名称和价格字段并提交表单来查看新验证消息的效果。如图 29-7 所示，响应包括文本字段旁边的验证消息。

### Displaying Model-Level Messages
验证摘要消息似乎是多余的，因为它重复了属性级消息。但摘要有一个有用的技巧，即能够显示适用于整个模型的消息，而不仅仅是单个属性。这意味着您可以报告由单个属性的组合引起的错误，否则很难用属性级消息来表达这些错误。  
在清单 29-12 中，我向 FormController.SubmitForm 操作添加了一个检查，当 Name 值以 Small 开头且 Price 值超过 100 时记录验证错误。  
Listing 29-12. Performing Model-Level Validation in the FormController.cs File in the Controllers Folder
```cs
if (ModelState.GetValidationState(nameof(Product.Name)) == ModelValidationState.Valid
    && ModelState.GetValidationState(nameof(Product.Price)) == ModelValidationState.Valid
    && product.Name.ToLower().StartsWith("small") && product.Price > 100) 
{
    ModelState.AddModelError("", "Small products cannot cost more than $100");
}
```
如果用户输入以 Small 开头的名称值和大于 100 的价格值，则会记录模型级验证错误。只有当各个属性值没有验证问题时，我才会检查值的组合，这确保用户不会看到冲突的消息。与整个模型相关的验证错误使用带有空字符串作为第一个参数的 AddModelError 来记录。
清单 29-13 将 asp-validation-summary 属性的值更改为 ModelOnly，这排除了属性级错误，这意味着摘要将仅显示适用于整个模型的那些错误。  
Listing 29-13. Configuring the Validation Summary in the Form.cshtml File in the Views/Form Folder
```html
<div asp-validation-summary="ModelOnly" class="text-danger"></div>
```
重新启动 ASP.NET Core 并请求 http://localhost:5000/controllers/form。在名称字段中输入 Small Kayak，在价格字段中输入 150，然后提交表格。响应将包括模型级错误消息，如图 29-8 所示。  

## Explicitly Validating Data in a Razor Page
Razor 页面验证依赖于上一节中控制器中使用的相同功能。清单 29-14 向 FormHandler 页面添加了显式验证检查和错误摘要。  
Listing 29-14. Validating Data in the FormHandler.cshtml File in the Pages Folder
```cshtml
@page "/pages/form/{id:long?}"
@model FormHandlerModel
@using Microsoft.AspNetCore.Mvc.RazorPages
@using Microsoft.EntityFrameworkCore
@using Microsoft.AspNetCore.Mvc.ModelBinding

<partial name="_Validation" />

<div class="m-2">
    <h5 class="bg-primary text-white text-center p-2">HTML Form</h5>
    <form asp-page="FormHandler" method="post" id="htmlform">
        <div class="form-group">
            <label>Name</label>
            <div>
                <span asp-validation-for="Product.Name" class="text-danger"></span>
            </div>
            <input class="form-control" asp-for="Product.Name" />
        </div>
        <div class="form-group">
            <label>Price</label>
            <div>
                <span asp-validation-for="Product.Price" class="text-danger"></span>
            </div>
            <input class="form-control" asp-for="Product.Price" />
        </div>
        <div class="form-group">
            <label>Category Name</label>
            @{
                #pragma warning disable CS8602
            }
            <input class="form-control" asp-for="Product.Category.Name" />
            @{
                #pragma warning restore CS8602
            }
        </div>
        <div class="form-group">
            <label>CategoryId</label>
            <div>
                <span asp-validation-for="Product.CategoryId" class="text-danger">
                </span>
            </div>
            <input class="form-control" asp-for="Product.CategoryId" />
        </div>
        <div class="form-group">
            <label>SupplierId</label>
            <div>
                <span asp-validation-for="Product.SupplierId" class="text-danger">
                </span>
            </div>
            <input class="form-control" asp-for="Product.SupplierId" />
        </div>
        <button type="submit" class="btn btn-primary mt-2">Submit</button>
    </form>
    <button form="htmlform" asp-page="FormHandler" class="btn btn-primary mt-2">
        Submit (Outside Form)
    </button>
</div>

@functions {
    [IgnoreAntiforgeryToken]
    public class FormHandlerModel : PageModel
    {
        private DataContext context;
        public FormHandlerModel(DataContext dbContext)
        {
            context = dbContext;
        }

        [BindProperty]
        public Product? Product { get; set; }

        public async Task OnGetAsync(long id = 1)
        {
            Product = await context.Products.FirstAsync(p => p.ProductId == id);
        }

        public IActionResult OnPost()
        {
            if (ModelState.GetValidationState("Product.Price") == ModelValidationState.Valid && Product.Price < 1)
            {
                ModelState.AddModelError("Product.Price", "Enter a positive price");
            }
            if (ModelState.GetValidationState("Product.Name")
                == ModelValidationState.Valid
                && ModelState.GetValidationState("Product.Price")
                == ModelValidationState.Valid
                && Product.Name.ToLower().StartsWith("small")
                && Product.Price > 100)
            {
                ModelState.AddModelError("",
                "Small products cannot cost more than $100");
            }
            if (ModelState.GetValidationState("Product.CategoryId")
            == ModelValidationState.Valid &&
            !context.Categories.Any(c => c.CategoryId == Product.CategoryId))
            {
                ModelState.AddModelError("Product.CategoryId",
                "Enter an existing category ID");
            }
            if (ModelState.GetValidationState("Product.SupplierId")
            == ModelValidationState.Valid &&
            !context.Suppliers.Any(s => s.SupplierId == Product.SupplierId))
            {
                ModelState.AddModelError("Product.SupplierId",
                "Enter an existing supplier ID");
            }
            if (ModelState.IsValid)
            {
                TempData["name"] = Product.Name;
                TempData["price"] = Product.Price.ToString();
                TempData["categoryId"] = Product.CategoryId.ToString();
                TempData["supplierId"] = Product.SupplierId.ToString();
                return RedirectToPage("FormResults");
            }
            else
            {
                return Page();
            }
        }
    }
}
```

PageModel 类定义了一个 ModelState 属性，该属性等同于我在控制器中使用的属性并允许记录验证错误。验证过程是相同的，但在记录错误时必须小心，以确保名称与 Razor Pages 使用的模式相匹配。当我在控制器中记录错误时，我使用 nameof 关键字来选择与错误相关的属性，如下所示：   
```cs
...
ModelState.AddModelError(nameof(Product.Price),"Enter a positive price");
...
```
这是一个常见的约定，因为它可以确保错别字不会导致错误记录错误。此表达式在 Razor 页面中不起作用，其中必须针对 Product.Price 而不是 Price 记录错误，以反映 Razor 页面中的 @Model 表达式返回页面模型对象，如下所示：
```cs
...
ModelState.AddModelError("Product.Price", "Enter a positive price");
...
```
要测试验证过程，重启ASP.NET Core，使用浏览器请求http://localhost:5000/pages/form，提交空字段或无法转换成Product类要求的C#类型值的表单。错误消息的显示与控制器的错误消息相同，如图 29-9 所示。 （值 1、2 和 3 对 CategoryId 和 SupplierId 字段都有效。）

## Specifying Validation Rules Using Metadata
将验证逻辑放入操作方法中的一个问题是它最终会在从用户接收数据的每个操作或处理程序方法中重复。为了帮助减少重复，验证过程支持使用属性直接在模型类中表达模型验证规则，确保无论使用哪种操作方法处理请求，都将应用同一组验证规则。在清单 29-15 中，我已将属性应用于 Product 类以描述 Name 和 Price 属性所需的验证。  
Listing 29-15. Applying Validation Attributes in the Product.cs File in the Models Folder
```cs
[Required(ErrorMessage = "Please enter a value")]
public string Name { get; set; } = string.Empty;

[Range(1, 999999, ErrorMessage = "Please enter a positive price")]
[Column(TypeName = "decimal(8, 2)")]
public decimal Price { get; set; }
```
我在清单中使用了两个验证属性： Required 和 Range 。 Required 特性指定如果用户未提交属性值则为验证错误，这在您拥有可为空的属性但想要从用户那里获取值时很有用。  
我在清单 29-15 中使用了 Required 属性来更改当用户没有为 Name 属性提供值时显示的错误消息。如前所述，隐式验证检查在处理不可空属性的方式上不一致，但这可以使用所有验证属性定义的 ErrorMessage 参数来纠正。  
我还在清单 29-15 中应用了 Range 属性，它允许我为 Price 属性指定一组可接受的值。表 29-7 显示了一组可用的内置验证属性。  
Table 29-7. The Built-in Validation Attributes
**忽略**

validation 属性的使用允许我从 action 方法中删除一些显式验证，如清单 29-16 所示。  
Listing 29-16. Removing Explicit Validation in the FormController.cs File in the Controllers Folder  
```cs
[HttpPost]
public IActionResult SubmitForm(Product product) {
    //if (ModelState.GetValidationState(nameof(Product.Price))
    // == ModelValidationState.Valid && product.Price <= 0) {
    // ModelState.AddModelError(nameof(Product.Price),
    // "Enter a positive price");
    //}
    ...
}
```
要查看正在运行的验证属性，请重新启动 ASP.NET Core MVC，请求 http://localhost:5000/controllers/form，清除 Name 和 Price 字段，然后提交表单。响应将包括由 Price 字段的属性产生的验证错误和 Name 字段的新消息，如图 29-10 所示。验证属性在调用操作方法之前应用，这意味着在执行模型级验证时，我仍然可以依赖模型状态来确定各个属性是否有效。

**VALIDATION WORK AROUNDS**
在使用验证属性时，获得您需要的验证结果需要多加注意。例如，如果要确保用户已选中复选框，则不能使用 Required 属性，因为当复选框未选中时浏览器将发送 false 值，这将始终通过 Required 属性应用的检查。相反，使用 Range 属性并将最小值和最大值指定为 true，如下所示： 
```
...
[Range(typeof(bool), "true", "true", ErrorMessage="You must check the box")]
...
```
如果这种解决方法感觉不舒服，那么您可以创建自定义验证属性，如下一节所述。要查看正在运行的验证属性，请重新启动 ASP.NET Core MVC，请求 http://localhost:5000/controllers/form，清除 Name 和 Price 字段，然后提交表单。响应将包括属性产生的验证错误，如图 29-10 所示。

**UNDERSTANDING WEB SERVICE CONTROLLER VALIDATION**
**使用 ApiController 属性装饰的控制器不需要检查 ModelState.IsValid 属性**。相反，只有在没有验证错误时才会调用 action 方法，这意味着您始终可以依赖于通过模型绑定功能接收经过验证的对象。如果检测到任何验证错误，则请求终止，并向浏览器发送错误响应。

### Creating a Custom Property Validation Attribute
可以通过创建一个扩展 ValidationAttribute 类的属性来扩展验证过程。为了演示，我创建了 WebApp/Validation 文件夹并向其中添加了一个名为 PrimaryKeyAttribute.cs 的类文件。
自定义属性覆盖 IsValid 方法（使用要检查的值调用）和 ValidationContext 对象，该对象提供有关验证过程的上下文并通过其 GetService 方法提供对应用程序服务的访问。  
在清单 29-17 中，自定义属性接收 Entity Framework Core 数据库上下文类的类型和模型类的类型。在 IsValid 方法中，该属性获取上下文类的一个实例，并使用它来查询数据库以确定该值是否已用作主键值。  
如果您修改从 model binder 接收到的对象，您可能需要再次执行验证过程。对于这些情况，请使用 ModelState.Clear 方法清除任何现有的验证错误并调用 TryValidateModel 方法。  

### Creating a Custom Model Validation Attribute
自定义验证属性也可用于执行模型级验证。为了演示，我在 Validation 文件夹中添加了一个名为 PhraseAndPriceAttribute.cs 的类文件。该属性配置有 Phrase 和 Price 属性，在 IsValid 方法中使用这些属性来检查模型对象的 Name 和 Price 属性。属性级自定义验证属性直接应用于它们验证的属性，模型级属性应用于整个类，如清单 29-19 所示。  
Listing 29-19. Applying Custom Validation Attributes in the Product.cs File in the Models Folder
```cs
using WebApp.Validation;
[PhraseAndPrice(Phrase = "Small", Price = "100")]
public class Product {
    ...
    [PrimaryKey(ContextType = typeof(DataContext), DataType = typeof(Category))]
    public long CategoryId { get; set; }
    public Category? Category { get; set; }

    [PrimaryKey(ContextType = typeof(DataContext), DataType = typeof(Supplier))]
    public long SupplierId { get; set; }
    ...
}
```
自定义属性允许从 Form 控制器的操作方法中删除剩余的显式验证语句，如清单 29-20 所示。  
Listing 29-20. Removing Explicit Validation in the FormController.cs File in the Controllers Folder
```cs
[HttpPost]
public IActionResult SubmitForm(Product product)
{
    if (ModelState.IsValid)
    {
        TempData["name"] = product.Name;
        TempData["price"] = product.Price.ToString();
        TempData["categoryId"] = product.CategoryId.ToString();
        TempData["supplierId"] = product.SupplierId.ToString();
        return RedirectToAction(nameof(Results));
    }
    else
    {
        return View("Form");
    }
}
```

**验证属性会在调用操作方法之前自动应用**，这意味着只需读取 ModelState.IsValid 属性即可确定验证结果。

**Using a Custom Model Validation Attribute in a Razor Page**
需要进行改编以支持 Razor Pages 中的自定义模型验证属性。当在 Razor 页面中应用验证属性时，它生成的错误与 Product 属性相关联，而不是与整个模型相关联，这意味着错误不会由验证摘要标签帮助程序显示。  
要解决此问题，请将名为 ModelStateExtensions.cs 的类文件添加到 WebApp/Validation 文件夹，并使用它来定义如清单 29-21 所示的扩展方法。 
Listing 29-22. Removing Explicit Validation in the FormHandler.cshtml File in the Pages Folder  
```html
@using WebApp.Validation

```

PromotePropertyErrors 扩展方法定位与指定属性关联的验证错误并添加相应的模型级错误。清单 29-22 从 Razor 页面中删除了显式验证并应用了新的扩展方法。通过自定义属性表达验证消除了控制器和 Razor 页面之间的代码重复，并确保在模型绑定用于 Product 对象的任何地方一致地应用验证。要测试验证属性，请重新启动 ASP.NET Core 并导航到 http://localhost:5000/controllers/form 或 http://localhost:5000/pages/form。清除表单字段或输入错误的键值并提交表单，您将看到属性产生的错误消息，其中一些如图 29-11 所示。 （值 1、2 和 3 对 CategoryId 和 SupplierId 字段都有效。）


## Performing Client-Side Validation
到目前为止，我演示的验证技术都是服务器端验证的示例。这意味着用户将他们的数据提交给服务器，服务器验证数据并发回验证结果（处理数据成功或需要更正的错误列表）。在 Web 应用程序中，用户通常期望立即获得验证反馈，而无需向服务器提交任何内容。这称为客户端验证，使用 JavaScript 实现。用户输入的数据在发送到服务器之前经过验证，为用户提供即时反馈和纠正任何问题的机会。 ASP.NET Core 支持不显眼的客户端验证。术语不显眼意味着验证规则是使用添加到视图生成的 HTML 元素的属性来表达的。这些属性由 Microsoft 分发的 JavaScript 库解释，然后配置 jQuery Validation 库，该库执行实际的验证工作。在以下部分中，我将向您展示内置验证支持的工作原理，并演示如何扩展功能以提供自定义客户端验证。第一步是安装处理验证的 JavaScript 包。打开一个新的 PowerShell 命令提示符，导航到 WebApp 项目文件夹，然后运行清单 29-23 中所示的命令。安装包后，将清单 29-24 中所示的元素添加到 Views/Shared 文件夹中的 _Validation.cshtml 文件，这提供了一种将验证与应用程序中的现有 jQuery 代码一起引入的便捷方法。 ASP.NET Core 表单标签帮助器将 data-val* 属性添加到描述字段验证约束的输入元素。以下是添加到 Name 字段的输入元素的属性，例如： 不显眼的验证 JavaScript 代码查找这些属性，并在用户尝试提交表单时在浏览器中执行验证。表单不会被提交，如果存在验证问题，则会显示错误。在没有未解决的验证问题之前，数据不会发送到应用程序。 JavaScript 代码查找具有 data-val 属性的元素，并在用户提交表单时在浏览器中执行本地验证，而无需向服务器发送 HTTP 请求。您可以通过运行应用程序并在使用 F12 工具提交表单时看到效果，请注意即使没有向服务器发送 HTTP 请求也会显示验证错误消息。

**AVOIDING CONFLICTS WITH BROWSER VALIDATION**
当前的一些 HTML5 浏览器支持基于应用于输入元素的属性的简单客户端验证。一般的想法是，例如，已应用 required 属性的输入元素将导致浏览器在用户尝试提交表单而不提供值时显示验证错误。如果你使用标签助手生成表单元素，就像我在本章中所做的那样，那么浏览器验证不会有任何问题，因为浏览器会忽略分配了数据属性的元素。但是，如果您无法完全控制应用程序中的标记，您可能会遇到问题，当您传递在别处生成的内容时经常会发生这种情况。结果是 jQuery 验证和浏览器验证都可以对表单进行操作，这只会让用户感到困惑。为避免此问题，您可以将 novalidate 属性添加到表单元素以禁用浏览器验证。一个很好的客户端验证特性是指定验证规则的相同属性应用于客户端和服务器。这意味着来自不支持 JavaScript 的浏览器的数据会受到与支持 JavaScript 的浏览器相同的验证，而无需任何额外的工作。要测试客户端验证功能，请重新启动 ASP.NET Core，请求 http://localhost:5000/controllers/form 或 http://localhost:5000/pages/form，清除 Name 字段，然后单击 Submit 按钮.错误消息看起来像服务器端验证生成的消息，但是如果您在该字段中输入文本，您将看到错误消息在 JavaScript 代码响应用户交互时立即消失，如图 29-12 所示。客户端验证功能支持内置的属性级属性。该功能可以扩展，但需要精通 JavaScript，并且需要直接使用 jQuery Validation 包。有关详细信息，请参阅 https://jqueryvalidation.org/documentation。如果您不想开始编写 JavaScript 代码，那么您可以遵循使用客户端验证进行内置验证检查和使用服务器端验证进行自定义验证的通用模式。

## Performing Remote Validation
远程验证模糊了客户端和服务器端验证之间的界限：验证检查由客户端 JavaScript 代码强制执行，但验证检查是通过向应用程序发送异步 HTTP 请求以测试输入到表单中的值来执行的由用户。远程验证的一个常见示例是在用户名必须唯一时检查用户名在应用程序中是否可用，用户提交数据并执行客户端验证。作为此过程的一部分，向服务器发出异步 HTTP 请求以验证所请求的用户名。如果用户名已被占用，则会显示验证错误，以便用户可以输入另一个值。这看起来像是常规的服务器端验证，但这种方法有一些好处。首先，只有一些属性会被远程验证；客户端验证的好处仍然适用于用户输入的所有其他数据值。其次，请求相对轻量级并且侧重于验证，而不是处理整个模型对象。第三个区别是远程验证是在后台执行的。用户不必单击提交按钮，然后等待呈现和返回新视图。它可以提供更灵敏的用户体验，尤其是当浏览器和服务器之间的网络速度较慢时。也就是说，远程验证是一种妥协。它在客户端和服务器端验证之间取得了平衡，但它确实需要向应用程序服务器发出请求，并且验证速度不如正常的客户端验证。对于示例应用程序，我将使用远程验证来确保用户输入 CategoryId 和 SupplierId 属性的现有键值。第一步是创建一个 Web 服务控制器，其操作方法将执行验证检查。我使用清单 29-25 中所示的代码将一个名为 ValidationController.cs 的类文件添加到 Controllers 文件夹中。验证操作方法必须定义一个参数，其名称与它们将验证的字段相匹配，这允许模型绑定过程从请求查询字符串中提取要测试的值。来自 action 方法的响应必须是 JSON，并且只能是 true 或 false，指示一个值是否可以接受。清单 29-25 中的操作方法接收候选值并检查它们是否已用作 Category 或 Supplier 对象的数据库键。
我本可以利用模型绑定，以便将操作方法​​的参数转换为 long 值，但这样做意味着如果用户输入的值无法转换为长型。如果模型绑定器无法转换值，则 MVC 框架无法调用操作方法并且无法执行验证。通常，远程验证的最佳方法是在操作方法中接受字符串参数并显式执行任何类型转换、解析或模型绑定。为了使用远程验证方法，我将 Remote 属性应用于 Product 类中的 CategoryId 和 SupplierId 属性，如清单 29-26 所示。 Remote 属性的参数指定验证控制器的名称及其操作方法。我还使用了可选的 ErrorMessage 参数来指定验证失败时将显示的错误消息。要查看远程验证，请重新启动 ASP.NET Core，导航至 http://localhost:5000/controllers/form，输入无效的键值，然后提交表单。您将看到一条错误消息，并且在每次按键后都会验证输入元素的值，如图 29-13 所示。 （只有值 1、2 和 3 对 CategoryId 和 SupplierId 字段均有效。）验证操作方法将在用户首次提交表单时调用，并在每次编辑数据时再次调用。对于文本输入元素，每次击键都会导致对服务器的调用。对于某些应用程序，这可能是大量请求，并且在指定应用程序在生产中所需的服务器容量和带宽时必须考虑在内。此外，您可能选择不对验证成本高昂的属性使用远程验证（该示例重复查询数据库中的键值，这可能对所有应用程序或数据库都不适用）。远程验证在 Razor 页面中有效，但必须注意用于验证值的异步 HTTP 请求中使用的名称。对于上一节中的控制器示例，浏览器将像这样向 URL 发送请求： 但对于示例 Razor Page，URL 将是这样的，反映了页面模型的使用： 我更喜欢解决这种差异的方式是通过向将接受两种类型请求的验证操作方法添加参数，使用前面章节中描述的模型绑定功能很容易做到这一点，如清单 29-27 所示。
KeyTarget 类被配置为绑定到请求的 Product 部分，其属性将匹配两种类型的远程验证请求。每个操作方法都被赋予了一个 KeyTarget 参数，如果现有参数没有收到任何值，则使用该参数。这允许相同的操作方法适应两种类型的请求，您可以通过重新启动 ASP.NET Core，导航到 http://localhost:5000/pages/form，输入一个不存在的键值，然后单击提交按钮来查看，这将产生如图 29-14 所示的响应。