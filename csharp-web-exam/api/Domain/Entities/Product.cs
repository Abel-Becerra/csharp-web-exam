namespace api.Domain.Entities;

/// <summary>
/// Represents a product in the system.
/// </summary>
public class Product
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public int CategoryId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property
    public Category? Category { get; set; }
}
