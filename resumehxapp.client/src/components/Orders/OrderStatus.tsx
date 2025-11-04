import { useEffect, useState } from 'react';
import type { OrderDetailResponse, OrderStatus } from './types';
import { orderApiService } from './orderApiService';
import './OrderStatus.css';

interface OrderStatusProps {
  orderId: string;
  onBack: () => void;
}

export function OrderStatusView({ orderId, onBack }: OrderStatusProps) {
  const [order, setOrder] = useState<OrderDetailResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let stopPolling: (() => void) | null = null;

    const fetchOrder = async () => {
      try {
        const orderData = await orderApiService.getOrder(orderId);
        setOrder(orderData);
        setLoading(false);

        // Start polling for status updates
        stopPolling = await orderApiService.pollOrderStatus(orderId, (updatedOrder) => {
          setOrder(updatedOrder);
        });
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch order');
        setLoading(false);
      }
    };

    fetchOrder();

    return () => {
      if (stopPolling) {
        stopPolling();
      }
    };
  }, [orderId]);

  if (loading) {
    return (
      <div className="order-status loading">
        <div className="spinner"></div>
        <p>Loading order details...</p>
      </div>
    );
  }

  if (error || !order) {
    return (
      <div className="order-status error">
        <h2>Error</h2>
        <p>{error || 'Order not found'}</p>
        <button onClick={onBack} className="btn-primary">
          Back to Create Order
        </button>
      </div>
    );
  }

  const getStatusClass = (status: string): string => {
    const statusMap: Record<string, string> = {
      Created: 'status-pending',
      Accepted: 'status-processing',
      InventoryReserved: 'status-processing',
      PaymentAuthorized: 'status-processing',
      Confirmed: 'status-success',
      FailedInventory: 'status-failed',
      FailedPayment: 'status-failed',
      EmailFailed: 'status-warning',
      Cancelled: 'status-cancelled',
      Failed: 'status-failed',
    };
    return statusMap[status] || 'status-pending';
  };

  const getStatusIcon = (status: string): string => {
    const iconMap: Record<string, string> = {
      Created: '⏳',
      Accepted: '🔄',
      InventoryReserved: '📦',
      PaymentAuthorized: '💳',
      Confirmed: '✅',
      FailedInventory: '❌',
      FailedPayment: '❌',
      EmailFailed: '⚠️',
      Cancelled: '🚫',
      Failed: '❌',
    };
    return iconMap[status] || '⏳';
  };

  const getStatusDescription = (status: string): string => {
    const descMap: Record<string, string> = {
      Created: 'Order has been created and is waiting to be processed',
      Accepted: 'Order accepted and being processed',
      InventoryReserved: 'Inventory has been reserved for your order',
      PaymentAuthorized: 'Payment has been authorized',
      Confirmed: 'Order confirmed! You will receive a confirmation email.',
      FailedInventory: 'Order failed due to insufficient inventory',
      FailedPayment: 'Order failed due to payment authorization failure',
      EmailFailed: 'Order confirmed but confirmation email failed to send',
      Cancelled: 'Order has been cancelled',
      Failed: 'Order processing failed',
    };
    return descMap[status] || 'Processing...';
  };

  const isTerminalStatus = (status: string): boolean => {
    return ['Confirmed', 'FailedInventory', 'FailedPayment', 'Cancelled', 'Failed'].includes(status);
  };

  const timeline: Array<{ status: OrderStatus; label: string }> = [
    { status: 'Created', label: 'Created' },
    { status: 'Accepted', label: 'Accepted' },
    { status: 'InventoryReserved', label: 'Inventory Reserved' },
    { status: 'PaymentAuthorized', label: 'Payment Authorized' },
    { status: 'Confirmed', label: 'Confirmed' },
  ];

  const getCurrentStepIndex = (): number => {
    const index = timeline.findIndex(step => step.status === order.status);
    return index >= 0 ? index : -1;
  };

  const currentStepIndex = getCurrentStepIndex();

  return (
    <div className="order-status">
      <button onClick={onBack} className="btn-back">
        ← Back to Create Order
      </button>

      <div className="order-header">
        <h2>Order Details</h2>
        <div className="order-id">
          <strong>Order ID:</strong> {order.orderId}
        </div>
      </div>

      <div className={`status-card ${getStatusClass(order.status)}`}>
        <div className="status-icon">{getStatusIcon(order.status)}</div>
        <div className="status-info">
          <h3>{order.status}</h3>
          <p>{getStatusDescription(order.status)}</p>
        </div>
        {!isTerminalStatus(order.status) && (
          <div className="status-updating">
            <span className="pulse-dot"></span>
            <span>Updating...</span>
          </div>
        )}
      </div>

      {!['FailedInventory', 'FailedPayment', 'Cancelled', 'Failed'].includes(order.status) && (
        <div className="timeline">
          {timeline.map((step, index) => (
            <div
              key={step.status}
              className={`timeline-step ${
                index < currentStepIndex
                  ? 'completed'
                  : index === currentStepIndex
                  ? 'current'
                  : 'pending'
              }`}
            >
              <div className="timeline-marker"></div>
              <div className="timeline-label">{step.label}</div>
            </div>
          ))}
        </div>
      )}

      <div className="order-details-grid">
        <div className="detail-card">
          <h3>Customer Information</h3>
          <div className="detail-row">
            <span className="label">Customer ID:</span>
            <span>{order.customerId}</span>
          </div>
          <div className="detail-row">
            <span className="label">Created:</span>
            <span>{new Date(order.createdAt).toLocaleString()}</span>
          </div>
          <div className="detail-row">
            <span className="label">Last Updated:</span>
            <span>{new Date(order.updatedAt).toLocaleString()}</span>
          </div>
        </div>

        <div className="detail-card">
          <h3>Shipping Address</h3>
          <div className="address">
            <p>{order.shippingAddress.name}</p>
            <p>{order.shippingAddress.line1}</p>
            {order.shippingAddress.line2 && <p>{order.shippingAddress.line2}</p>}
            <p>
              {order.shippingAddress.city}, {order.shippingAddress.state}{' '}
              {order.shippingAddress.postalCode}
            </p>
            <p>{order.shippingAddress.country}</p>
          </div>
        </div>
      </div>

      <div className="detail-card">
        <h3>Order Items</h3>
        <div className="items-table-container">
        <table className="items-table">
          <thead>
            <tr>
              <th>SKU</th>
              <th>Name</th>
              <th>Quantity</th>
              <th>Unit Price</th>
              <th>Taxable</th>
              <th>Total</th>
            </tr>
          </thead>
          <tbody>
            {order.items.map((item, index) => (
              <tr key={index}>
                <td>{item.sku}</td>
                <td>{item.name}</td>
                <td>{item.quantity}</td>
                <td>${item.unitPrice.toFixed(2)}</td>
                <td>{item.isTaxable ? 'Yes' : 'No'}</td>
                <td>${(item.quantity * item.unitPrice).toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
        </div>
      </div>

      <div className="detail-card order-summary">
        <h3>Order Summary</h3>
        <div className="summary-row">
          <span>Subtotal:</span>
          <span>${order.subtotal.toFixed(2)}</span>
        </div>
        <div className="summary-row">
          <span>Shipping:</span>
          <span>${order.shippingCost.toFixed(2)}</span>
        </div>
        <div className="summary-row">
          <span>Tax (9.85%):</span>
          <span>${order.tax.toFixed(2)}</span>
        </div>
        <div className="summary-row total">
          <span><strong>Total ({order.currency}):</strong></span>
          <span><strong>${order.total.toFixed(2)}</strong></span>
        </div>
      </div>
    </div>
  );
}
