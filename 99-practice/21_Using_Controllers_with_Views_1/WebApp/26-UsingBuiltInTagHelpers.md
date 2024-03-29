# Using the Built-in Tag Helpers

ASP.NET Core 提供了一组内置的标签助手，用于应用最常需要的元素转换。在本章中，我将解释那些处理锚点、脚本、链接和图像元素的标签助手，以及缓存内容和根据环境选择内容的功能。在第 27 章中，我描述了支持 HTML 表单的标签助手。表 26-1 将内置的标签助手放在上下文中。
| Question | Answer |
|-|-|
| 它们是什么？     | 内置的标签助手对 HTML 元素执行通常需要的转换。                |
| 为什么它们有用？  | 使用内置的标签助手意味着你不必使用第 25 章中的技术创建自定义助手。|
| 如何用它们？     | 使用标准 HTML 元素上的属性或通过自定义 HTML 元素应用标签助手。  |
| 有那么坑或者限制？| 没有，这些标签助手经过充分测试且易于使用。除非您有特殊需求，否则使用这些标签助手比自定义实现更可取。|
| 有没有其他选择？  | 这些标签助手是可选的，它们的使用不是必需的。|

## Enabling the Built-in Tag Helpers
内置标签助手全部在 Microsoft.AspNetCore.Mvc.TagHelpers 命名空间中定义，并通过将 @addTagHelpers 指令添加到各个视图或页面来启用，或者，如在示例项目的情况下，查看导入文件。这是 Views/Pages 文件夹中 _ViewImports.cshtml 文件中的必需指令，它为控制器视图启用内置标签助手： 
```cs
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
这些指令已添加到第 24 章的示例项目中以启用视图组件功能。

## Transforming Anchor Elements
a 元素是在应用程序中导航和向应用程序发送 GET 请求的基本工具。 AnchorTagHelper 类用于转换元素的 href 属性，以便它们以使用路由系统生成的 URL 为目标，这意味着不需要硬编码 URL，路由配置的更改将自动反映在应用程序的锚元素中。表 26-3 描述了 AnchorTagHelper 类支持的属性。
Table 26-3. The Built-in Tag Helper Attributes for Anchor Elements
| Name | Description |
|-|-|
| asp-action | This attribute specifies the action method that the URL will target.|
| asp-controller | This attribute specifies the controller that the URL will target. If this attribute is omitted, then the URL will target the controller or page that rendered the current view.|
| asp-page | This attribute specifies the Razor Page that the URL will target.|
| asp-page-handler | This attribute specifies the Razor Page handler function that will process the request, as described in Chapter 23.|
| asp-fragment | This attribute is used to specify the URL fragment (which appears after the # character).|
| asp-host | This attribute specifies the name of the host that the URL will target.|
| asp-protocol | This attribute specifies the protocol that the URL will use.|
| asp-route | This attribute specifies the name of the route that will be used to generate the URL.|
| asp-route-* | Attributes whose name begins with asp-route- are used to specify additional values for the URL so that the asp-route-id attribute is used to provide a value for the id segment to the routing system.|
| asp-all-route-data | This attribute provides values used for routing as a single value, rather than using individual attributes.|

AnchorTagHelper 简单且可预测，可以轻松地在使用应用程序路由配置的元素中生成 URL。清单 26-7 添加了一个锚点元素，该元素使用表中的属性创建一个 URL，该 URL 以 Home 控制器定义的另一个操作为目标。
Listing 26-7. Transforming an Element in the _RowPartial.cshtml File in the Views/Home Folder
```html
<td>
    <a asp-action="index" asp-controller="home" asp-route-id="@Model?.ProductId"
        class="btn btn-sm btn-info text-white">
        Select
    </a>
