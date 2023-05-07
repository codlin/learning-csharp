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
    public async Task<IActionResult> Index([FromQuery] long? id) 
    {
        ViewBag.Categories = new SelectList(context.Categories, "CategoryId", "Name");
        return View("Form", await context.Products.Include(p => p.Category)
            .Include(p => p.Supplier).FirstOrDefaultAsync(p => id == null || p.ProductId == id));
    }

        public IActionResult Results()
    {
        return View();
    }
}