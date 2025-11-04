import { useState } from 'react';
import { CreateOrder } from './CreateOrder';
import { OrderStatusView } from './OrderStatus';

export function OrdersPage() {
  const [currentOrderId, setCurrentOrderId] = useState<string | null>(null);

  const handleOrderCreated = (orderId: string) => {
    setCurrentOrderId(orderId);
  };

  const handleBackToCreate = () => {
    setCurrentOrderId(null);
  };

  return (
    <div className="orders-page">
      {currentOrderId ? (
        <OrderStatusView orderId={currentOrderId} onBack={handleBackToCreate} />
      ) : (
        <CreateOrder onOrderCreated={handleOrderCreated} />
      )}
    </div>
  );
}
