export interface FreelancerUserDto {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  about: string;
  email: string;
  registeredAt: string;
  stripeAccountId?: string;
  skills: FreelancerSkillDto[];
  imageUrl?: string;
  roleName: string;
}

export interface FreelancerSkillDto {
  id: string;
  name: string;
}