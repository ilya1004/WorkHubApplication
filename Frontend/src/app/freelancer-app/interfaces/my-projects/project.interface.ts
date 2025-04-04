import {Category} from "./category.interface";
import {Lifecycle} from "./lifecycle.interface";

export interface Project {
    id: number;
    title: string;
    description: string;
    budget: number;
    categoryId: string;
    category: Category;
    employerId: string;
    freelancerId: string;
    lifecycle: Lifecycle;
}