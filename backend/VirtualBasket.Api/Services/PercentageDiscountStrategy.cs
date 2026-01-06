using VirtualBasket.Api.Models;

namespace VirtualBasket.Api.Services;

public class PercentageDiscountStrategy : IDiscountStrategy
{
    public decimal CalculateDiscount(List<BasketItem> items, Dictionary<int, Product> products, Discount rule, List<int> applicableProductIds)
    {
        if (!rule.PercentageOff.HasValue || applicableProductIds.Count == 0)
            return 0;

        // Calculate subtotal only for products that are in the applicableProductIds list
        var subtotal = items
            .Where(item => applicableProductIds.Contains(item.ProductId))
            .Sum(item =>
            {
                if (products.TryGetValue(item.ProductId, out var product))
                    return product.Price * item.Quantity;
                return 0;
            });

        return subtotal * (rule.PercentageOff.Value / 100);
    }
}
