import {Category} from "./category.interface";
import {Lifecycle} from "./lifecycle.interface";
import {FreelancerApplication} from "./freelancer-application.interface";

export interface Project {
    id: string;
    title: string;
    description: string;
    budget: number;
    categoryId: string | null;
    category: Category | null;
    freelancerApplications: FreelancerApplication[];
    employerId: string;
    freelancerId: string;
    lifecycle: Lifecycle;
}