</td>
```
asp-action 和 asp-controller 属性指定操作方法的名称和定义它的控制器。段变量的值是使用 asp-route-[name] 属性定义的，这样 asp-route-id 属性为 id 段变量提供一个值，该变量用于为 asp-action 选择的操作方法提供参数属性。
如果您检查 Select 锚元素，您会看到每个 href 属性都包含与其相关的 Product 对象的 ProductId 值，如下所示：
```html
...
<a class="btn btn-sm btn-info text-white" href="/Home/index/3">Select</a>
...
```
在这种情况下，asp-route-id 属性提供的值意味着无法使用默认 URL，因此路由系统生成了一个 URL，其中包含控制器和操作名称的段，以及将要使用的段为操作方法提供参数。在这两种情况下，由于只指定了一个操作方法，标签助手创建的 URL 以呈现视图的控制器为目标。单击锚点元素将发送一个以 Home 控制器的 Index 方法为目标的 HTTP GET 请求。

### Using Anchor Elements for Razor Pages
asp-page 属性用于将 Razor 页面指定为锚元素的 href 属性的目标。页面路径以 / 字符为前缀，@page 指令定义的路由段的值使用 asp-route-[name] 属性定义。清单 26-8 添加了一个定位元素，该元素以 Pages/Suppliers 文件夹中定义的列表页面为目标。

**GENERATING URLS (AND NOT LINKS)**
标签助手仅在锚元素中生成 URL。如果您需要生成 URL 而不是链接，则可以使用 Url 属性，该属性在控制器、页面模型和视图中可用。该属性返回一个实现 IUrlHelper 接口的对象，该接口提供一组生成 URL 的方法和扩展方法。下面是一个在视图中生成 URL 的 Razor 片段： 
```html
...
<div>@Url.Page("/suppliers/list")</div>
...
```
该片段生成一个 div 元素，其内容是指向 /Suppliers/List Razor Page 的 URL。控制器或页面模型类中使用相同的接口，例如以下语句：
```cs
...
string url = Url.Action("List", "Home");
...
```
该语句生成一个 URL，该 URL 以 Home 控制器上的 List 操作为目标，并将其分配给名为 url 的字符串变量。

## Using the JavaScript and CSS Tag Helpers
ASP.NET Core 提供了标签助手，用于通过脚本和链接元素管理 JavaScript 文件和 CSS 样式表。正如您将在接下来的部分中看到的那样，这些标签助手功能强大且灵活，但需要密切注意以避免产生意外结果。 

### Managing JavaScript Files
ScriptTagHelper 类是脚本元素的内置标记助手，用于使用表 26-4 中描述的属性管理 JavaScript 文件在视图中的包含，我将在接下来的部分中对其进行描述。 
Table 26-4. The Built-in Tag Helper Attributes for script Elements
| Name | Description |
|-|-|
| asp-src-include | 此属性用于指定将包含在视图中的 JavaScript 文件。|
| asp-src-exclude | 此属性用于指定将从视图中排除的 JavaScript 文件。|
| asp-append-version | 该属性用于缓存清除，如“了解缓存清除”边栏中所述。|
| asp-fallback-src | 此属性用于指定在内容分发网络出现问题时使用的回退 JavaScript 文件。|
| asp-fallback-src-include | 此属性用于选择在存在内容分发网络问题时将使用的 JavaScript 文件。|
| asp-fallback-src-exclude | 此属性用于排除 JavaScript 文件以在存在内容分发网络问题时显示它们的用途。|
| asp-fallback-test | 此属性用于指定将用于确定 JavaScript 代码是否已从内容分发网络正确加载的 JavaScript 片段。|

#### **Selecting JavaScript Files**
asp-src-include 属性用于使用通配模式将 JavaScript 文件包含在视图中。通配模式支持一组用于匹配文件的通配符，表 26-5 描述了最常见的通配模式。
Table 26-5. Common Globbing Patterns
**忽略**
Globbing 是一种确保视图包含应用程序所需的 JavaScript 文件的有用方法，即使文件的确切路径发生变化，这通常发生在文件名中包含版本号或包添加其他文件时。  
清单 26-9 使用 asp-src-include 属性包含 wwwroot/lib/jquery 文件夹中的所有 JavaScript 文件，这是使用清单 26-4 中的命令安装的 jQuery 包的位置。
```html
<head>
    ...
    <script asp-src-include="lib/jquery/**/*.js"></script>
