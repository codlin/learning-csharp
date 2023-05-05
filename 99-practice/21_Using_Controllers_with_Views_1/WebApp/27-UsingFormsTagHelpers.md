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