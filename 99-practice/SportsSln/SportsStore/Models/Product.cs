using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models;

public class Product {
    public long? ProductID { get; set; }

    [Required(ErrorMessage = "Please enter a product name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The Price property has been decorated with the Column attribute to specify the SQL data type that 
    /// will be used to store values for this property. Not all C# types map neatly onto SQL types, and this attribute
    /// ensures the database uses an appropriate type for the application data.
    /// </summary>
    /// <value></value>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
    [Column(TypeName = "decimal(8, 2)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Please specify a category")]
    public string Category { get; set; } = string.Empty;
}