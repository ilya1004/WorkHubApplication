export interface EmployerUser {
  id: string;
  userName: string;
  companyName: string;
  about: string;
  email: string;
  registeredAt: string;
  stripeCustomerId?: string;
  industryName: string;
  imageUrl?: string;
  roleName: string;
}