</head>
```
重新启动 ASP.NET Core 和浏览器以请求 http://localhost:5000/home/list 并检查发送到浏览器的 HTML。您将看到布局中的单个脚本元素已转换为每个 JavaScript 文件的脚本元素，如下所示：
```html
<head>
    ...
    <script src="/lib/jquery/jquery.js"></script>
    <script src="/lib/jquery/jquery.min.js"></script>
    <script src="/lib/jquery/jquery.slim.js"></script>
    <script src="/lib/jquery/jquery.slim.min.js"></script>
</head>
```
**UNDERSTANDING SOURCE MAPS**
JavaScript 文件被缩小以使其更小，这意味着它们可以更快地交付给客户端并使用更少的带宽。缩小过程从文件中删除所有空格并重命名函数和变量，以便有意义的名称（例如 myHelpfullyNamedFunction）将由较少数量的字符表示，例如 x1。当使用浏览器的 JavaScript 调试器来跟踪缩小代码中的问题时，像 x1 这样的名称几乎不可能跟踪代码的进度。  
具有 map 文件扩展名的文件是源映射，浏览器通过在缩小代码和开发人员可读的未缩小源文件之间提供映射来帮助调试缩小代码。当您打开浏览器的 F12 开发者工具时，浏览器会自动请求源映射并使用它们来帮助调试应用程序的客户端代码。

#### **Narrowing the Globbing Pattern**
没有应用程序需要清单 26-9 中的模式选择的所有文件。许多包包括多个包含相似内容的 JavaScript 文件，通常会删除不太受欢迎的功能以节省带宽。 jQuery 包中包含 jquery.slim.js 文件，该文件包含与 jquery.js 文件相同的代码，但没有处理异步 HTTP 请求和动画效果的功能。  
这些文件中的每一个都有一个带有 min.js 文件扩展名的对应文件，它表示一个缩小的文件。缩小通过删除所有空格并重命名函数和变量以使用更短的名称来减小 JavaScript 文件的大小。  
每个包只需要一个 JavaScript 文件，如果您只需要缩小版本（大多数项目都是这种情况），那么您可以限制 globbing 模式匹配的文件集，如清单 26-10 所示。
```html
<head>
    ...
    <script asp-src-include="lib/jquery**/*slim.min.js"></script>
</head>
```
重新启动 ASP.NET Core 和浏览器以再次请求 http://localhost:5000/home/list 并检查应用程序发送的 HTML。您将看到只选择了缩小的文件。
```html
<head>
    ...
    <script src="/lib/jquery/jquery.slim.min.js"></script>
</head>
```

**Excluding Files**
当您要选择名称包含特定术语（例如 slim）的文件时，缩小 JavaScript 文件的模式会有所帮助。当您想要的文件没有该术语时，例如当您想要缩小文件的完整版本时，它没有帮助。幸运的是，您可以使用 asp-src-exclude 属性从与 asp-src-include 属性匹配的列表中删除文件，如清单 26-12 所示。
```html
<head>
    ...
    <script asp-src-include="/lib/jquery/**/*.min.js" asp-src-exclude="**.slim.**"></script>
</head>
```
重新启动 ASP.NET Core 和浏览器以再次请求 http://localhost:5000/home/list 并检查应用程序发送的 HTML。您将看到只选择了缩小的文件。
```html
<head>
    ...
    <script src="/lib/jquery/jquery.min.js"></script>
