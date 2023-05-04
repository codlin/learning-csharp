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