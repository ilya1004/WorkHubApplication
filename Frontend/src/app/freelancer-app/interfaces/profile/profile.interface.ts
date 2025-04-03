export interface FreelancerUser {
  firstName: string;
  lastName: string;
  about: string;
  email: string | null;
  registeredAt: string;
  stripeAccountId: string | null;
  skills: string[];
  imageUrl: string | null;
  roleName: string;
}
