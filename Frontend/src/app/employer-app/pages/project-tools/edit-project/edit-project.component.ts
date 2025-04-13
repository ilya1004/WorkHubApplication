import {CommonModule} from '@angular/common';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators} from "@angular/forms";
import {NzButtonModule} from 'ng-zorro-antd/button';
import {NzCardModule} from 'ng-zorro-antd/card';
import {NzFormModule} from 'ng-zorro-antd/form';
import {NzInputModule} from 'ng-zorro-antd/input';
import {Project} from '../../../../core/interfaces/project/project.interface';
import {Category} from '../../../../core/interfaces/project/category.interface';
import {NzSelectModule} from "ng-zorro-antd/select";
import {NzDatePickerModule} from 'ng-zorro-antd/date-picker';
import {NzInputNumberModule} from "ng-zorro-antd/input-number";
import {ProjectUpdateData, UpdateProjectForm} from "../../../interfaces/project-tools/update-project.interface";

@Component({
  selector: 'app-edit-project',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSelectModule,
    NzDatePickerModule,
    NzCardModule,
    NzInputNumberModule
  ],
  templateUrl: './edit-project.component.html',
  styleUrls: ['./edit-project.component.scss']
})
export class EditProjectComponent implements OnInit {
  @Input() project!: Project;
  @Input() categories: Category[] = [];
  @Input() disablePastDates!: (current: Date) => boolean;
  @Output() projectUpdated = new EventEmitter<ProjectUpdateData>();
  @Output() cancel = new EventEmitter<void>();
  
  updateProjectForm!: FormGroup<UpdateProjectForm>;
  
  constructor(private formBuilder: FormBuilder) {}
  
  ngOnInit(): void {
    this.initForm();
    this.patchFormWithProjectData();
  }
  
  initForm(): void {
    this.updateProjectForm = this.formBuilder.group<UpdateProjectForm>({
      title: new FormControl<string>('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(200)]
      }),
      description: new FormControl<string>('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(1000)]
      }),
      budget: new FormControl<number>(0, {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.min(0.01),
          Validators.pattern(/^\d{1,16}(\.\d{1,2})?$/)
        ]
      }),
      categoryId: new FormControl<string | null>(null, {
        validators: []
      }),
      applicationsStartDate: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [Validators.required]
      }),
      applicationsDeadline: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [Validators.required]
      }),
      workStartDate: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [Validators.required]
      }),
      workDeadline: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [Validators.required]
      })
    }, { validators: [this.dateSequenceValidator, this.futureDateValidator] });
  }
  
  patchFormWithProjectData(): void {
    this.updateProjectForm.patchValue({
      title: this.project.title,
      description: this.project.description,
      budget: this.project.budget,
      categoryId: this.project.categoryId,
      applicationsStartDate: new Date(this.project.lifecycle.applicationsStartDate),
      applicationsDeadline: new Date(this.project.lifecycle.applicationsDeadline),
      workStartDate: new Date(this.project.lifecycle.workStartDate),
      workDeadline: new Date(this.project.lifecycle.workDeadline)
    });
  }
  
  submitUpdateProject(): void {
    if (this.updateProjectForm.valid) {
      const value = this.updateProjectForm.getRawValue();
      this.projectUpdated.emit({
        project: {
          title: value.title,
          description: value.description,
          budget: value.budget,
          categoryId: value.categoryId
        },
        lifecycle: {
          applicationsStartDate: value.applicationsStartDate,
          applicationsDeadline: value.applicationsDeadline,
          workStartDate: value.workStartDate,
          workDeadline: value.workDeadline
        }
      });
    }
  }
  
  onCancel(): void {
    this.cancel.emit();
  }
  
  private futureDateValidator(group: AbstractControl): ValidationErrors | null {
    const now = new Date();
    now.setHours(0, 0, 0, 0);
    now.setDate(now.getDate() + 1);
    
    const start = group.get('applicationsStartDate')?.value;
    const deadline = group.get('applicationsDeadline')?.value;
    const workStart = group.get('workStartDate')?.value;
    const workEnd = group.get('workDeadline')?.value;
    
    if (start && start < now) {
      return { applicationsStartDateInPast: true };
    }
    if (deadline && deadline < now) {
      return { applicationsDeadlineInPast: true };
    }
    if (workStart && workStart < now) {
      return { workStartDateInPast: true };
    }
    if (workEnd && workEnd < now) {
      return { workDeadlineInPast: true };
    }
    return null;
  }
  
  private dateSequenceValidator(group: AbstractControl): ValidationErrors | null {
    const start = group.get('applicationsStartDate')?.value;
    const deadline = group.get('applicationsDeadline')?.value;
    const workStart = group.get('workStartDate')?.value;
    const workEnd = group.get('workDeadline')?.value;
    
    if (start && deadline && start >= deadline) {
      return { applicationsDateError: true };
    }
    if (deadline && workStart && deadline >= workStart) {
      return { workStartDateError: true };
    }
    if (workStart && workEnd && workStart >= workEnd) {
      return { workEndDateError: true };
    }
    return null;
  }
}