</head>
```

**UNDERSTANDING CACHE BUSTING**
静态内容（例如图像、CSS 样式表和 JavaScript 文件）通常被缓存以阻止对很少更改的内容的请求到达应用程序服务器。缓存可以通过不同的方式完成：服务器可以告诉浏览器缓存内容，应用程序可以使用缓存服务器来补充应用程序服务器，或者可以使用内容分发网络来分发内容。并非所有缓存都在您的控制之下。例如，大公司通常会安装缓存以减少带宽需求，因为很大一部分请求往往会转到相同的站点或应用程序。  
缓存的一个问题是，当您部署它们时，客户端不会立即收到新版本的静态文件，因为它们的请求仍在由以前缓存的内容提供服务。最终，缓存的内容将过期，新的内容将被使用，但这会留下一段时期，应用程序控制器生成的动态内容与缓存交付的静态内容不同步。这可能会导致布局问题或意外的应用程序行为，具体取决于已更新的内容。  
解决此问题称为缓存破坏。这个想法是允许缓存处理静态内容，但立即反映在服务器上所做的任何更改。标签助手类通过向静态内容的 URL 添加查询字符串来支持缓存清除，其中包括充当版本号的校验和。例如，对于 JavaScript 文件，ScriptTagHelper 类通过 asp-append-version 属性支持缓存无效化，如下所示：
```html
...
<script asp-src-include="/lib/jquery/**/*.min.js" asp-src-exclude="**.slim.**" asp-append-version="true">
</script>
...
```
启用缓存清除功能会在发送到浏览器的 HTML 中生成如下元素：
```html
...
<script src="/lib/jquery/jquery.min.js?v=_xUj-3OJU5yExlq6GSYGSHk7tPXikyn"></script>
...
```
标签助手将使用相同的版本号，直到您更改文件的内容，例如通过更新 JavaScript 库，此时将计算不同的校验和。版本号的加入意味着每次更改文件时，客户端都会请求不同的URL，缓存将其视为请求新的内容无法满足之前缓存的内容，并传递给应用服务器。然后内容会正常缓存直到下一次更新，这会产生另一个具有不同版本的 URL。

#### **Working with Content Delivery Networks**
内容分发网络 (CDN) 用于将对应用程序内容的请求卸载`offload`到离用户较近的服务器。浏览器不是从您的服务器请求 JavaScript 文件，而是从解析为地理上本地服务器的主机名请求它，这减少了加载文件所需的时间，并减少了您必须为应用程序提供的带宽量。如果您有大量分散在不同地域的用户，那么注册 CDN 可能具有商业意义，但即使是最小和最简单的应用程序也可以从使用主要技术公司运营的免费 CDN 来交付通用 JavaScript 包中受益，比如jQuery。  
对于本章，我将使用 CDNJS，它与库管理器工具用于在 ASP.NET Core 项目中安装客户端包的 CDN 相同。可以在 https://cdnjs.com 搜索包；对于清单 26-4 中安装的包和版本 jQuery 3.6.0，有六个 CDNJS URL。  
```shell
https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.js
https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js
https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.map
https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.slim.js
https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.slim.min.js
https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.slim.min.map
```
这些 URL 为 jQuery 的完整版和精简版提供常规 JavaScript 文件、缩小的 JavaScript 文件和缩小文件的源映射。   
CDN 的问题在于它们不受您组织的控制，这意味着它们可能会失败，使您的应用程序保持运行但无法按预期工作，因为 CDN 内容不可用。 ScriptTagHelper 类提供了在客户端无法加载 CDN 内容时回退到本地文件的能力，如清单 26-13 所示。   
Listing 26-13. Using CDN Fallback in the _SimpleLayout.cshtml File in the Views/Shared Folder
```html
...
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"
        asp-fallback-src="/lib/jquery/jquery.min.js" asp-fallback-test="window.jQuery">
