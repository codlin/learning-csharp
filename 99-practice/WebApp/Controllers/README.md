# 控制器说明

Controllers are classes whose methods, known as actions, can process HTTP requests.  
控制器`Controllers`是类，其方法（称为操作`actions`）可以处理 HTTP 请求。

Controllers are discovered automatically when the application is started.  
启动应用程序时会自动发现控制器。

The basic discovery process is simple: any public class whose name ends with Controller is a controller, 
and any public method a controller defines is an action.  
基本的发现过程很简单：名称以 Controller 结尾的任何公共类都是控制器，控制器定义的任何公共方法都是操作。

## 第一个控制类例子
To demonstrate how simple a controller can be, create the WebApp/Controllers folder and add to it a file named `ProductsController.cs`.  
为了演示控制器可以多么简单，创建 WebApp/Controllers 文件夹并向其中添加一个名为 `ProductsController.cs` 的文件。

## ControllerBase
控制器派生自 `ControllerBase` 类，该类提供对 MVC 框架和底层 ASP.NET Core 平台提供的功能的访问。 
表 19-4 描述了 ControllerBase 类提供的最有用的属性。 
| Name             | Description | 
|---|---|
|HttpContext        |This property returns the HttpContext object for the current request.|  
|ModelState         |This property returns details of the data validation process, as demonstrated in the “Validating Data” section later  in the chapter and described in detail in Chapter 29.  |
|Request            |This property returns the HttpRequest object for the current request.  |
|Response           |This property returns the HttpResponse object for the current response.  |
|RouteData          |This property returns the data extracted from the request URL by the routing middleware, as described in Chapter 13.  |
|User               |This property returns an object that describes the user associated with the currentrequest, as described in Chapter 38.  |

尽管控制器通常派生自 `ControllerBase` 或 `Controller` 类（在第 21 章中描述），但这只是约定，
MVC 框架将接受名称以 `Controller` 结尾的任何类，派生自名称以 `Controller` 结尾的类，或者已用 `Controller` 属性装饰。  
将 `NonController` 属性应用于满足这些条件但不应接收 HTTP 请求的类。

## Understanding the Controller Attributes
操作方法支持的 HTTP 方法和 URL 由应用于控制器的属性组合决定。控制器的 URL 由应用于类的 Route 属性指定，如下所示：  
```c#
...
[Route("api/[controller]")]
public class ProductsController: ControllerBase {
...
```
属性参数的 [controller] 部分用于从控制器类的名称派生 URL。类名的 Controller 部分被删除，这意味着`ProductsController.cs`中的属性
将控制器的 URL 设置为 `/api/products`。每个操作都装饰有一个属性，该属性指定它支持的 HTTP 方法，如下所示：
```c#
...
[HttpGet]
public Product[] GetProducts() {
...
```
操作方法命名在用于 Web 服务的控制器中无关紧要。控制器还有其他用途，名称确实很重要，但对于 Web 服务，重要的是 HTTP 方法属性和路由模式。 
HttpGet 属性告诉 MVC 框架 GetProducts 操作方法将处理 HTTP GET 请求。表 19-5 描述了可应用于操作以指定 HTTP 方法的完整属性集。
| Name             | Description | 
|---|---|
|HttpGet     |This attribute specifies that the action can be invoked only by HTTP requests that use the GET verb.|
|HttpPost    |This attribute specifies that the action can be invoked only by HTTP requests that use the POST verb.|
|HttpDelete  |This attribute specifies that the action can be invoked only by HTTP requests that use the DELETE verb.|
|HttpPut     |This attribute specifies that the action can be invoked only by HTTP requests that use the PUT verb.|
|HttpPatch   |This attribute specifies that the action can be invoked only by HTTP requests that use the PATCH verb.|
|HttpHead    |This attribute specifies that the action can be invoked only by HTTP requests that use the HEAD verb. |
|AcceptVerbs |This attribute is used to specify multiple HTTP verbs.|

应用于操作以指定 HTTP 方法的属性也可用于构建控制器的基本 URL。
```c#
...
[HttpGet("{id}")]
public Product GetProduct() {
...
```
此属性告诉 MVC 框架，GetProduct 操作方法处理对 URL 模式 api/products/{id} 的 GET 请求。在发现过程中，应用于控制器的属性用于构建控制器可以处理的 URL 模式集，如表 19-6 所示。
| HTTP Method | URL Pattern | Action Method Name | 
|---|---|---|
| GET | api/products      | GetProducts |
| GET | api/products/{id} | GetProduct  |
