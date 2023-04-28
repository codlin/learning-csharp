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
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> Index(long id = 1)
    {
        return View(await context.Products.FindAsync(id));
    }
}