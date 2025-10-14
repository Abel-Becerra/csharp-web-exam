namespace api.Application.DTOs;

/// <summary>
/// Represents products grouped by category with aggregated data.
/// </summary>
public class ProductGroupDto
{
    public int CategoryId { get; set; }
    
    public string CategoryName { get; set; } = string.Empty;
    
    public int ProductCount { get; set; }
    
    public decimal TotalValue { get; set; }
    
    public decimal AveragePrice { get; set; }
    
    public decimal MinPrice { get; set; }
    
    public decimal MaxPrice { get; set; }
}
