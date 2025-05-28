import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {Category} from "../../../../core/interfaces/project/category.interface";
import {CategoriesService} from "../../../../core/services/categories/categories.service";
import {PaginatedResult} from "../../../../core/interfaces/common/paginated-result.interface";
import {NzTableComponent, NzTableModule} from "ng-zorro-antd/table";
import {NzFormItemComponent, NzFormLabelComponent} from "ng-zorro-antd/form";
import {FormsModule} from "@angular/forms";
import {NzInputDirective} from "ng-zorro-antd/input";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NgForOf, NgIf} from "@angular/common";
import {NzCardComponent} from "ng-zorro-antd/card";
import {NzModalService} from "ng-zorro-antd/modal";
import {NzMessageService} from "ng-zorro-antd/message";

interface CategoryForm {
  id: string | null;
  name: string;
}

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    NzTableComponent,
    NzTableModule,
    NzFormItemComponent,
    NzFormLabelComponent,
    FormsModule,
    NzInputDirective,
    NzButtonComponent,
    NzFlexDirective,
    NgIf,
    NgForOf,
    NzCardComponent
  ],
  providers: [NzModalService],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  totalCount = 0;
  loading = false;
  pageNo = 1;
  pageSize = 5;
  
  showCreateForm = false;
  showEditForm = false;
  createForm: CategoryForm = { id: null, name: '' };
  editForm: CategoryForm = { id: null, name: '' };
  
  @Output() categoryChanged = new EventEmitter<void>();
  
  constructor(
    private categoriesService: CategoriesService,
    private modal: NzModalService,
    private message: NzMessageService
  ) {}
  
  ngOnInit(): void {
    this.loadCategories();
  }
  
  loadCategories(): void {
    this.loading = true;
    this.categoriesService.getCategories(this.pageNo, this.pageSize).subscribe({
      next: (result: PaginatedResult<Category>) => {
        this.categories = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.message.error('Failed to load categories', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.pageNo = page;
    this.loadCategories();
  }
  
  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNo = 1;
    this.loadCategories();
  }
  
  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
    this.showEditForm = false;
    this.createForm = { id: null, name: '' };
  }
  
  createCategory(): void {
    if (!this.createForm.name.trim()) {
      this.message.error('Name cannot be empty', { nzDuration: 2000 });
      return;
    }
    if (this.createForm.name.length > 200) {
      this.message.error('Name must not exceed 200 characters', { nzDuration: 2000 });
      return;
    }
    
    this.loading = true;
    this.categoriesService.createCategory(this.createForm.name).subscribe({
      next: () => {
        this.message.success('Category created successfully', { nzDuration: 2000 });
        this.showCreateForm = false;
        this.createForm = { id: null, name: '' };
        this.loadCategories();
        this.categoryChanged.emit();
      },
      error: () => {
        this.message.error('Failed to create category', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  editCategory(category: Category): void {
    this.showEditForm = true;
    this.showCreateForm = false;
    this.editForm = { id: category.id, name: category.name };
  }
  
  updateCategory(): void {
    if (!this.editForm.name.trim()) {
      this.message.error('Name cannot be empty', { nzDuration: 2000 });
      return;
    }
    if (this.editForm.name.length > 200) {
      this.message.error('Name must not exceed 200 characters', { nzDuration: 2000 });
      return;
    }
    if (!this.editForm.id) {
      this.message.error('Invalid category ID', { nzDuration: 2000 });
      return;
    }
    
    this.loading = true;
    this.categoriesService.updateCategory(this.editForm.id, this.editForm.name).subscribe({
      next: () => {
        this.message.success('Category updated successfully', { nzDuration: 2000 });
        this.showEditForm = false;
        this.editForm = { id: null, name: '' };
        this.loadCategories();
        this.categoryChanged.emit();
      },
      error: () => {
        this.message.error('Failed to update category', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  cancelEdit(): void {
    this.showEditForm = false;
    this.editForm = { id: null, name: '' };
  }
  
  deleteCategory(categoryId: string): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this category?',
      nzOnOk: () => {
        this.loading = true;
        this.categoriesService.deleteCategory(categoryId).subscribe({
          next: () => {
            this.message.success('Category deleted successfully', { nzDuration: 2000 });
            this.loadCategories();
            this.categoryChanged.emit();
          },
          error: () => {
            this.message.error('Failed to delete category', { nzDuration: 2000 });
            this.loading = false;
          }
        });
      }
    });
  }
}