</script>
...
```
src 属性用于指定 CDN URL。 asp-fallback-src 属性用于指定一个本地文件，如果 CDN 无法传送由常规 src 属性指定的文件，将使用该文件。为了确定 CDN 是否正常工作，asp-fallback-test 属性用于定义将在浏览器中评估的 JavaScript 片段。如果片段评估为 false，则将请求回退文件。   
**提示** asp-fallback-src-include 和 asp-fallback-src-exclude 属性可用于选择具有通配模式的本地文件。但是，鉴于 CDN 脚本元素选择单个文件，我建议使用 asp-fallback-src 属性选择相应的本地文件，如示例所示。
使用浏览器请求 http://localhost:5000/home/list，你会看到 HTML 响应包含两个脚本元素，如下所示：
```html
...
<head>
    ...
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script>
        (window.jQuery||document.write("\u003Cscript src=\u0022/lib/jquery/jquery.min.js\u0022\u003E\u003C/script\u003E"));
    </script>
</head>
...
```
第一个脚本元素从 CDN 请求 JavaScript 文件。第二个脚本元素评估由 asp-fallback-test 属性指定的 JavaScript 片段，它检查第一个脚本元素是否有效。如果该片段评估为真，则不会采取任何操作，因为 CDN 正常工作。如果该片段的计算结果为 false，则会向 HTML 文档添加一个新的脚本元素，指示浏览器从回退 URL 加载 JavaScript 文件。  
测试回退设置很重要，因为在 CDN 停止工作并且您的用户无法访问您的应用程序之前，您不会发现它们是否失败。检查回退的最简单方法是将 src 属性指定的文件的名称更改为您知道不存在的名称（我在文件名后附加了 FAIL 一词），然后查看浏览器的网络请求使用 F12 开发者工具。您应该会看到 CDN 文件的错误，然后是对回退文件的请求。     
**注意** CDN回退功能依赖于浏览器同步加载和执行脚本元素的内容，并按照它们的定义顺序。有许多技术可以通过使进程异步来加速 JavaScript 的加载和执行，但这些技术可能导致在浏览器从 CDN 检索文件并执行其内容之前执行回退测试，从而导致请求即使 CDN 运行良好并且首先击败了 CDN 的使用，回退文件也是如此。**不要将异步脚本加载与 CDN 回退功能混合使用**。

### Managing CSS Stylesheets
LinkTagHelper 类是链接元素的内置标签助手，用于管理视图中 CSS 样式表的包含。这个标签助手支持表 26-6 中描述的属性，我将在以下部分中对其进行演示。
Table 26-6. The Built-in Tag Helper Attributes for link Elements
| Name | Description |
|-|-|
| asp-href-include | 此属性用于为输出元素的 href 属性选择文件。|
| asp-href-exclude | 此属性用于从输出元素的 href 属性中排除文件。|
| asp-append-version | 此属性用于启用缓存清除，如“了解缓存清除”边栏中所述。|
| asp-fallback-href | 如果 CDN 出现问题，此属性用于指定回退文件。|
| asp-fallback-hrefinclude | 此属性用于选择在出现 CDN 问题时将使用的文件。|
| asp-fallback-hrefexclude | 此属性用于从出现 CDN 问题时将使用的集合中排除文件。|
| asp-fallback-hreftest-class | 此属性用于指定将用于测试 CDN 的 CSS 类。|
| asp-fallback-hreftest-property | 此属性用于指定将用于测试 CDN 的 CSS 属性。|
| asp-fallback-hreftest-value | 此属性用于指定将用于测试 CDN 的 CSS 值。|

#### **Selecting Stylesheets**
LinkTagHelper 与 ScriptTagHelper 共享许多功能，包括支持通配模式以选择或排除 CSS 文件，因此不必单独指定它们。能够准确地选择 CSS 文件与选择 JavaScript 文件一样重要，因为样式表可以有常规版本和缩小版本，并且支持源映射。流行的 Bootstrap 包，我在本书中一直使用它来设置 HTML 元素的样式，它在 wwwroot/lib/bootstrap/css 文件夹中包含它的 CSS 样式表。
Listing 26-14. Selecting a Stylesheet in the _SimpleLayout.cshtml File in the Views/Shared Folder
```html
<head>
    ....
    <link asp-href-include="/lib/bootstrap/css/*.min.css"
        asp-href-exclude="**/*-reboot*,**/*-grid*,**/*-utilities*, **/*.rtl.*"
        rel="stylesheet" />
