namespace VirtualBasket.Api.Models;

public class ItemDiscount
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public string DiscountDescription { get; set; } = string.Empty;
}

public class PricingResponse
{
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public List<ItemDiscount> ItemDiscounts { get; set; } = new();
}
