namespace VirtualBasket.Api.Models;

public class DiscountProduct
{
    public int Id { get; set; }
    public int DiscountRuleId { get; set; }
    public int ProductId { get; set; }
    
    public Discount? DiscountRule { get; set; }
    public Product? Product { get; set; }
}
