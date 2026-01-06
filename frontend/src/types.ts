export interface Product {
  id: number;
  name: string;
  price: number;
}

export interface BasketItem {
  productId: number;
  quantity: number;
}

export interface DiscountProduct {
  id: number;
  discountRuleId: number;
  productId: number;
}

export interface Discount {
  id: number;
  type: string;
  percentageOff?: number;
  buyQuantity?: number;
  getQuantity?: number;
  discountProducts?: DiscountProduct[];
}

export interface PricingRequest {
  items: BasketItem[];
  discountRuleIds: number[];
}

export interface ItemDiscount {
  productId: number;
  productName: string;
  discountAmount: number;
  discountDescription: string;
}

export interface PricingResponse {
  subtotal: number;
  discount: number;
  total: number;
  itemDiscounts: ItemDiscount[];
}
