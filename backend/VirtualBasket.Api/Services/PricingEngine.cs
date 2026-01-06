using VirtualBasket.Api.Models;
using VirtualBasket.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace VirtualBasket.Api.Services;

public class PricingEngine
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<string, IDiscountStrategy> _strategies;

    public PricingEngine(ApplicationDbContext context)
    {
        _context = context;
        _strategies = new Dictionary<string, IDiscountStrategy>
        {
            { "Percentage", new PercentageDiscountStrategy() },
            { "BuyXGetYFree", new BuyXGetYFreeStrategy() }
        };
    }

    public async Task<PricingResponse> CalculatePricing(PricingRequest request)
    {
        // Aggregate basket items by ProductId
        var aggregatedItems = request.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new BasketItem
            {
                ProductId = g.Key,
                Quantity = g.Sum(i => i.Quantity)
            })
            .ToList();

        // Get all products
        var productIds = aggregatedItems.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // Calculate subtotal
        var subtotal = aggregatedItems.Sum(item =>
        {
            if (products.TryGetValue(item.ProductId, out var product))
                return product.Price * item.Quantity;
            return 0;
        });

        // Auto-apply all applicable discounts
        // Get all discount rules with their applicable products
        var allDiscountRules = await _context.DiscountRules.ToListAsync();
        var allDiscountProducts = await _context.DiscountProducts.ToListAsync();

        // Group discounts by product to find the best discount for each product
        var productDiscounts = new Dictionary<int, (Discount rule, decimal amount, string description)>();

        foreach (var item in aggregatedItems)
        {
            decimal bestDiscount = 0;
            Discount? bestRule = null;
            string bestDescription = string.Empty;

            // Find all discounts applicable to this product
            var applicableDiscounts = allDiscountRules
                .Where(dr => allDiscountProducts.Any(dp => dp.DiscountRuleId == dr.Id && dp.ProductId == item.ProductId))
                .ToList();

            foreach (var discountRule in applicableDiscounts)
            {
                if (_strategies.TryGetValue(discountRule.Type, out var strategy))
                {
                    var applicableProductIds = new List<int> { item.ProductId };
                    var discount = strategy.CalculateDiscount(new List<BasketItem> { item }, products, discountRule, applicableProductIds);

                    if (discount > bestDiscount)
                    {
                        bestDiscount = discount;
                        bestRule = discountRule;
                        
                        // Create description
                        if (discountRule.Type == "Percentage")
                        {
                            bestDescription = $"{discountRule.PercentageOff}% off";
                        }
                        else if (discountRule.Type == "BuyXGetYFree")
                        {
                            bestDescription = $"Buy {discountRule.BuyQuantity} Get {discountRule.GetQuantity} Free";
                        }
                    }
                }
            }

            if (bestDiscount > 0 && bestRule != null && products.TryGetValue(item.ProductId, out var product))
            {
                productDiscounts[item.ProductId] = (bestRule, bestDiscount, bestDescription);
            }
        }

        // Build item discounts list and calculate total discount
        var itemDiscounts = new List<ItemDiscount>();
        decimal totalDiscount = 0;

        foreach (var kvp in productDiscounts)
        {
            var productId = kvp.Key;
            var (rule, amount, description) = kvp.Value;
            
            if (products.TryGetValue(productId, out var product))
            {
                itemDiscounts.Add(new ItemDiscount
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    DiscountAmount = Math.Round(amount, 2),
                    DiscountDescription = description
                });
                totalDiscount += amount;
            }
        }

        return new PricingResponse
        {
            Subtotal = Math.Round(subtotal, 2),
            Discount = Math.Round(totalDiscount, 2),
            Total = Math.Round(subtotal - totalDiscount, 2),
            ItemDiscounts = itemDiscounts
        };
    }
}
