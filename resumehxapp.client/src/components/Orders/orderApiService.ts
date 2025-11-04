import type { CreateOrderRequest, OrderResponse, OrderDetailResponse } from './types';

const ORDER_API_BASE_URL = 'http://localhost:5001/api';

class OrderApiService {
  private generateIdempotencyKey(): string {
    return `web-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }

  async createOrder(request: CreateOrderRequest): Promise<OrderResponse> {
    const idempotencyKey = this.generateIdempotencyKey();
    
    const response = await fetch(`${ORDER_API_BASE_URL}/orders`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Idempotency-Key': idempotencyKey,
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'Failed to create order');
    }

    return response.json();
  }

  async getOrder(orderId: string): Promise<OrderDetailResponse> {
    const response = await fetch(`${ORDER_API_BASE_URL}/orders/${orderId}`);

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Order not found');
      }
      throw new Error('Failed to fetch order');
    }

    return response.json();
  }

  async pollOrderStatus(
    orderId: string,
    onUpdate: (order: OrderDetailResponse) => void,
    intervalMs: number = 2000
  ): Promise<() => void> {
    let isPolling = true;

    const poll = async () => {
      while (isPolling) {
        try {
          const order = await this.getOrder(orderId);
          onUpdate(order);

          // Stop polling if order reached terminal state
          if (['Confirmed', 'FailedInventory', 'FailedPayment', 'Cancelled', 'Failed'].includes(order.status)) {
            isPolling = false;
            break;
          }
        } catch (error) {
          console.error('Error polling order status:', error);
        }

        await new Promise(resolve => setTimeout(resolve, intervalMs));
      }
    };

    poll();

    // Return stop function
    return () => {
      isPolling = false;
    };
  }
}

export const orderApiService = new OrderApiService();
