namespace VirtualBasket.Api.Models;

public class Discount
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // "Percentage" or "BuyXGetYFree"
    public decimal? PercentageOff { get; set; } // For percentage discounts: 5, 10, or 20
    public int? BuyQuantity { get; set; } // For BuyXGetYFree
    public int? GetQuantity { get; set; } // For BuyXGetYFree
    
    public List<DiscountProduct> DiscountProducts { get; set; } = new();
}
