export interface PaymentIntent {
  id: string;
  amount: number;
  currency: string;
  status: string;
  created: string;
  transferGroup: string;
  metadata: { [key: string]: string };
}