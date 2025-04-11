import {Card} from "./card.interface";

export interface PaymentMethod {
  id: string;
  type: string;
  card: Card | null;
  createdAt: string;
}