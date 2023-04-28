using Microsoft.AspNetCore.Mvc;

using WebApp.Models;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private DataContext context;
    public HomeController(DataContext ctx)
    {
        context = ctx;
    }

    /// <summary>
    /// MVC 框架假定 HTML 控制器定义的任何公共方法都是一个操作方法，并且该操作方法支持所有 HTTP 方法。
    /// 如果您需要在控制器中定义一个不是动作的方法，您可以将其设为私有，或者如果不可能，则使用 NonAction 属性修饰该方法。
    /// 您可以通过应用属性来限制操作方法以支持特定的 HTTP 方法，以便 HttpGet 属性表示处理 GET 请求的操作，HttpPost 方法表示处理 POST 请求的操作，等等。
    /// 当操作方法调用 View 方法时，它会创建一个 ViewResult，告诉 MVC 框架使用默认约定来定位视图。 Razor 视图引擎查找与操作方法同名的视图，
    /// 并添加 cshtml 文件扩展名，这是 Razor 视图引擎使用的文件类型。视图存储在 Views 文件夹中，按它们关联的控制器分组。
    /// 搜索的第一个位置是 Views/Home 文件夹，因为操作方法是由 Home 控制器定义的（其名称是通过从控制器类的名称中删除 Controller 而获得的）。
    /// 如果在 Views/Home 文件夹中找不到 Index.cshtml 文件，则会检查 Views/Shared 文件夹，这是存储控制器之间共享的视图的位置。
    /// 虽然大多数控制器都有自己的视图，但也可以共享视图，这样就不必复制常用功能。
    /// 路由约定用于使用 Home 控制器定义的 Index 操作方法处理请求，它告诉 Razor 视图引擎使用视图搜索约定来定位视图。
    /// 视图引擎使用操作方法和控制器的名称来构建其搜索模式并检查 Views/Home/Index.cshtml 和 Views/Shared/Index.cshtml 文件。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> Index(long id = 1)
    {
        return View(await context.Products.FindAsync(id));
    }
}