</head>
```
选择 JavaScript 文件时需要同样注意细节，因为很容易为同一文件的多个版本或您不想要的文件生成链接元素。

#### **Working with Content Delivery Networks**
LinkTag 帮助程序类提供了一组属性，用于在 CDN 不可用时回退到本地内容，尽管测试样式表是否已加载的过程比测试 JavaScript 文件更复杂。清单 26-15 使用 CDNJS URL 作为 Bootstrap CSS 样式表。   
```html
<link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.1.3/css/bootstrap.min.css"
        asp-fallback-href="/lib/bootstrap/css/bootstrap.min.css" asp-fallback-test-class="btn"
        asp-fallback-test-property="display" asp-fallback-test-value="inline-block" rel="stylesheet" />
```
href 属性用于指定 CDN URL，我已经使用 asp-fallback-href 属性来选择在 CDN 不可用时将使用的文件。然而，测试 CDN 是否有效需要使用三个不同的属性，并了解由正在使用的 CSS 样式表定义的 CSS 类。  
使用浏览器请求 http://localhost:5000/home/list 并检查响应中的 HTML 元素。您会看到布局中的链接元素已转换为三个独立的元素，如下所示：
```html
<link href="https://cdnjs.cloudflare.com/.../bootstrap.min.css" rel="stylesheet"/>
<meta name="x-stylesheet-fallback-test" content="" class="btn" />
<script>
    !function(a,b,c,d){var e,f=document,
    g=f.getElementsByTagName("SCRIPT"),
    h=g[g.length1].previousElementSibling,
    i=f.defaultView&&f.defaultView.getComputedStyle ?
    f.defaultView.getComputedStyle(h) : h.currentStyle;
    if(i&&i[a]!==b)for(e=0;e<c.length;e++)
    f.write('<link href="'+c[e]+'" '+d+"/>")}("display","inline-block",
    ["/lib/bootstrap/css/bootstrap.min.css"],
    "rel=\u0022stylesheet\u0022 ");
</script>
``` 
为了使转换更容易理解，我格式化了 JavaScript 代码并缩短了 URL。  
第一个元素是一个常规链接，其 href 属性指定 CDN 文件。第二个元素是 meta 元素，它指定视图中 asp-fallback-test-class 属性的类。我在清单中指定了 btn 类，这意味着将像这样的元素添加到发送到浏览器的 HTML 中：  
```html
<meta name="x-stylesheet-fallback-test" content="" class="btn">
```
您指定的 CSS 类必须在将从 CDN 加载的样式表中定义。我指定的 btn 类提供了 Bootstrap 按钮元素的基本格式。    
asp-fallback-test-property 属性用于指定将 CSS 类应用于元素时设置的 CSS 属性，而 asp-fallback-test-value 属性用于指定将设置的值。   
tag helper 创建的 script 元素包含 JavaScript 代码，该代码将元素添加到指定的类中，然后测试 CSS 属性的值以确定 CDN 样式表是否已加载。如果不是，则为回退文件创建链接元素。 Bootstrap btn 类将 display 属性设置为 inline-block，这提供了查看浏览器是否能够从 CDN 加载 Bootstrap 样式表的测试。

## Working with Image Elements
ImageTagHelper 类用于通过 img 元素的 src 属性为图像提供缓存清除，允许应用程序利用缓存，同时确保立即反映对图像的修改。 ImageTagHelper 类在定义 asp-appendversion 属性的 img 元素中运行，该属性在表 26-7 中进行了描述以供快速参考。
Table 26-7. The Built-in Tag Helper Attribute for Image Elements
| Name | Description |
|-|-|
| asp-append-version | 此属性用于启用缓存清除，如“了解缓存清除”边栏中所述。|

Listing 26-16. Adding an Image in the _SimpleLayout.cshtml File in the Views/Shared Folder
```html
<div class="m-2">
    <img src="/images/city.png" asp-append-version="true" class="m-2" />
    @RenderBody()
