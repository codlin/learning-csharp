using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers;

[Authorize]
public class StoreController : Controller
{
    private ProductDbContext DbContext;

    public StoreController(ProductDbContext ctx) => DbContext = ctx;

    public IActionResult Index() => View(DbContext.Products);
}