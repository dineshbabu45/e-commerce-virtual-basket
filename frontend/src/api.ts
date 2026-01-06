import type { Product, Discount, PricingRequest, PricingResponse } from './types';

const API_BASE_URL = 'http://localhost:5000/api';

export const api = {
  async getProducts(): Promise<Product[]> {
    const response = await fetch(`${API_BASE_URL}/pricing/products`);
    if (!response.ok) throw new Error('Failed to fetch products');
    return response.json();
  },

  async getDiscounts(): Promise<Discount[]> {
    const response = await fetch(`${API_BASE_URL}/pricing/discounts`);
    if (!response.ok) throw new Error('Failed to fetch discounts');
    return response.json();
  },

  async calculatePricing(request: PricingRequest): Promise<PricingResponse> {
    const response = await fetch(`${API_BASE_URL}/pricing/calculate`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });
    if (!response.ok) throw new Error('Failed to calculate pricing');
    return response.json();
  },
};
