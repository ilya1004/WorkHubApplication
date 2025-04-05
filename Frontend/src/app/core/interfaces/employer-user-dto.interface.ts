export interface EmployerUserDto {
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