using Microsoft.AspNetCore.Mvc;

using WebApp.Models;

namespace WebApp.Controllers;

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

    [HttpGet("{id}")]
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
    public async Task<IActionResult> SavaProduct([FromBody] ProductBindingTarget target)
    {
        Product p = target.ToProduct();
        await context.Products.AddAsync(p);
        await context.SaveChangesAsync();

        // 把生成的包含`id`的对象返回
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
}