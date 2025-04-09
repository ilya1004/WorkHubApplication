import {Category} from "./category.interface";
import {Lifecycle} from "./lifecycle.interface";
import {FreelancerApplication} from "./freelancer-application.interface";

export interface Project {
    id: number;
    title: string;
    description: string;
    budget: number;
    categoryId: string;
    category: Category;
    freelancerApplications: FreelancerApplication[];
    employerId: string;
    freelancerId: string;
    lifecycle: Lifecycle;
}
