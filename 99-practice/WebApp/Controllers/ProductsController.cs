using Microsoft.AspNetCore.Mvc;

using WebApp.Models;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private DataContext context;
    public ProductsController(DataContext ctx)
    {
        context = ctx;
    }

    [HttpGet]
    public IAsyncEnumerable<Product> GetProducts()
    {
        return context.Products.AsAsyncEnumerable();
    }

    /// <summary>
    /// 为了修复API分析器检测到的问题，可以使用 ProducesResponseType 属性来声明操作方法可以产生的每种响应类型。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult?> GetProduct(long id, [FromServices] ILogger<ProductsController> logger)
    {
        logger.LogDebug("GetProduct Action Invoke");
        Product? p = await context.Products.FindAsync(id);
        if (p == null)
        {
            return NotFound();
        }
        return Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> SaveProduct(ProductBindingTarget target)
    {
        Product p = target.ToProduct();
        await context.Products.AddAsync(p);
        await context.SaveChangesAsync();
        return Ok(p);
    }

    [HttpPut]
    public async void UpdateProduct([FromBody] Product product)
    {
        context.Update(product);
        await context.SaveChangesAsync();
    }

    [HttpDelete("{id}")]
    public async void DeleteProduct(long id)
    {
        context.Products.Remove(new Product() { ProductId = id });
        await context.SaveChangesAsync();
    }

    [HttpGet("redirect")]
    public IActionResult Redirect()
    {
        // return Redirect("/api/products/1");
        return RedirectToAction(nameof(GetProduct), new { Id = 1 });
    }
}