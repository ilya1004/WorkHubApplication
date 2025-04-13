import {FreelancerSkill} from "../../../core/interfaces/freelancer/freelancer-skill.interface";

export interface FreelancerProfile {
  firstName: string;
  lastName: string;
  about: string | null;
  skills: FreelancerSkill[];
  stripeAccountId: string | null;
  userId: string;
}