export interface Charge {
  id: string;
  amount: number;
  currency: string;
  captured: boolean;
  status: string;
  paymentMethod: string;
}