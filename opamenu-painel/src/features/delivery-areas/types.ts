export interface DeliveryArea {
  id: string;
  city: string;
  neighborhood?: string;
  fee: number;
}

export interface CreateDeliveryAreaRequest {
  city: string;
  neighborhood?: string;
  fee: number;
}