</div>
```
检查 HTML 响应，您会看到用于请求图像文件的 URL 包含版本校验和，如下所示：
```html
...
<img src="/images/city.png?v=KaMNDSZFAJufRcRDpKh0K_IIPNc7E" class="m-2">
...
```
添加校验和可确保对文件的任何更改都将通过任何缓存，避免陈旧的内容。

## Using the Data Cache
CacheTagHelper 类允许缓存内容片段以加速视图或页面的呈现。要缓存的内容使用 cache 元素表示，该元素使用表 26-8 中所示的属性进行配置。
缓存是重用部分内容的有用工具，因此不必为每个请求生成它们。但是有效地使用缓存需要仔细的思考和计划。虽然缓存可以提高应用程序的性能，但它也会产生奇怪的效果，例如用户收到陈旧的内容、包含不同版本内容的多个缓存以及由于从应用程序的先前版本缓存的内容混合而导致的更新部署被破坏包含新版本的内容。**除非您有明确定义的性能问题需要解决，否则不要启用缓存，并确保您了解缓存将产生的影响**。  
Table 26-8. The Built-in Tag Helper Attributes for cache Elements
| Name | Description |
|-|-|
| enabled | 这个bool属性用来控制缓存元素的内容是否被缓存。省略此属性将启用缓存。|
| expires-on | 此属性用于指定缓存内容过期的绝对时间，以 DateTime 值表示。|
| expires-after | 此属性用于指定缓存内容过期的相对时间，以 TimeSpan 值表示。|
| expires-sliding | 此属性用于指定自上次使用以来缓存内容将过期的时间，以 TimeSpan 值表示。|
| vary-by-header | 此属性用于指定将用于管理缓存内容的不同版本的请求标头的名称。|
| vary-by-query | 此属性用于指定将用于管理缓存内容的不同版本的查询字符串键的名称。|
| vary-by-route | 此属性用于指定将用于管理缓存内容的不同版本的路由变量的名称。|
| vary-by-cookie | 此属性用于指定将用于管理缓存内容的不同版本的 cookie 的名称。|
| vary-by-user | 该布尔属性用于指定是否使用经过身份验证的用户的名称来管理缓存内容的不同版本。|
| vary-by | 评估此属性以提供用于管理不同版本内容的密钥。|
| priority | 此属性用于指定当内存缓存空间不足并清除未过期的缓存内容时将考虑的相对优先级。| 
清单 26-17 将上一节中的 img 元素替换为包含时间戳的内容。
Listing 26-17. Caching Content in the _SimpleLayout.cshtml File in the Views/Shared Folder
```html
<div class="m-2">
    <h6 class="bg-primary text-white m-2 p-2">
        Uncached timestamp: @DateTime.Now.ToLongTimeString()
    </h6>
    <cache>
        <h6 class="bg-primary text-white m-2 p-2">
            Cached timestamp: @DateTime.Now.ToLongTimeString()
        </h6>
    </cache>
    @RenderBody()
