// Order Processing API Types
export interface OrderItem {
  sku: string;
  name: string;
  quantity: number;
  unitPrice: number;
  isTaxable: boolean;
}

export interface ShippingAddress {
  name: string;
  line1: string;
  line2?: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
}

export interface CreateOrderRequest {
  customerId: string;
  currency: string;
  shippingAddress: ShippingAddress;
  shippingCost: number;
  items: OrderItem[];
}

export interface OrderResponse {
  orderId: string;
  status: string;
  customerId: string;
  currency: string;
  subtotal: number;
  shippingCost: number;
  tax: number;
  total: number;
  createdAt: string;
}

export interface OrderDetailResponse extends OrderResponse {
  shippingAddress: ShippingAddress;
  updatedAt: string;
  items: OrderItem[];
}

export type OrderStatus =
  | 'Created'
  | 'Accepted'
  | 'InventoryReserved'
  | 'PaymentAuthorized'
  | 'Confirmed'
  | 'FailedInventory'
  | 'FailedPayment'
  | 'EmailFailed'
  | 'Cancelled'
  | 'Failed';

export interface OrderEvent {
  id: string;
  eventType: string;
  payload: string;
  createdAt: string;
}
