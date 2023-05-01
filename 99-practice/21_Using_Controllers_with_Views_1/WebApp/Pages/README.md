# Understanding Razor Pages
当您了解 Razor Pages 的工作原理时，您将看到它们与 MVC 框架共享功能。事实上，Razor Pages 通常被描述为 MVC 框架的简化——这是事实——但这并不能说明 Razor Pages 为什么有用。   
MVC 框架以相同的方式解决所有问题：控制器定义选择视图以产生响应的操作方法。这是一个有效的解决方案，因为它非常灵活：控制器可以定义多个响应不同请求的操作方法，操作方法可以决定在处理请求时使用哪个视图，视图可以依赖于私有或分享部分观点以产生回应。  
并非 Web 应用程序中的每个功能都需要 MVC 框架的灵活性。对于许多功能，将使用单个操作方法来处理范围广泛的请求，所有这些请求都使用相同的视图进行处理。 Razor Pages 提供了一种更加集中的方法，将标记和 C# 代码联系在一起，牺牲了集中的灵活性。  
但是 Razor Pages 有局限性。 Razor Pages 开始时倾向于专注于单一功能，但随着功能的增强逐渐失去控制。而且，与 MVC 控制器不同，Razor Pages 不能用于创建 Web 服务。  
您不必只选择一种模型，因为 MVC 框架和 Razor Pages 共存，如本章所示。这意味着可以使用 Razor Pages 轻松开发自包含功能，而将应用程序的更复杂方面留给使用 MVC 控制器和操作来实现。

## Configuring Razor Pages
要为 Razor Pages 准备应用程序，必须将语句添加到 Program.cs 文件以设置服务和配置端点路由系统，如清单 23-3 所示。
```cs
...
builder.Services.AddRazorPages();
...
app.MapRazorPages();
```
