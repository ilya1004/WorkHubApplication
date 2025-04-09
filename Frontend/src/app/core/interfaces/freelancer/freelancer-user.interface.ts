import {FreelancerSkill} from "./freelancer-skill.interface";

export interface FreelancerUser {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  about: string;
  email: string;
  registeredAt: string;
  stripeAccountId?: string;
  skills: FreelancerSkill[];
  imageUrl?: string;
  roleName: string;
}

