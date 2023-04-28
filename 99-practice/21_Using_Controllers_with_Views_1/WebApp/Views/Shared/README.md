
# Using Shared Views
当 Razor 视图引擎找到一个视图时，它会在 View/[controller] 文件夹中查找，然后在 Views/Shared 文件夹中查找。
这种搜索模式意味着包含公共内容的视图可以在控制器之间共享，避免重复。

# Working with Layouts
示例应用程序中的视图包含处理设置 HTML 文档、定义 head 部分、加载 Bootstrap CSS 文件等的重复元素。   
Razor 支持布局，将常用内容合并到一个文件中，供任何视图使用。布局通常存储在 Views/Shared 文件夹中，因为它们通常由多个控制器的操作方法使用。  
在 Views/Shared 文件夹中创建一个名为 _Layout.cshtml 的文件。
**布局包含将由多个视图使用的公共内容。通过调用 RazorPage<T> 类继承的 RenderBody 方法将每个视图独有的内容插入到响应中**。  
使用布局的视图可以仅关注其独特的内容，如图所示在清单 22-10 中。
Listing 22-10. Using a Layout in the Index.cshtml File in the Views/Home Folder
```cs
@model Product
@{
    Layout = "_Layout";
}
<div class="m-2">
   ... 
</div>
```