# 25. Using Tag Helpers

标签助手`Tag Helpers`是 C# 类，可以转换视图或页面中的 HTML 元素。标签助手的常见用途包括使用应用程序的路由配置为表单生成 URL，确保特定类型的元素样式一致，以及用常用的内容片段替换自定义速记元素。在本章中，我将描述标签助手的工作原理以及自定义标签助手的创建和应用方式。在第 26 章中，我描述了内置的标签助手，在第 27 章中，我使用标签助手来解释如何创建 HTML 表单。表 25-1 将标签助手放在上下文中。
Table 25-1. Putting Tag Helpers in Context
| Question | Answer |
|-|-|
| 标签助手是什么？| 标签助手是操纵 HTML 元素的类，以某种方式更改它们，用额外的内容补充它们，或者用新内容完全替换它们。 |
| 为什么它们有用？| 标签助手允许使用 C# 逻辑生成或转换视图内容，确保发送到客户端的 HTML 反映应用程序的状态。|
| 如何使用它们？  | 应用标签助手的 HTML 元素是根据类的名称或 HTMLTargetElement 属性来选择的。呈现视图时，元素由标签助手转换并包含在发送给客户端的 HTML 中。|
| 有什么坑？     | 使用标签助手可以很容易得意忘形并生成复杂的 HTML 内容部分，这使用视图组件更容易实现，如第 24 章所述。 |
| 有没有其他选择？| 您不必使用标签助手，但它们可以轻松地在 ASP.NET Core 应用程序中生成复杂的 HTML。|

## Creating a Tag Helper
理解标签助手的最佳方式是创建一个标签助手，它揭示了它们的操作方式以及它们如何适应 ASP.NET Core 应用程序。在接下来的部分中，我将完成创建和应用标签助手的过程，该标签助手将为 tr 元素设置 Bootstrap CSS 类，以便像这样的元素：
```html
...
<tr tr-color="primary">
    <th colspan="2">Product Summary</th>
</tr>
...
```
会被转换为：
```html
...
<tr class="bg-primary text-white text-center">
    <th colspan="2">Product Summary</th>
</tr>
...
```
标签助手将识别 tr-color 属性并使用它的值来设置发送到浏览器的元素的类属性。这不是最引人注目或最有用的转换，但它为解释标签助手的工作原理提供了基础。

### Defining the Tag Helper Class
标签助手可以在项目的任何地方定义，但将它们放在一起会有所帮助，因为它们需要在使用前注册。创建 WebApp/TagHelpers 文件夹并向其中添加一个名为 TrTagHelper.cs 的类文件，代码如清单 25-6 所示。
标签助手派生自 TagHelper 类，该类在 Microsoft.AspNetCore.Razor.TagHelpers 命名空间中定义。 TagHelper 类定义了一个 Process 方法，该方法被子类覆盖以实现转换元素的行为。标签助手的名称结合了它转换的元素的名称，后跟 TagHelper。在这个例子中，类名 TrTagHelper 表明这是一个对 tr 元素进行操作的标签助手。可以使用属性来扩大或缩小可以应用标签助手的元素范围，如本章后面所述，但默认行为由类名定义。
可以通过重写 ProcessAsync 方法而不是 Process 方法来创建异步标记帮助器，但这对于大多数帮助器来说不是必需的，它们往往会对 HTML 元素进行小而集中的更改。您可以在“高级标记帮助程序功能”部分中查看异步标记帮助程序的示例。

