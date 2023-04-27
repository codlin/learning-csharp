## Validating Data
当您从客户端接受数据时，您必须假设很多数据都是无效的，并准备好过滤掉应用程序无法使用的值。
为 MVC 框架控制器提供的数据验证功能在第 29 章中有详细描述，但对于这一章，我将只关注一个问题：确保客户端为在数据库中存储数据所需的属性提供值。
模型绑定的第一步是将属性应用于数据模型类的属性，如清单 19-24 所示。
```C#
public class ProductBindingTarget {
    ...
    [Required]
    public string Name { get; set; } = "";
    ...
}
```
