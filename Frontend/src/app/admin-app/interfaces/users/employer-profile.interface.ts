import {EmployerIndustry} from "../../../core/interfaces/employer/employer-industry.interface";

export interface EmployerProfile {
  companyName: string;
  about: string | null;
  industryId: string | null;
  industry: EmployerIndustry | null;
  stripeCustomerId?: string;
  userId: string;
}