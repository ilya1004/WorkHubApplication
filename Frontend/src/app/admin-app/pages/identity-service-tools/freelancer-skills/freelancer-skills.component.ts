import {Component, OnInit} from '@angular/core';
import {CommonModule} from "@angular/common";
import {NzTableModule} from "ng-zorro-antd/table";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzSpinModule} from "ng-zorro-antd/spin";
import {NzFormModule} from "ng-zorro-antd/form";
import {NzInputModule} from "ng-zorro-antd/input";
import {NzCardModule} from "ng-zorro-antd/card";
import {NzModalModule, NzModalService} from "ng-zorro-antd/modal";
import {FormsModule} from "@angular/forms";
import {NzMessageService} from "ng-zorro-antd/message";
import {FreelancerSkill} from "../../../../core/interfaces/freelancer/freelancer-skill.interface";
import {FreelancerSkillsService} from "../../../services/freelancer-skills/freelancer-skills.service";
import {PaginatedResult} from "../../../../core/interfaces/common/paginated-result.interface";
import {NzFlexDirective} from "ng-zorro-antd/flex";

interface SkillForm {
  id: string | null;
  name: string;
}

@Component({
  selector: 'app-freelancer-skills',
  standalone: true,
  imports: [
    CommonModule,
    NzTableModule,
    NzButtonModule,
    NzSpinModule,
    NzFormModule,
    NzInputModule,
    NzCardModule,
    NzModalModule,
    FormsModule,
    NzFlexDirective
  ],
  providers: [NzModalService, NzMessageService],
  templateUrl: './freelancer-skills.component.html',
  styleUrl: './freelancer-skills.component.scss'
})
export class FreelancerSkillsComponent implements OnInit {
  skills: FreelancerSkill[] = [];
  totalCount = 0;
  loading = false;
  pageNo = 1;
  pageSize = 5;
  
  showCreateForm = false;
  showEditForm = false;
  createForm: SkillForm = { id: null, name: '' };
  editForm: SkillForm = { id: null, name: '' };
  
  constructor(
    private skillsService: FreelancerSkillsService,
    private modal: NzModalService,
    private message: NzMessageService
  ) {}
  
  ngOnInit(): void {
    this.loadSkills();
  }
  
  loadSkills(): void {
    this.loading = true;
    this.skillsService.getAllSkills(this.pageNo, this.pageSize).subscribe({
      next: (result: PaginatedResult<FreelancerSkill>) => {
        this.skills = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.message.error('Failed to load skills', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.pageNo = page;
    this.loadSkills();
  }
  
  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNo = 1;
    this.loadSkills();
  }
  
  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
    this.showEditForm = false;
    this.createForm = { id: null, name: '' };
  }
  
  createSkill(): void {
    if (!this.createForm.name.trim()) {
      this.message.error('Name cannot be empty', { nzDuration: 2000 });
      return;
    }
    if (this.createForm.name.length > 200) {
      this.message.error('Name must not exceed 200 characters', { nzDuration: 2000 });
      return;
    }
    
    this.loading = true;
    this.skillsService.createSkill(this.createForm.name).subscribe({
      next: () => {
        this.message.success('Skill created successfully', { nzDuration: 2000 });
        this.showCreateForm = false;
        this.createForm = { id: null, name: '' };
        this.loadSkills();
      },
      error: () => {
        this.message.error('Failed to create skill', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  editSkill(skill: FreelancerSkill): void {
    this.showEditForm = true;
    this.showCreateForm = false;
    this.editForm = { id: skill.id, name: skill.name };
  }
  
  updateSkill(): void {
    if (!this.editForm.name.trim()) {
      this.message.error('Name cannot be empty', { nzDuration: 2000 });
      return;
    }
    if (this.editForm.name.length > 200) {
      this.message.error('Name must not exceed 200 characters', { nzDuration: 2000 });
      return;
    }
    if (!this.editForm.id) {
      this.message.error('Invalid skill ID', { nzDuration: 2000 });
      return;
    }
    
    this.loading = true;
    this.skillsService.updateSkill(this.editForm.id, this.editForm.name).subscribe({
      next: () => {
        this.message.success('Skill updated successfully', { nzDuration: 2000 });
        this.showEditForm = false;
        this.editForm = { id: null, name: '' };
        this.loadSkills();
      },
      error: () => {
        this.message.error('Failed to update skill', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  cancelEdit(): void {
    this.showEditForm = false;
    this.editForm = { id: null, name: '' };
  }
  
  deleteSkill(skillId: string): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this skill?',
      nzOnOk: () => {
        this.loading = true;
        this.skillsService.deleteSkill(skillId).subscribe({
          next: () => {
            this.message.success('Skill deleted successfully', { nzDuration: 2000 });
            this.loadSkills();
          },
          error: () => {
            this.message.error('Failed to delete skill', { nzDuration: 2000 });
            this.loading = false;
          }
        });
      }
    });
  }
}