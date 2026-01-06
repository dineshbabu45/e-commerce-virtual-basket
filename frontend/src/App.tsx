import { useState, useEffect } from 'react';
import './App.css';
import type { Product, BasketItem, PricingResponse } from './types';
import { api } from './api';

function App() {
  const [products, setProducts] = useState<Product[]>([]);
  const [basket, setBasket] = useState<BasketItem[]>([]);
  const [pricing, setPricing] = useState<PricingResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  // Auto-calculate pricing whenever basket changes
  useEffect(() => {
    if (basket.length > 0) {
      calculatePricing();
    } else {
      setPricing(null);
    }
  }, [basket]);

  const loadData = async () => {
    try {
      const productsData = await api.getProducts();
      setProducts(productsData);
    } catch (err) {
      setError('Failed to load data. Make sure the backend is running.');
    }
  };

  const addToBasket = (productId: number) => {
    setBasket((prev) => {
      const existing = prev.find((item) => item.productId === productId);
      if (existing) {
        return prev.map((item) =>
          item.productId === productId
            ? { ...item, quantity: item.quantity + 1 }
            : item
        );
      }
      return [...prev, { productId, quantity: 1 }];
    });
  };

  const removeFromBasket = (productId: number) => {
    setBasket((prev) => {
      const existing = prev.find((item) => item.productId === productId);
      if (!existing) return prev;
      
      if (existing.quantity === 1) {
        return prev.filter((item) => item.productId !== productId);
      }
      return prev.map((item) =>
        item.productId === productId
          ? { ...item, quantity: item.quantity - 1 }
          : item
      );
    });
  };

  const calculatePricing = async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await api.calculatePricing({
        items: basket,
        discountRuleIds: [], // Empty - discounts are auto-applied
      });
      setPricing(result);
    } catch (err) {
      setError('Failed to calculate pricing');
      setPricing(null);
    } finally {
      setLoading(false);
    }
  };

  const clearBasket = () => {
    setBasket([]);
    setPricing(null);
  };

  const getProductById = (id: number) => products.find((p) => p.id === id);

  return (
    <div className="container">
      <h1>Virtual Basket - Online Pricing Calculator</h1>

      {error && <div className="error">{error}</div>}

      <div className="layout">
        <div className="products-section">
          <h2>Products</h2>
          <div className="products-grid">
            {products.map((product) => (
              <div key={product.id} className="product-card">
                <h3>{product.name}</h3>
                <p className="price">${product.price.toFixed(2)}</p>
                <button onClick={() => addToBasket(product.id)}>Add to Basket</button>
              </div>
            ))}
          </div>
        </div>

        <div className="basket-section">
          <h2>Basket</h2>
          {basket.length === 0 ? (
            <p>Your basket is empty</p>
          ) : (
            <>
              <div className="basket-items">
                {basket.map((item) => {
                  const product = getProductById(item.productId);
                  const itemDiscount = pricing?.itemDiscounts?.find(d => d.productId === item.productId);
                  
                  return product ? (
                    <div key={item.productId} className="basket-item-container">
                      <div className="basket-item">
                        <span>{product.name}</span>
                        <div className="quantity-controls">
                          <button onClick={() => removeFromBasket(item.productId)}>-</button>
                          <span>{item.quantity}</span>
                          <button onClick={() => addToBasket(item.productId)}>+</button>
                        </div>
                        <span className="item-total">
                          ${(product.price * item.quantity).toFixed(2)}
                        </span>
                      </div>
                      {itemDiscount && (
                        <div className="item-discount">
                          Discount: {itemDiscount.discountDescription} - ${itemDiscount.discountAmount.toFixed(2)}
                        </div>
                      )}
                    </div>
                  ) : null;
                })}
              </div>

              {pricing && pricing.itemDiscounts && pricing.itemDiscounts.length > 0 && (
                <div className="applied-discounts">
                  <h3>Applied Discounts</h3>
                  {pricing.itemDiscounts.map((itemDiscount, index) => (
                    <div key={index} className="applied-discount-item">
                      <span>{itemDiscount.productName}: {itemDiscount.discountDescription}</span>
                      <span className="discount">-${itemDiscount.discountAmount.toFixed(2)}</span>
                    </div>
                  ))}
                </div>
              )}

              <div className="actions">
                <button onClick={clearBasket} className="clear-btn">
                  Clear Basket
                </button>
              </div>

              {pricing && (
                <div className="pricing-result">
                  <div className="pricing-row">
                    <span>Subtotal:</span>
                    <span>${pricing.subtotal.toFixed(2)}</span>
                  </div>
                  <div className="pricing-row">
                    <span>Discount:</span>
                    <span className="discount">-${pricing.discount.toFixed(2)}</span>
                  </div>
                  <div className="pricing-row total">
                    <span>Total:</span>
                    <span>${pricing.total.toFixed(2)}</span>
                  </div>
                </div>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}

export default App;