**Receiving Context Data**
标签助手通过 TagHelperContext 类的实例接收有关它们正在转换的元素的信息，该实例作为 Process 方法的参数接收，并定义表 25-3 中描述的属性。
Table 25-3. The TagHelperContext Properties
| Name | Description |
|-|-|
| AllAttributes | 此属性返回应用于正在转换的元素的属性的只读字典，按名称和索引索引。|
| Items | 此属性返回一个字典，用于在标签助手之间进行协调，如“标签助手之间的协调”部分所述。|
| UniqueId | 此属性返回正在转换的元素的唯一标识符。 |
虽然您可以通过 AllAttributes 字典访问元素属性的详细信息，但更方便的方法是定义一个属性，其名称对应于您感兴趣的属性，如下所示：
```cs
...
public string BgColor { get; set; } = "dark";
public string TextColor { get; set; } = "white";
...
```
When a tag helper is being used, the properties it defines are inspected and assigned the value of any
whose name matches attributes applied to the HTML element.
当使用标签助手时，将检查它定义的属性，并为其分配名称与应用于HTML元素的属性相匹配的任何属性的值。  
作为`Process`的一部分，属性值将被转换以匹配 C# 属性的类型，以便 bool 属性可用于接收 true 和 false 属性值，因此 int 属性可用于接收数字属性值，例如 1 和2. 对应的 HTML 元素属性中没有的 C# 类属性不会被设置，这意味着您应该检查以确保您没有处理 null 或提供默认值。    
属性的名称会自动从默认的 HTML 样式 bg-color 转换为 C# 样式 BgColor。您可以使用除 asp-（Microsoft 使用）和 data-（为发送到客户端的自定义属性保留）之外的任何属性前缀。示例标签助手将使用 bg-color 和 text-color 属性进行配置，这将为 BgColor 和 TextColor 属性提供值，并用于在 Process 方法中配置 tr 元素，如下所示：
```cs
...
output.Attributes.SetAttribute("class", $"bg-{BgColor} text-center text-{TextColor}");
...
```
将 HTML 属性名称用于标签助手属性并不总是会产生可读或可理解的类。您可以使用 HtmlAttributeName 属性断开属性名称和它表示的属性之间的链接，该属性可用于指定属性表示的 HTML 属性。  

**Producing Output**
Process 方法通过配置作为参数接收的 TagHelperOutput 对象来转换元素。 TagHelperOuput 对象首先描述出现在视图中的 HTML 元素，然后通过表 25-4 中描述的属性和方法进行修改。
Table 25-4. The TagHelperOutput Properties and Methods
*忽略*

在 TrTagHelper 类中，我使用 Attributes 字典向指定 Bootstrap 样式的 HTML 元素添加了一个类属性，包括 BgColor 和 TextColor 属性的值。效果是可以通过将 bg-color 和 text-color 属性设置为 Bootstrap 名称（例如 primary、info 和 danger）来指定 tr 元素的背景色。

