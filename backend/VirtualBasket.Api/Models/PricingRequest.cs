namespace VirtualBasket.Api.Models;

public class PricingRequest
{
    public List<BasketItem> Items { get; set; } = new();
    public List<int> DiscountRuleIds { get; set; } = new();
}
