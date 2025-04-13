import {Component, OnInit} from '@angular/core';
import {DatePipe, NgForOf, NgIf} from "@angular/common";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzSpinComponent} from "ng-zorro-antd/spin";
import {NzCardComponent} from "ng-zorro-antd/card";
import {NzIconDirective} from "ng-zorro-antd/icon";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators} from "@angular/forms";
import {NzSpaceComponent, NzSpaceItemDirective} from "ng-zorro-antd/space";
import {NzInputDirective, NzInputGroupComponent} from "ng-zorro-antd/input";
import {NzOptionComponent, NzSelectComponent} from "ng-zorro-antd/select";
import {NzProgressComponent} from "ng-zorro-antd/progress";
import {EmployerUser} from "../../../core/interfaces/employer/employer-user.interface";
import {EmployerIndustry} from '../../../core/interfaces/employer/employer-industry.interface';
import {ProfileService} from "../../services/profile.service";
import {Router, RouterModule} from "@angular/router";
import {NzMessageService} from "ng-zorro-antd/message";
import {NzModalService} from "ng-zorro-antd/modal";
import {UsersService} from "../../../core/services/users/users.service";

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    NzFlexDirective,
    NzSpinComponent,
    NzCardComponent,
    NzIconDirective,
    NzButtonComponent,
    ReactiveFormsModule,
    NzSpaceComponent,
    NzSpaceItemDirective,
    NzInputDirective,
    NzSelectComponent,
    NzOptionComponent,
    NgIf,
    NgForOf,
    NzProgressComponent,
    NzInputGroupComponent,
    DatePipe,
    RouterModule
  ],
  providers: [NzMessageService, NzModalService],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  availableIndustries: EmployerIndustry[] = [];
  isEditing: boolean = false;
  isChangingPassword: boolean = false;
  isLoadingUserData: boolean = true;
  isLoadingIndustries: boolean = true;
  isUpdating: boolean = false;
  isChangingPasswordInProgress: boolean = false;
  uploadProgress: number = 0;
  
  currentPasswordVisible: boolean = false;
  newPasswordVisible: boolean = false;
  confirmPasswordVisible: boolean = false;
  
  userData: EmployerUser = {
    id: '',
    userName: '',
    companyName: '',
    about: '',
    email: '',
    registeredAt: '',
    stripeCustomerId: null,
    industry: null,
    imageUrl: null,
    roleName: ''
  };
  
  editEmployerForm = new FormGroup({
    companyName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    about: new FormControl<string | null>(null, { validators: [Validators.maxLength(1000)] }),
    industryId: new FormControl<string | null>(null),
    resetImage: new FormControl(false, { nonNullable: true }),
    image: new FormControl<File | null>(null),
  });
  
  changePasswordForm = new FormGroup({
    currentPassword: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(8)] }),
    newPassword: new FormControl('', { nonNullable: true, validators: [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/[a-z]/),
        Validators.pattern(/[A-Z]/),
        Validators.pattern(/[0-9]/),
        Validators.pattern(/[^a-zA-Z0-9]/)
      ] }),
    confirmNewPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  }, { validators: this.passwordsMatchValidator });
  
  constructor(
    private profileService: ProfileService,
    private usersService: UsersService,
    private message: NzMessageService,
    private modal: NzModalService,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    this.loadUserData();
    this.loadAvailableIndustries();
  }
  
  loadUserData(): void {
    this.isLoadingUserData = true;
    this.profileService.getUserData().subscribe({
      next: (value) => {
        this.userData = value;
        this.isLoadingUserData = false;
      },
      error: (err) => {
        console.error('Error loading employer data:', err);
        this.message.error('Failed to load user data', { nzDuration: 3000 });
        this.isLoadingUserData = false;
      }
    });
  }
  
  loadAvailableIndustries(): void {
    this.isLoadingIndustries = true;
    this.profileService.getAvailableIndustries().subscribe({
      next: (result) => {
        this.availableIndustries = result.items;
        this.isLoadingIndustries = false;
      },
      error: (err) => {
        console.error('Error loading industries:', err);
        this.message.error('Failed to load industries', { nzDuration: 3000 });
        this.isLoadingIndustries = false;
      }
    });
  }
  
  onFileSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      const file = fileInput.files[0];
      this.editEmployerForm.patchValue({ image: file });
    }
  }
  
  onSubmitEditForm(): void {
    if (this.editEmployerForm.valid) {
      this.isUpdating = true;
      const formData = new FormData();
      const formValue = this.editEmployerForm.getRawValue();
      
      formData.append('EmployerProfile.CompanyName', formValue.companyName);
      formData.append('EmployerProfile.About', formValue.about || '');
      formData.append('EmployerProfile.IndustryId', formValue.industryId || '');
      formData.append('EmployerProfile.ResetImage', String(formValue.resetImage));
      if (formValue.image) {
        formData.append('ImageFile', formValue.image);
      }
      
      const interval = setInterval(() => {
        this.uploadProgress += 20;
        if (this.uploadProgress >= 100) clearInterval(interval);
      }, 100);
      
      this.profileService.updateEmployerProfile(formData).subscribe({
        next: () => {
          this.isUpdating = false;
          this.uploadProgress = 0;
          this.message.success('Profile updated successfully!', { nzDuration: 3000 });
          this.loadUserData();
          this.isEditing = false;
        },
        error: (err) => {
          console.error('Error updating profile:', err);
          this.message.error('Failed to update profile', { nzDuration: 3000 });
          this.isUpdating = false;
          this.uploadProgress = 0;
        }
      });
    }
  }
  
  onClickEdit(): void {
    this.isEditing = !this.isEditing;
    this.isChangingPassword = false;
    if (this.isEditing) {
      this.uploadProgress = 0;
      this.editEmployerForm.patchValue({
        companyName: this.userData.companyName,
        about: this.userData.about,
        industryId: this.userData.industry?.id || null,
        resetImage: false,
        image: null
      });
    }
  }
  
  onCancelEdit(): void {
    this.isEditing = false;
    this.uploadProgress = 0;
    this.editEmployerForm.reset({
      companyName: this.userData.companyName,
      about: this.userData.about,
      industryId: this.userData.industry?.id || null,
      resetImage: false,
      image: null
    });
  }
  
  onImageError(event: Event): void {
    (event.target as HTMLImageElement).src = '/assets/images/avatar-placeholder.png';
  }
  
  onClickChangePassword(): void {
    this.isChangingPassword = !this.isChangingPassword;
    this.isEditing = false;
    if (!this.isChangingPassword) {
      this.changePasswordForm.reset();
      this.currentPasswordVisible = false;
      this.newPasswordVisible = false;
      this.confirmPasswordVisible = false;
    }
  }
  
  onSubmitChangePassword(): void {
    if (this.changePasswordForm.valid) {
      this.isChangingPasswordInProgress = true;
      const formValue = this.changePasswordForm.getRawValue();
      
      const request = {
        email: this.userData.email,
        currentPassword: formValue.currentPassword,
        newPassword: formValue.newPassword
      };
      
      this.profileService.changePassword(request).subscribe({
        next: () => {
          this.isChangingPasswordInProgress = false;
          this.message.success('Password changed successfully!', { nzDuration: 3000 });
          this.isChangingPassword = false;
          this.changePasswordForm.reset();
        },
        error: (err) => {
          this.isChangingPasswordInProgress = false;
          this.message.error(err.error?.message || 'Failed to change password', { nzDuration: 3000 });
          console.error('Error changing password:', err);
        }
      });
    }
  }
  
  onClickDeleteAccount(): void {
    this.modal.confirm({
      nzTitle: 'Delete Account',
      nzContent: 'Are you sure you want to delete your account? This action cannot be undone.',
      nzOkText: 'Delete',
      nzCancelText: 'Cancel',
      nzOnOk: () => this.deleteAccount()
    });
  }
  
  private deleteAccount(): void {
    this.usersService.deleteUser(this.userData.id).subscribe({
      next: () => {
        this.message.success('Account deleted successfully', { nzDuration: 3000 });
        localStorage.removeItem('jwt_token');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Error deleting account:', err);
        this.message.error(err.error?.message || 'Failed to delete account', { nzDuration: 3000 });
      }
    });
  }
  
  private passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmNewPassword')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
  }
}