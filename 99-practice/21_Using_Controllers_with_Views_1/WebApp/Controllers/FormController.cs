using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

[AutoValidateAntiforgeryToken]
public class FormController : Controller
{
    private DataContext context;
    public FormController(DataContext dbContext)
    {
        context = dbContext;
    }
    public async Task<IActionResult> Index(long? id)
    {
        ViewBag.Categories = new SelectList(context.Categories, "CategoryId", "Name");
        return View("Form", await context.Products.Include(p => p.Category)
            .Include(p => p.Supplier).FirstOrDefaultAsync(p => id == null || p.ProductId == id));
    }
    // [HttpPost]
    // public IActionResult SubmitForm()
    // {
    //     foreach (string key in Request.Form.Keys)
    //     // .Where(k => !k.StartsWith("_")))
    //     {
    //         TempData[key] = string.Join(", ", Request.Form[key]);
    //     }
    //     return RedirectToAction(nameof(Results));
    // }
    // [HttpPost]
    // public IActionResult SubmitForm(string name, decimal price)
    // {
    //     TempData["name param"] = name;
    //     TempData["price param"] = price.ToString();
    //     return RedirectToAction(nameof(Results));
    // }
    // [HttpPost]
    // public IActionResult SubmitForm(Product product)
    // {
    //     TempData["product"] = System.Text.Json.JsonSerializer.Serialize(product);
    //     return RedirectToAction(nameof(Results));
    // }

    // [HttpPost]
    // public IActionResult SubmitForm(Category category)
    // {
    //     TempData["category"] = System.Text.Json.JsonSerializer.Serialize(category);
    //     return RedirectToAction(nameof(Results));
    // }
    // [HttpPost]
    // public IActionResult SubmitForm([Bind(Prefix = "Category")] Category category)
    // {
    //     TempData["category"] = System.Text.Json.JsonSerializer.Serialize(category);
    //     return RedirectToAction(nameof(Results));
    // }
    [HttpPost]
    public IActionResult SubmitForm([Bind("Name", "Category")] Product product)
    {
        TempData["name"] = product.Name;
        TempData["price"] = product.Price.ToString();
        TempData["category name"] = product.Category?.Name;
        return RedirectToAction(nameof(Results));
    }

    public IActionResult Results()
    {
        return View();
    }
}