### Registering Tag Helpers
**标签助手类必须在使用前使用 @addTagHelper 指令注册**。可以应用标签助手的视图或页面集取决于 @addTagHelper 指令的使用位置。对于单个视图或页面，该指令出现在 CSHTML 文件本身中。**为了使标记帮助程序更广泛地可用，可以将其添加到视图导入文件中，该文件在控制器的 Views 文件夹和 Razor Pages 的 Pages 文件夹中定义**。  
我希望我在本章中创建的标签助手在应用程序的任何地方都可用，这意味着 @addTagHelper 指令被添加到 Views 和 Pages 文件夹中的 _ViewImports.cshtml 文件中。第 24 章中用于应用视图组件的 vc 元素是一个标签助手，这就是为什么启用标签助手所需的指令已经在 _ViewImports.cshtml 文件中。
```cs
@addTagHelper *, WebApp
```
**参数的第一部分指定标签助手类的名称，支持通配符，第二部分指定定义它们的程序集的名称**。此 @addTagHelper 指令使用通配符来选择 WebApp 程序集中的所有命名空间，其效果是可以在任何控制器视图中使用项目中任何位置定义的标记助手。 Pages 文件夹中的 Razor Pages _ViewImports.cshtml 文件中有相同的语句。  
另一个 @addTagHelper 指令启用 Microsoft 提供的内置标签助手，这在第 26 章中有描述。
```cs
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

### Using a Tag Helper
最后一步是使用标签助手来转换元素。在清单 25-7 中，我将属性添加到 tr 元素，它将应用标签助手。
Listing 25-7. Using a Tag Helper in the Index.cshtml File in the Views/Home Folder
```html
<tr bg-color="info" text-color="white"></tr>
```
示例 25-7 中应用了属性的 tr 元素已经被转换，但这并不是图中显示的唯一变化。**默认情况下，标签助手适用于特定类型的所有元素，这意味着视图中的所有 tr 元素都已使用标签助手类中定义的默认值进行转换，因为没有定义任何属性**。 （有些表行没有显示文本的原因是因为 Bootstrap table-striped 类，它对交替行应用不同的样式。）  
实际上，问题更严重，因为视图导入文件中的 @addTagHelper 指令意味着example 标签助手应用于控制器和 Razor Pages 呈现的任何视图中使用的所有 tr 元素。例如，使用浏览器请求 http://localhost:5000/cities，您将看到来自 Cities Razor Page 的响应中的 tr 元素也发生了转换，如图 25-3 所示。

### Narrowing the Scope of a Tag Helper
可以使用 HtmlTargetElement 元素控制由标签助手转换的元素范围，如清单 25-8 所示。
Listing 25-8. Narrowing Scope in the TrTagHelper.cs File in the TagHelpers Folder
```cs
[HtmlTargetElement("tr", Attributes = "bg-color,text-color", ParentTag = "thead")]
public class TrTagHelper : TagHelper { ... }
```
Table 25-5. The HtmlTargetElement Properties
| Name | Description |
|-|-|
| Attributes | 此属性用于指定标签助手应仅应用于具有给定属性集的元素，以逗号分隔的列表形式提供。以星号结尾的属性名称将被视为前缀，因此 bg-* 将匹配 bg-color、bgsize 等。|
| ParentTag | 此属性用于指定标签助手应仅应用于给定类型的元素中包含的元素。|
| TagStructure | 此属性用于指定标签助手应仅应用于其标签结构对应于 TagStructure 枚举中给定值的元素，该枚举定义了 Unspecified、NormalOrSelfClosing 和 WithoutEndTag。|

### Widening the Scope of a Tag Helper
HtmlTargetElement 属性也可用于扩大标签助手的范围，使其匹配更广泛的元素。这是通过将属性的第一个参数设置为星号（* 字符）来完成的，它可以匹配任何元素。清单 25-9 更改了应用于示例标签助手的属性，以便它匹配任何具有 bg-color 和 text-color 属性的元素。
```cs
[HtmlTargetElement("*", Attributes = "bg-color,text-color")]
public class TrTagHelper : TagHelper { ... }
```
平衡 TagHelpers 文件夹中 TrTagHelper.cs 文件中的范围
```cs
[HtmlTargetElement("tr", Attributes = "bg-color,text-color")]
[HtmlTargetElement("td", Attributes = "bg-color")]
public class TrTagHelper : TagHelper { ... }
```
如果需要对一个元素应用多个标签助手，可以通过设置从 TagHelper 基类继承的 Order 属性来控制它们执行的顺序。管理序列可以帮助最小化标签助手之间的冲突，尽管仍然很容易遇到问题。

## Advanced Tag Helper Features
### Creating Shorthand Elements
标签助手不仅限于转换标准 HTML 元素，还可以用于用常用内容替换自定义元素。这可能是一个有用的特性，可以使视图更简洁并使它们的意图更明显。为了演示，清单 25-12 用自定义 HTML 元素替换了 Index 视图中的 thead 元素。
清单 25-12在 Views/Home 文件夹中的 Index.cshtml 文件中添加自定义 HTML 元素
```html
<tablehead bg-color="dark">Product Summary</tablehead>
```
tablehead 元素不是 HTML 规范的一部分，浏览器无法理解。相反，我将使用此元素作为生成 thead 元素及其 HTML 表格内容的简写。  
将名为 TableHeadTagHelper.cs 的类添加到 TagHelpers 文件夹，并使用它来定义如清单 25-13 所示的类。
提示： 在处理不属于 HTML 规范的自定义元素时，您必须应用 HtmlTargetElement 属性并指定元素名称，如清单 25-13 所示。基于类名将标签助手应用于元素的约定仅适用于标准元素名称。
Listing 25-13. The Contents of TableHeadTagHelper.cs in the TagHelpers Folder
**见文件**
此标签助手是异步的并重写 ProcessAsync 方法，以便它可以访问它转换的元素的现有内容。 ProcessAsync方法使用TagHelperOuput对象的属性生成一个完全不同的元素：TagName属性用于指定一个thead元素，TagMode属性用于指定该元素使用开始和结束标签编写，Attributes.SetAttribute方法用于定义类属性，Content属性用于设置元素内容。元素的现有内容是通过异步 GetChildContentAsync 方法获取的，该方法返回一个 TagHelperContent 对象。这是由 TagHelperOutput.Content 属性返回的同一对象，并允许使用相同类型通过表 25-6 中描述的方法检查和更改元素的内容。
Table 25-6. Useful TagHelperContent Methods
**略**

### Creating Elements Programmatically
当生成新的 HTML 元素时，您可以使用标准的 C# 字符串格式来创建您需要的内容，这是我在清单 25-13 中采用的方法。这行得通，但可能会很尴尬，需要密切注意以避免拼写错误。一种更可靠的方法是使用 TagBuilder 类，它在 Microsoft.AspNetCore.Mvc.Rendering 命名空间中定义，并允许以更结构化的方式创建元素。表 25-6 中描述的 TagHelperContent 方法接受 TagBuilder 对象，这使得在标签助手中创建 HTML 内容变得容易，如清单 25-14 所示。
Listing 25-14. Creating HTML Elements in the TableHeadTagHelper.cs File in the TagHelpers Folder
```cs
using Microsoft.AspNetCore.Mvc.Rendering;
...
public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
    ...
    TagBuilder header = new TagBuilder("th");
    header.Attributes["colspan"] = "2";
    header.InnerHtml.Append(content);

    TagBuilder row = new TagBuilder("tr");
    row.InnerHtml.AppendHtml(header);

    output.Content.SetHtmlContent(row);
    ...
}
...
```

### Prepending and Appending Content and Elements
TagHelperOutput 类提供了四个属性，可以轻松地将新内容注入视图，使其包围元素或元素的内容，如表 25-7 中所述。在接下来的部分中，我将解释如何在目标元素周围和内部插入内容。
Table 25-7. The TagHelperOutput Properties for Appending Context and Elements
| Name | Description |
|-|-|
| PreElement | 此属性用于在目标元素之前将元素插入到视图中。|
| PostElement | 此属性用于在目标元素之后将元素插入到视图中。|
| PreContent | 此属性用于在任何现有内容之前将内容插入到目标元素中。|
| PostContent | 此属性用于在任何现有内容之后将内容插入到目标元素中。|

**Inserting Content Around the Output Element**
第一个 TagHelperOuput 属性是 PreElement 和 PostElement，它们用于在输出元素之前和之后将元素插入到视图中。为了演示这些属性的使用，将名为 ContentWrapperTagHelper.cs 的类文件添加到 WebApp/TagHelpers 文件夹，其内容如清单 25-15 所示。
Listing 25-15. The Contents of the WrapperTagHelper.cs File in the TagHelpers Folder
**见文件**
此标记助手转换具有值为 true 的 wrap 属性的元素，它使用 PreElement 和 PostElement 属性在输出元素之前和之后添加 div 元素。清单 25-16 将一个元素添加到由标签助手转换的索引视图中。
Listing 25-16. Adding an Element in the Index.cshtml File in the Views/Home Folder
```html
<div class="m-2" wrap="true">Inner Content</div>
```

**Inserting Content Inside the Output Element**
PreContent 和 PostContent 属性用于在原始内容周围的输出元素内插入内容。要演示此功能，请将名为 HighlightTagHelper.cs 的类文件添加到 TagHelpers 文件夹，并使用它来定义清单 25-17 中所示的标签助手。
这个标签助手在输出元素的内容周围插入 b 和 i 元素。清单 25-18 将 wrap 属性添加到 Index 视图中的一个表格单元格。
```html
<tbody>
    <tr><th>Name</th><td highlight="true">@Model?.Name</td></tr>
    ...
