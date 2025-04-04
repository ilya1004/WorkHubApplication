import {FreelancerSkill} from './skill.interface';

export interface FreelancerUser {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  about: string;
  email: string | null;
  registeredAt: string;
  stripeAccountId: string | null;
  skills: FreelancerSkill[];
  imageUrl: string | null;
  roleName: string;
}
