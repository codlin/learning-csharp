# Understanding Convention Routing

HTML 控制器依赖约定路由而不是 Route 属性。该术语中的约定是指使用控制器类名和用于配置路由系统的操作方法名，例如，通过将以下语句添加到端点路由配置中来完成：
```C#
// Program.cs
...
app.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id?}");
...
```
此语句设置的路由匹配两段和三段 URL。第一段的值作为控制器类的名称，不带Controller后缀，这样Home就是指HomeController类。第二段是动作方法的名称，可选的第三段允许动作方法接收名为 id 的参数。默认值用于为不包含所有段的 URL 选择 Home 控制器上的 Index 操作方法。这是一个非常常见的约定，可以设置相同的路由配置而无需指定 URL 模式，如清单 21-6 所示。
```C#
app.MapDefaultControllerRoute();
```