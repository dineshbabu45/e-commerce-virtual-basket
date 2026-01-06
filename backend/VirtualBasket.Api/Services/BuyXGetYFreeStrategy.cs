using VirtualBasket.Api.Models;

namespace VirtualBasket.Api.Services;

public class BuyXGetYFreeStrategy : IDiscountStrategy
{
    public decimal CalculateDiscount(List<BasketItem> items, Dictionary<int, Product> products, Discount rule, List<int> applicableProductIds)
    {
        if (!rule.BuyQuantity.HasValue || !rule.GetQuantity.HasValue || applicableProductIds.Count == 0)
            return 0;

        decimal totalDiscount = 0;

        // Apply discount to each applicable product using aggregated quantities
        foreach (var item in items.Where(i => applicableProductIds.Contains(i.ProductId)))
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                continue;

            // Calculate how many free items the customer gets based on complete sets
            // Example: For Buy 2 Get 1 Free with quantity 3: 3/(2+1) = 1 set = 1 free item
            // Example: For Buy 2 Get 1 Free with quantity 6: 6/(2+1) = 2 sets = 2 free items
            var totalPerSet = rule.BuyQuantity.Value + rule.GetQuantity.Value;
            var completeSets = item.Quantity / totalPerSet;
            var freeItems = completeSets * rule.GetQuantity.Value;

            totalDiscount += freeItems * product.Price;
        }

        return totalDiscount;
    }
}
