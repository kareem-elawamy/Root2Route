export interface LatestOrder {
    orderId: string;
    buyerName: string;
    totalAmount: number;
    shippingFees: number;
    status: number;
    orderDate: string;
    shippingCity?: string;
}
