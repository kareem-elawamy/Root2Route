export interface ProductResponse {
    id: string;
    organizationId: string;
    name: string;
    description?: string;
    images: string[];
    mainImageUrl?: string;
    stockQuantity: number;
    isAvailableForDirectSale: boolean;
    directSalePrice: number;
    isAvailableForAuction: boolean;
    startBiddingPrice: number;
    barcode?: string;
    expiryDate?: string;
    weightUnit?: string;
    productType?: string;
}
