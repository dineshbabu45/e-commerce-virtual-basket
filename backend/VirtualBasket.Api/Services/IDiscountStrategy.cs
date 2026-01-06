using VirtualBasket.Api.Models;

namespace VirtualBasket.Api.Services;

public interface IDiscountStrategy
{
    decimal CalculateDiscount(List<BasketItem> items, Dictionary<int, Product> products, Discount rule, List<int> applicableProductIds);
}
