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
JavaScript 代码在浏览器解析完 HTML 文档中的所有元素时运行，其效果是突出显示已分配给 inputvalidaton-error 类的输入元素。您可以通过重新启动 ASP.NET Core、导航到 http://localhost:5000/controllers/form、清除 Name 字段的内容并提交表单来查看效果，结果如图 29-2 所示。图 29-2 清楚地表明 Name 字段有问题，但没有提供有关检测到的问题的任何详细信息。为用户提供更多信息需要使用不同的标签助手，它将问题的摘要添加到视图中，如清单 29-8 所示。 ValidationSummaryTagHelper 类检测 div 元素上的 asp-validation-summary 属性，并通过添加描述已记录的任何验证错误的消息进行响应。 asp-validation-summary 属性的值是来自 ValidationSummary 枚举的一个值，它定义了表 29-4 中显示的值，我稍后将对此进行演示。显示错误消息有助于用户理解无法处理表单的原因。重新启动 ASP。 NET Core，请求 http://localhost:5000/controllers/form，清除 Name 字段，然后提交表单。如图 29-3 所示，现在有一条错误消息说明已检测到问题。
图 29-3 中显示的错误消息是由隐式验证过程生成的，该过程在模型绑定期间自动执行。隐式验证简单但有效，有两个基本检查：用户必须为所有使用不可空类型定义的属性提供值，ASP.NET Core 必须能够解析在 HTTP 中接收到的字符串值请求到相应的属性类型。提醒一下，这里是Product类的定义，它是用来接收表单数据的类： Name属性的类型是一个不可为空的字符串，这就是清除字段时报验证错误的原因在上一节中。 Name 属性没有解析问题，因为从 HTTP 请求接收的字符串值不需要任何类型转换。在 Price 字段中输入 10 并提交表格；您将看到一个错误，表明 ASP.NET Core 无法将 HTTP 请求中的字符串解析为 Price 属性所需的十进制值，如图 29-4 所示。隐式验证处理基础问题，但大多数应用程序需要额外的检查以确保它们接收到有用的数据。这称为显式验证，它是使用表 29-4 中描述的 ModelStateDictionary 方法完成的。为避免显示冲突的错误消息，显式验证通常仅在用户提供已通过隐式检查的值时进行。 ModelStateDictionary.GetValidationState 方法用于查看是否为模型属性记录了任何错误。 GetValidationState 方法从 ModelValidationState 枚举返回一个值，该枚举定义表 29-5 中描述的值。清单 29-9 为 Product 类定义的一些属性定义了显式验证检查。作为使用 ModelStateDictionary 的示例，请考虑如何验证 Price 属性。 Product 类的验证要求之一是确保用户为 Price 属性提供正值。这是 ASP.NET Core 无法从 Product 类推断出来的东西，因此需要显式验证。我首先确保 Price 属性不存在验证错误：我想确保用户提供的 Price 值大于零，但如果用户提供了模型联编程序无法转换为十进制值的值。在执行我自己的验证检查之前，我使用 GetValidationState 方法确定 Price 属性的验证状态：
如果用户提供的值小于或等于零，那么我将使用 AddModelError 方法记录验证错误： AddModelError 方法的参数是属性名称和将显示给用户的字符串描述验证问题。对于 CategoryId 和 SupplierId 属性，我遵循类似的过程并使用 Entity Framework Core 来确保用户提供的值与存储在数据库中的 ID 相对应。执行显式验证检查后，我使用 ModelState.IsValid 属性查看是否有错误，这意味着隐式或显式验证错误将以相同的方式报告。要查看显式验证的效果，请重新启动 ASP.NET Core，请求 http://localhost:5000/controllers/form，然后在 Price、CategoryId 和 SupplierId 字段中输入 0。提交表单，您将看到如图 29-5 所示的验证错误。当涉及到显示的验证消息时，验证过程有一些不一致。并非模型绑定器生成的所有验证消息都对用户有帮助，您可以通过清除价格字段并提交表单来查看。空字段会产生以下消息：隐式验证过程在找不到属性值时将此消息添加到 ModelStateDictionary。例如，与字符串属性的缺失值相比，小数属性的缺失值会导致不同的消息，而且用处不大。这是因为执行验证检查的方式不同。一些验证错误的默认消息可以使用 DefaultModelBindingMessageProvider 类定义的方法替换为自定义消息，表 29-6 中描述了其中最有用的方法。表中描述的每个方法都接受一个函数，该函数被调用以获取验证消息以显示给用户。这些方法是通过 Program.cs 文件中的选项模式应用的，如清单 29-10 所示，我在其中替换了当值为 null 或无法转换时显示的默认消息。您指定的函数接收用户提供的值，尽管这在处理空值时不是特别有用。要查看自定义消息，请重新启动 ASP.NET Core，使用浏览器请求 http://localhost:5000/controllers/form，然后提交带有空价格字段的表单。响应将包含自定义错误消息，如图 29-6 所示。
图 29-6 还显示了缺少名称字段时显示的消息，该消息不受表 29-6 中设置的影响。这是验证不可为 null 的模型属性的方式的一个怪癖，其行为就像 Required 属性已应用于不可为 null 的属性一样。我将在本章后面描述 Required 属性，并解释如何使用它来更改不可为空属性的错误消息。尽管自定义错误消息比默认错误消息更有意义，但它仍然没有那么有用，因为它没有清楚地指出问题与哪个字段有关。对于此类错误，将验证错误消息与包含问题数据的 HTML 元素一起显示更有用。这可以使用 ValidationMessageTag 标签助手来完成，它查找具有 asp-validationfor 属性的 span 元素，该属性用于指定应显示错误消息的属性。在清单 29-11 中，我为表单中的每个输入元素添加了属性级验证消息元素。由于 span 元素是内联显示的，因此必须小心呈现验证消息，以明确消息与哪个元素相关。您可以通过重新启动 ASP.NET Core、请求 http://localhost:5000/controllers/form、清除名称和价格字段并提交表单来查看新验证消息的效果。如图 29-7 所示，响应包括文本字段旁边的验证消息。验证摘要消息似乎是多余的，因为它重复了属性级消息。但摘要有一个有用的技巧，即能够显示适用于整个模型的消息，而不仅仅是单个属性。这意味着您可以报告由单个属性的组合引起的错误，否则很难用属性级消息来表达这些错误。在清单 29-12 中，我向 FormController.SubmitForm 操作添加了一个检查，当 Name 值以 Small 开头且 Price 值超过 100 时记录验证错误。如果用户输入以 Small 开头的名称值和大于 100 的价格值，则会记录模型级验证错误。只有当各个属性值没有验证问题时，我才会检查值的组合，这确保用户不会看到冲突的消息。与整个模型相关的验证错误使用带有空字符串作为第一个参数的 AddModelError 来记录。
清单 29-13 将 asp-validation-summary 属性的值更改为 ModelOnly，这排除了属性级错误，这意味着摘要将仅显示适用于整个模型的那些错误。重新启动 ASP.NET Core 并请求 http://localhost:5000/controllers/form。在名称字段中输入 Small Kayak，在价格字段中输入 150，然后提交表格。响应将包括模型级错误消息，如图 29-8 所示。 Razor 页面验证依赖于上一节中控制器中使用的相同功能。清单 29-14 向 FormHandler 页面添加了显式验证检查和错误摘要。 PageModel 类定义了一个 ModelState 属性，该属性等同于我在控制器中使用的属性并允许记录验证错误。验证过程是相同的，但在记录错误时必须小心，以确保名称与 Razor Pages 使用的模式相匹配。当我在控制器中记录错误时，我使用 nameof 关键字来选择与错误相关的属性，如下所示： 这是一个常见的约定，因为它可以确保错别字不会导致错误记录错误。此表达式在 Razor 页面中不起作用，其中必须针对 Product.Price 而不是 Price 记录错误，以反映 Razor 页面中的 @Model 表达式返回页面模型对象，如下所示：要测试验证过程，重启ASP.NET Core，使用浏览器请求http://localhost:5000/pages/form，提交空字段或无法转换成Product类要求的C#类型值的表单。错误消息的显示与控制器的错误消息相同，如图 29-9 所示。 （值 1、2 和 3 对 CategoryId 和 SupplierId 字段都有效。）