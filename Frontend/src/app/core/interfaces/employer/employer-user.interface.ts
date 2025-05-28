import {EmployerIndustry} from "./employer-industry.interface";

export interface EmployerUser {
  id: string;
  userName: string;
  companyName: string;
  about: string;
  email: string;
  registeredAt: string;
  stripeCustomerId: string | null;
  industry: EmployerIndustry | null;
  imageUrl: string | null;
  roleName: string;
}