</div>
```
`cache`元素用于表示应该缓存的内容区域，并已应用于包含时间戳的 h6 元素之一。
重启 ASP.NET Core 和浏览器请求 http://localhost:5000/home/list，两个时间戳将相同。重新加载浏览器，你会看到缓存的内容用于其中一个 h6 元素，时间戳没有改变。  
**USING DISTRIBUTED CACHING FOR CONTENT**
CacheTagHelper 类使用的缓存是基于内存的，这意味着它的容量受可用 RAM 的限制，并且每个应用程序服务器维护一个单独的缓存。当可用容量不足时，内容将从缓存中弹出，当应用程序停止或重新启动时，所有内容都会丢失。   
distributed-cache 元素可用于将内容存储在共享缓存中，这可确保所有应用程序服务器使用相同的数据并且缓存在重启后仍然存在。分布式缓存元素配置有与缓存元素相同的属性，如表 26-8 中所述。有关设置分布式缓存的详细信息，请参阅第 17 章。

### Setting Cache Expiry
expires-* 属性允许您指定缓存内容何时过期，表示为绝对时间或相对于当前时间的时间，或者指定不请求缓存内容的持续时间。在清单 26-18 中，我使用了 expires-after 属性来指定内容应该缓存 15 秒。
Listing 26-18. Setting Cache Expiry in the _SimpleLayout.cshtml File in the Views/Shared Folder
```html
<cache expires-after="@TimeSpan.FromSeconds(15)">
```

#### **Setting a Fixed Expiry Point**
您可以使用 expires-on 属性指定缓存内容过期的固定时间，该属性接受 DateTime 值，如清单 26-19 所示。
```html
<cache expires-on="@DateTime.Parse("2100-01-01")">
```
#### **Setting a Last-Used Expiry Period**
expires-sliding 属性用于指定一个时间段，如果内容尚未从缓存中检索到，则在该时间段之后内容将过期。在示例 26-20 中，我指定了 10 秒的滑动到期时间。
Listing 26-20. Using a Sliding Expiry in the _SimpleLayout.cshtml File in the Views/Shared Folder
您可以通过重新启动 ASP.NET Core 并请求 http://localhost:5000/home/list 并定期重新加载页面来查看 express-sliding 属性的效果。  
如果您在 10 秒内重新加载页面，将使用缓存的内容，并推迟10秒。如果等待超过 10 秒重新加载页面，那么缓存的内容将被丢弃，视图组件将用于生成新内容，并且该过程将重新开始。

#### **Using Cache Variations**
默认情况下，所有请求都会收到相同的缓存内容。 CacheTagHelper 类可以维护缓存内容的不同版本，并使用它们来满足不同类型的 HTTP 请求，使用名称以 vary-by 开头的属性之一指定。清单 26-21 显示了使用 vary-by-route 属性根据路由系统匹配的操作值创建缓存变体。
Listing 26-21. Creating a Variation in the _SimpleLayout.cshtml File in the Views/Shared Folder
```html
<cache expires-sliding="@TimeSpan.FromSeconds(10)" vary-by-route="action">
```
如果重新启动 ASP.NET Core 并使用两个浏览器选项卡请求 http://localhost:5000/home/index 和 http://localhost:5000/home/list，您将看到每个窗口都收到自己的缓存内容它自己的过期时间，因为每个请求都会产生不同的动作路由值。

## Using the Hosting Environment Tag Helper
EnvironmentTagHelper 类应用于自定义环境元素，并根据托管环境确定发送到浏览器的 HTML 中是否包含内容区域，我在第 15 章和第 16 章中对此进行了描述。环境元素依赖于 names 属性，我已在表 26-9 中对其进行了描述。
Table 26-9. The Built-in Tag Helper Attribute for environment Elements
| Name | Description |
|-|-|
| names | 此属性用于指定以逗号分隔的托管环境名称列表，环境元素中包含的内容将包含在发送给客户端的 HTML 中。|
在清单 26-22 中，我向共享布局添加了环境元素，包括开发和生产托管环境视图中的不同内容。
Listing 26-22. Using environment in the _SimpleLayout.cshtml File in the Views/Shared Folder
```html
<div class="m-2">
    <environment names="development">
        <h2 class="bg-info text-white m-2 p-2">This is Development</h2>
    </environment>
    <environment names="production">
        <h2 class="bg-danger text-white m-2 p-2">This is Production</h2>
    </environment>
    @RenderBody()
</div>
```
在清单 26-22 中，我向共享布局添加了环境元素，包括开发和生产托管环境视图中的不同内容。