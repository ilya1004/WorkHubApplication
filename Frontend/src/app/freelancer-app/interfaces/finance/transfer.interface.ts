export interface Transfer {
  id: string;
  amount: number;
  currency: string;
  transferGroup: string;
  metadata: { [key: string]: string };
}