namespace SimpleApp.Models;

public interface IDataSource
{
    IEnumerable<Product> Products { get; }
}

public class Product
{
    public String Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }

    //public static Product[] GetProducts() {
    // Product kayak = new Product {
    // Name = "Kayak", Price = 275M
    // };
    // Product lifejacket = new Product {
    // Name = "Lifejacket", Price = 48.95M
    // };
    // return new Product[] { kayak, lifejacket };
    //}
}

public class ProductDataSource : IDataSource
{
    public IEnumerable<Product> Products => new Product[] {
        new Product {
            Name = "Kayak",
            Price = 275M
        },
        new Product
        {
            Name = "Lifejacket",
            Price = 48.95M
        }
    };
}