</tbody>
```

### Getting View Context Data
标签助手的一个常见用途是转换元素，使它们包含当前请求或视图模型/页面模型的详细信息，这需要访问上下文数据。要创建这种类型的标签助手，请将名为 RouteDataTagHelper.cs 的文件添加到 TagHelpers 文件夹，其内容如清单 25-19 所示。
Listing 25-19. The Contents of the RouteDataTagHelper.cs File in the TagHelpers Folder
**见文件**
标签助手转换具有值为 true 的 route-data 属性的 div 元素，并使用路由系统获得的段变量列表填充输出元素。为了获取路由数据，我添加了一个名为 Context 的属性，并用两个属性对其进行修饰，如下所示：
```cs
...
[ViewContext]
[HtmlAttributeNotBound]
public ViewContext Context { get; set; } = new();
...
```
`ViewContext` 属性表示**当创建标签助手类的新实例时，应该为该属性的值分配一个 ViewContext 对象，它提供正在呈现的视图的详细信息，包括路由数据**，如第 13 章所述。  
**如果在 div 元素上定义了匹配的属性，则 HtmlAttributeNotBound 属性会阻止将值分配给此属性。这是一个很好的做法**，尤其是当您正在编写供其他开发人员使用的标签助手时。
清单 25-20 向 Home 控制器的 Index 视图添加了一个元素，该元素将被新的标签助手转换。
Listing 25-20. Adding an Element in the Index.cshtml File in the Views/Home Folder
```html
<div route-data="true"></div>
```

### Working with Model Expressions
标签助手可以对视图模型进行操作，定制它们执行的转换或它们创建的输出。要查看此功能的工作原理，请将名为 ModelRowTagHelper.cs 的类文件添加到 TagHelpers 文件夹。
这个标签助手转换具有 for 属性的 tr 元素。这个标签助手的重要部分是 For 属性的类型，它用于接收 for 属性的值。
```cs
...
public ModelExpression? For { get; set; }
...
```
当您想对视图模型的一部分进行操作时，可以使用 ModelExpression 类，这可以通过向前跳转并显示如何在视图中应用标签助手来轻松解释，如清单 25-22 所示。
注意：ModelExpression 功能只能用于视图模型或页面模型。它不能用于在视图中创建的变量，例如 @foreach 表达式。
Listing 25-22. Using the Tag Helper in the Index.cshtml File in the Views/Home Folder
```html
<tr for="Name" />
<tr for="Price" format="c" />
<tr for="CategoryId" />
```
`for` 属性的值是**由视图模型类定义的属性的名称**。创建标签助手时，会检测 For 属性的类型，并为其分配一个描述所选属性的 ModelExpression 对象。  
我不打算详细描述 ModelExpression 类，因为对类型的任何内省都会导致无穷无尽的类和属性列表。此外，ASP.NET Core 提供了一组有用的内置标签助手，它们使用视图模型来转换元素，如第 26 章所述，这意味着您无需创建自己的标签助手。  
对于示例标签助手，我使用了三个值得描述的基本功能。第一个是获取模型属性的名称，以便我可以将其包含在输出元素中，如下所示：   
```
...
th.InnerHtml.Append(For?.Name ?? String.Empty);
...
```
Name 属性返回模型属性的名称。第二个功能是获取模型属性的类型，以便我可以确定是否格式化该值，如下所示： 
```cs
...
if (Format != null && For?.Metadata.ModelType == typeof(decimal)) {
...
```
第三个功能是获取属性的值，以便它可以包含在响应中。
```cs
...
td.InnerHtml.Append(For?.Model.ToString() ?? String.Empty);
...
```

**Working with the Page Model**
可以在 Razor Pages 中应用带有模型表达式的标记帮助程序，尽管选择属性的表达式必须说明 Model 属性返回页面模型类的方式。清单 25-23 将标签助手应用于 Editor Razor 页面，其页面模型定义了一个 Product 属性。
Listing 25-23. Applying a Tag Helper in the Editor.cshtml File in the Pages Folder
```html
 <tbody>
    <tr for="Product.Name" />
    <tr for="Product.Price" format="c" />
</tbody>
``
for 属性的值通过 Product 属性选择嵌套属性，该属性为标签助手提供它需要的 ModelExpression。模型表达式不能与 null 条件运算符一起使用，这给本示例带来了问题，因为 Product 属性的类型是 Product?。清单 25-24 将属性类型更改为 Product 并分配了一个默认值。 （我在第 27 章展示了解决这个问题的不同方法。）
Listing 25-24. Changing a Property Type in the Editor.cshtml.cs File in the Pages Folder
```cs
public class EditorModel : PageModel {
    ...
    public Product Product { get; set; } = new();
    public async Task OnGetAsync(long id) {
        Product = await context.Products.FindAsync(id) ?? new();
    }
}
```
页面模型的一个结果是 ModelExpression.Name 属性将返回 Product.Name，例如，而不仅仅是名称。清单 25-25 更新了标签助手，使其只显示模型表达式名称的最后一部分。
Listing 25-25. Processing Names in the ModelRowTagHelper.cs File in the TagHelpers Folder
```cs
th.InnerHtml.Append(For?.Name.Split(".").Last() ?? String.Empty);
```

### Coordinating Between Tag Helpers
TagHelperContext.Items 属性提供了一个字典，供对元素及其后代进行操作的标签助手使用。为了演示 Items 集合的使用，将一个名为 CoordinatingTagHelpers.cs 的类文件添加到 WebApp/TagHelpers 文件夹。
第一个标签助手对具有 theme 属性的 tr 元素进行操作。协调标签助手`Coordinating tag helpers`可以转换它们自己的元素，但此示例只是将 theme 属性的值添加到 Items 字典，以便标签助手可以对 tr 元素中包含的元素进行操作。第二个标签助手对 th 和 td 元素进行操作，并使用 Items 字典中的主题值为其输出元素设置 Bootstrap 样式。  
清单 25-27 将元素添加到 Home 控制器的 Index 视图中，这些元素应用了协调标签助手。  
请注意，我添加了在清单 25-27 中转换的 th 和 td 元素，而不是依赖标签助手来生成它们。标签助手不应用于由其他标签助手生成的元素，并且仅影响视图中定义的元素。
Listing 25-27. Applying a Tag Helper in the Index.cshtml File in the Views/Home Folder
```html
<tbody>
    <tr theme="primary">
        <th>Name</th><td>@Model?.Name</td>
    </tr>
    <tr theme="secondary">
        <th>Price</th><td>@Model?.Price.ToString("c")</td>
    </tr>
    <tr theme="info">
        <th>Category</th><td>@Model?.CategoryId</td>
    </tr>
</tbody>
```

### Suppressing the Output Element
通过在作为 Process 方法的参数接收的 TagHelperOutput 对象上调用 SuppressOuput 方法，可以使用标记助手来防止元素包含在 HTML 响应中。在清单 25-28 中，我向 Home 控制器的 Index 视图添加了一个元素，只有当视图模型的 Price 属性超过指定值时才应显示该元素。
Listing 25-28. Adding an Element in the Index.cshtml File in the Views/Home Folder
```html
<div show-when-gt="500" for="Price">
    <h5 class="bg-danger text-white text-center p-2">
        Warning: Expensive Item
    </h5>
</div>
```
show-when-gt 属性指定 div 元素应该显示的值，for 属性选择将被检查的模型属性。要创建将管理元素（包括响应）的标签助手，请将名为 SelectiveTagHelper.cs 的类文件添加到 WebApp/TagHelpers 文件夹，代码如清单 25-29 所示。
标签助手使用模型表达式访问属性并调用 SuppressOutput 方法，除非超过阈值。

## Using Tag Helper Components
标签助手组件提供了一种将标签助手作为服务应用的替代方法。当您需要设置标记帮助程序以支持另一个服务或中间件组件时，此功能可能很有用，这通常是同时具有客户端组件和服务器端组件的诊断工具或功能的情况，例如 Blazor，它是在第 4 部分中进行了描述。在接下来的部分中，我将向您展示如何创建和应用标签助手组件。

### Creating a Tag Helper Component
Tag helper 组件派生自 TagHelperComponent 类，该类提供与前面示例中使用的 TagHelper 基类类似的 API。要创建标签助手组件，请在 TagHelpers 文件夹中添加一个名为 TimeTagHelperComponent.cs 的类文件。
标签助手组件不指定它们转换的元素，并且为已配置标签助手组件功能的每个元素调用 Process 方法。默认情况下，标签助手组件用于转换 head 和 body 元素。这意味着标签助手组件类必须检查输出元素的 TagName 属性，以确保它们只执行预期的转换。TimeTagHelperComponent.cs 中的标签助手组件查找 body 元素并使用 PreContent 属性在元素的其余内容之前插入一个包含时间戳的 div 元素。
标签助手组件被注册为实现 ITagHelperComponent 接口的服务，如清单 25-31 所示。
Listing 25-31. Registering a Tag Helper Component in the Program.cs File in the WebApp Folder
```cs
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebApp.TagHelpers;
...
builder.Services.AddTransient<ITagHelperComponent, TimeTagHelperComponent>();
```

### Expanding Tag Helper Component Element Selection
默认情况下，只有 head 和 body 元素由标签助手组件处理，但可以通过创建一个从可怕命名的 TagHelperComponentTagHelper 类派生的类来选择其他元素。将名为 TableFooterTagHelperComponent.cs 的类文件添加到 TagHelpers 文件夹。
TableFooterSelector 类派生自 TagHelperComponentTagHelper，并装饰有 HtmlTargetElement 属性，该属性扩展了应用程序的标签助手组件处理的元素范围。在这种情况下，属性选择表元素。  
在同一个文件中定义的 TableFooterTagHelperComponent 类是一个标签帮助器组件，它通过添加一个代表表格页脚的 tfoot 元素来转换表格元素。  
**注意**：请记住，当您创建一个新的 TagHelperComponentTagHelper 时，所有标签助手组件都将接收由 HtmlTargetAttribute 元素选择的元素。标签助手组件必须注册为服务才能接收用于转换的元素，但标签助手组件标签助手（这是我多年来见过的最糟糕的命名选择之一）是自动发现和应用的。清单 25-33 添加了标签助手组件服务。
Listing 25-33. Registering a Tag Helper Component in the Program.cs File in the WebApp Folder
```cs
builder.Services.AddTransient<ITagHelperComponent, TableFooterTagHelperComponent>();
```
