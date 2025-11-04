import { useState } from 'react';
import type { CreateOrderRequest, OrderItem, ShippingAddress } from './types';
import { orderApiService } from './orderApiService';
import './CreateOrder.css';

interface CreateOrderProps {
  onOrderCreated: (orderId: string) => void;
}

export function CreateOrder({ onOrderCreated }: CreateOrderProps) {
  const [customerId, setCustomerId] = useState('demo-customer');
  const [shippingAddress, setShippingAddress] = useState<ShippingAddress>({
    name: 'John Doe',
    line1: '123 Main Street',
    line2: '',
    city: 'Springfield',
    state: 'IL',
    postalCode: '62701',
    country: 'USA',
  });
  const [shippingCost, setShippingCost] = useState(15.00);
  const [items, setItems] = useState<OrderItem[]>([
    { sku: 'WIDGET-001', name: 'Super Widget', quantity: 1, unitPrice: 29.99, isTaxable: true },
  ]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleAddItem = () => {
    setItems([...items, { sku: '', name: '', quantity: 1, unitPrice: 0, isTaxable: true }]);
  };

  const handleRemoveItem = (index: number) => {
    setItems(items.filter((_, i) => i !== index));
  };

  const handleItemChange = (index: number, field: keyof OrderItem, value: string | number | boolean) => {
    const updatedItems = [...items];
    updatedItems[index] = { ...updatedItems[index], [field]: value };
    setItems(updatedItems);
  };

  const calculateTotals = () => {
    const subtotal = items.reduce((sum, item) => sum + item.quantity * item.unitPrice, 0);
    const taxableAmount = items
      .filter(item => item.isTaxable)
      .reduce((sum, item) => sum + item.quantity * item.unitPrice, 0) + shippingCost;
    const tax = Math.round(taxableAmount * 0.0985 * 100) / 100;
    const total = subtotal + shippingCost + tax;
    return { subtotal, tax, total };
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const request: CreateOrderRequest = {
        customerId,
        currency: 'USD',
        shippingAddress,
        shippingCost,
        items,
      };

      const response = await orderApiService.createOrder(request);
      onOrderCreated(response.orderId);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create order');
    } finally {
      setLoading(false);
    }
  };

  const { subtotal, tax, total } = calculateTotals();

  return (
    <div className="create-order">
      <h2>Create New Order</h2>
      
      {error && (
        <div className="error-banner">
          <strong>Error:</strong> {error}
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="form-grid">
          <div className="form-main">
        <div className="form-section">
          <h3>Customer Information</h3>
          <div className="form-group">
            <label htmlFor="customerId">Customer ID</label>
            <input
              id="customerId"
              type="text"
              value={customerId}
              onChange={(e) => setCustomerId(e.target.value)}
              required
            />
          </div>
        </div>

        <div className="form-section">
          <h3>Shipping Address</h3>
          <div className="form-group">
            <label htmlFor="name">Name</label>
            <input
              id="name"
              type="text"
              value={shippingAddress.name}
              onChange={(e) => setShippingAddress({ ...shippingAddress, name: e.target.value })}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="line1">Address Line 1</label>
            <input
              id="line1"
              type="text"
              value={shippingAddress.line1}
              onChange={(e) => setShippingAddress({ ...shippingAddress, line1: e.target.value })}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="line2">Address Line 2 (Optional)</label>
            <input
              id="line2"
              type="text"
              value={shippingAddress.line2 || ''}
              onChange={(e) => setShippingAddress({ ...shippingAddress, line2: e.target.value })}
            />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="city">City</label>
              <input
                id="city"
                type="text"
                value={shippingAddress.city}
                onChange={(e) => setShippingAddress({ ...shippingAddress, city: e.target.value })}
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="state">State</label>
              <input
                id="state"
                type="text"
                value={shippingAddress.state}
                onChange={(e) => setShippingAddress({ ...shippingAddress, state: e.target.value })}
                required
                maxLength={2}
              />
            </div>

            <div className="form-group">
              <label htmlFor="postalCode">Postal Code</label>
              <input
                id="postalCode"
                type="text"
                value={shippingAddress.postalCode}
                onChange={(e) => setShippingAddress({ ...shippingAddress, postalCode: e.target.value })}
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="country">Country</label>
            <input
              id="country"
              type="text"
              value={shippingAddress.country}
              onChange={(e) => setShippingAddress({ ...shippingAddress, country: e.target.value })}
              required
            />
          </div>
        </div>

        <div className="form-section">
          <h3>Items</h3>
          <div className="items-scroll">
          {items.map((item, index) => (
            <div key={index} className="item-row">
              <div className="form-group">
                <label>SKU</label>
                <input
                  aria-label="SKU"
                  type="text"
                  value={item.sku}
                  onChange={(e) => handleItemChange(index, 'sku', e.target.value)}
                  required
                />
              </div>

              <div className="form-group">
                <label>Name</label>
                <input
                  aria-label="Name"
                  type="text"
                  value={item.name}
                  onChange={(e) => handleItemChange(index, 'name', e.target.value)}
                  required
                />
              </div>

              <div className="form-group">
                <label>Quantity</label>
                <input
                  aria-label="Quantity"
                  type="number"
                  min="1"
                  value={item.quantity}
                  onChange={(e) => handleItemChange(index, 'quantity', parseInt(e.target.value))}
                  required
                />
              </div>

              <div className="form-group">
                <label>Unit Price</label>
                <input
                  aria-label="Unit Price"
                  type="number"
                  min="0.01"
                  step="0.01"
                  value={item.unitPrice}
                  onChange={(e) => handleItemChange(index, 'unitPrice', parseFloat(e.target.value))}
                  required
                />
              </div>

              <div className="form-group checkbox-group">
                <label>
                  <input
                    type="checkbox"
                    checked={item.isTaxable}
                    onChange={(e) => handleItemChange(index, 'isTaxable', e.target.checked)}
                  />
                  Taxable
                </label>
              </div>

              {items.length > 1 && (
                <button
                  type="button"
                  className="btn-remove"
                  onClick={() => handleRemoveItem(index)}
                >
                  Remove
                </button>
              )}
            </div>
          ))}
          </div>

          <button type="button" className="btn-secondary" onClick={handleAddItem}>
            Add Item
          </button>
        </div>

        <div className="form-section">
          <h3>Shipping</h3>
          <div className="form-group">
            <label htmlFor="shippingCost">Shipping Cost</label>
            <input
              id="shippingCost"
              type="number"
              min="0"
              step="0.01"
              value={shippingCost}
              onChange={(e) => setShippingCost(parseFloat(e.target.value))}
              required
            />
          </div>
        </div>
          </div>
          <div className="form-side">
            <div className="order-summary sticky">
              <h3>Order Summary</h3>
              <div className="summary-row">
                <span>Subtotal:</span>
                <span>${subtotal.toFixed(2)}</span>
              </div>
              <div className="summary-row">
                <span>Shipping:</span>
                <span>${shippingCost.toFixed(2)}</span>
              </div>
              <div className="summary-row">
                <span>Tax (9.85%):</span>
                <span>${tax.toFixed(2)}</span>
              </div>
              <div className="summary-row total">
                <span><strong>Total:</strong></span>
                <span><strong>${total.toFixed(2)}</strong></span>
              </div>
            </div>
            <button type="submit" className="btn-primary" disabled={loading}>
              {loading ? 'Creating Order...' : 'Create Order'}
            </button>
          </div>
        </div>
      </form>
    </div>
  );
}
