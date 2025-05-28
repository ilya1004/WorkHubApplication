import {FreelancerSkill} from "./freelancer-skill.interface";

export interface FreelancerUser {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  about: string;
  email: string;
  registeredAt: string;
  stripeAccountId?: string | null;
  skills: FreelancerSkill[];
  imageUrl: string | null;
  roleName: string;
}

