using System.ComponentModel.DataAnnotations;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#nullable disable
namespace IdentityApp.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("/api/data")]
public class ValuesController : ControllerBase
{
    private ProductDbContext DbContext;
    public ValuesController(ProductDbContext dbContext)
    {
        DbContext = dbContext;
    }

    [HttpGet]
    public IAsyncEnumerable<Product> GetProducts() => (IAsyncEnumerable<Product>)DbContext.Products;

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductBindingTarget target)
    {
        if (ModelState.IsValid)
        {
            Product product = new Product
            {
                Name = target.Name,
                Price = target.Price,
                Category = target.Category
            };
            await DbContext.AddAsync(product);
            await DbContext.SaveChangesAsync();
            return Ok(product);
        }
        return BadRequest(ModelState);
    }

    [HttpDelete("{id}")]
    public Task DeleteProduct(long id)
    {
        DbContext.Products.Remove(new Product { Id = id });
        return DbContext.SaveChangesAsync();
    }
}
public class ProductBindingTarget
{
    [Required]
    public string Name { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public string Category { get; set; }
}