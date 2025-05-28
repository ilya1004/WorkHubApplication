import {CommonModule} from '@angular/common';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {NzCardModule} from 'ng-zorro-antd/card';
import {NzInputModule} from 'ng-zorro-antd/input';
import {NzInputNumberModule} from 'ng-zorro-antd/input-number';
import {NzSelectModule} from 'ng-zorro-antd/select';
import {NzDatePickerModule} from "ng-zorro-antd/date-picker";
import {NzButtonModule} from "ng-zorro-antd/button";
import {Category} from '../../../../core/interfaces/project/category.interface';
import {CreateProjectForm, ProjectCreateData} from '../../../interfaces/project-tools/create-project.interface';
import {NzFormModule} from "ng-zorro-antd/form";
import {PaymentMethod} from "../../../interfaces/finance/payment-method.interface";
import {FinanceService} from "../../../services/finance.service";

@Component({
  selector: 'app-create-project',
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
  templateUrl: './create-project.component.html',
  styleUrls: ['./create-project.component.scss']
})
export class CreateProjectComponent implements OnInit {
  @Input() categories: Category[] = [];
  @Input() disablePastDates!: (current: Date) => boolean;
  @Output() projectCreated = new EventEmitter<ProjectCreateData>();
  @Output() cancel = new EventEmitter<void>();
  
  createProjectForm!: FormGroup<CreateProjectForm>;
  paymentMethods: PaymentMethod[] = [];
  
  constructor(
    private formBuilder: FormBuilder,
    private financeService: FinanceService
  ) {}
  
  ngOnInit(): void {
    this.initForm();
    this.loadPaymentMethods();
  }
  
  initForm(): void {
    this.createProjectForm = this.formBuilder.group<CreateProjectForm>({
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
          Validators.pattern(/^\d{1,10}(\.\d{1,2})?$/)
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
      }),
      paymentMethodId: new FormControl<string>('', {
        nonNullable: true,
        validators: [Validators.required]
      })
    }, { validators: [this.dateSequenceValidator, this.futureDateValidator] });
  }
  
  loadPaymentMethods(): void {
    this.financeService.getMyPaymentMethods().subscribe({
      next: (methods) => {
        this.paymentMethods = methods;
        if (methods.length > 0) {
          this.createProjectForm.patchValue({ paymentMethodId: methods[0].id });
        }
      },
      error: (err) => {
        console.error('Failed to load payment methods:', err);
      }
    });
  }
  
  submitCreateProject(): void {
    if (this.createProjectForm.valid) {
      const value = this.createProjectForm.getRawValue();
      this.projectCreated.emit({
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
        },
        paymentMethodId: value.paymentMethodId
      });
    }
  }
  
  onCancel(): void {
    this.cancel.emit();
    this.createProjectForm.reset();
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
  
  getPaymentMethodLabel(paymentMethod: PaymentMethod): string {
    return `${paymentMethod.card?.brand} (**** **** **** ${paymentMethod.card?.last4Digits})`;
  }
}