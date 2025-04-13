import {FreelancerProfile} from "./freelancer-profile.interface";
import {UserRole} from "./user-role.interface";
import {EmployerProfile} from "./employer-profile.interface";

export interface AppUser {
  id: string;
  userName: string;
  email: string;
  registeredAt: string;
  imageUrl: string | null;
  freelancerProfile: FreelancerProfile | null;
  employerProfile: EmployerProfile | null;
  role: UserRole;
}