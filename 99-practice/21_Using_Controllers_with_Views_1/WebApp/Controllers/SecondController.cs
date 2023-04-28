using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class SecondController : Controller
{
    public IActionResult Index()
    {
        return View("Common");
    }

    /// <summary>
    /// 指定视图时，必须指定相对于项目文件夹的路径，以 / 字符开头。
    /// 请注意，使用了文件的全名，包括文件扩展名。
    /// 这是一种应该谨慎使用的技术，因为它会创建对特定文件的依赖性，而不是让视图引擎选择文件。
    /// </summary>
    /// <returns></returns>
    public IActionResult Index2()
    {
        return View("/Views/Shared/Common.cshtml");
    }
}