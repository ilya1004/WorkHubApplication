import {Component, OnInit} from '@angular/core';
import {ProfileService} from '../../services/profile.service';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {DatePipe, NgForOf, NgIf} from '@angular/common';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzTagComponent} from 'ng-zorro-antd/tag';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {NzOptionComponent, NzSelectComponent} from 'ng-zorro-antd/select';
import {NzSpinComponent} from "ng-zorro-antd/spin";
import {NzProgressComponent} from "ng-zorro-antd/progress";
import {NzAlertComponent} from "ng-zorro-antd/alert";
import {FreelancerSkill} from "../../../core/interfaces/freelancer/freelancer-skill.interface";
import {FreelancerUser} from "../../../core/interfaces/freelancer/freelancer-user.interface";


@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    NzFlexDirective,
    NzCardComponent,
    NzTagComponent,
    NzButtonComponent,
    NzIconDirective,
    NzInputDirective,
    NzSpaceComponent,
    NzSelectComponent,
    NzOptionComponent,
    NzSpaceItemDirective,
    NgIf,
    NgForOf,
    NzSpinComponent,
    NzProgressComponent,
    NzAlertComponent,
    ReactiveFormsModule,
    DatePipe,
    NzInputGroupComponent
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  availableSkills: FreelancerSkill[] = [];
  
  isEditing: boolean = false;
  isChangingPassword: boolean = false;
  isLoadingUserData: boolean = true;
  isLoadingSkills: boolean = true;
  isUpdating: boolean = false;
  isChangingPasswordInProgress: boolean = false;
  
  uploadProgress: number = 0;
  successMessage: string | null = null;
  passwordChangeMessage: string | null = null;
  passwordChangeError: string | null = null;
  
  currentPasswordVisible: boolean = false;
  newPasswordVisible: boolean = false;
  confirmPasswordVisible: boolean = false;
  
  userData: FreelancerUser = {
    id: '',
    userName: '',
    firstName: '',
    lastName: '',
    about: '',
    email: '',
    registeredAt: '',
    stripeAccountId: null,
    skills: [],
    imageUrl: null,
    roleName: ''
  };
  
  editFreelancerForm = new FormGroup({
    firstName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    lastName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    about: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(1000)] }),
    skillIds: new FormControl<string[]>([], { nonNullable: true }),
    resetImage: new FormControl(false, { nonNullable: true }),
    image: new FormControl<File | null>(null),
  });
  
  changePasswordForm = new FormGroup({
    currentPassword: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
    newPassword: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
    confirmNewPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  }, { validators: this.passwordsMatchValidator });
  
  constructor(private profileService: ProfileService) {}
  
  ngOnInit(): void {
    this.loadUserData();
    this.loadAvailableSkills();
  }
  
  loadUserData(): void {
    this.isLoadingUserData = true;
    this.profileService.getUserData().subscribe({
      next: (value) => {
        this.userData = value;
        this.isLoadingUserData = false;
      },
      error: (err) => {
        console.error('Error loading user data:', err);
        this.isLoadingUserData = false;
      }
    });
  }
  
  loadAvailableSkills(): void {
    this.isLoadingSkills = true;
    this.profileService.getAvailableSkill().subscribe({
      next: (value) => {
        this.availableSkills = value.items;
        this.isLoadingSkills = false;
      },
      error: (err) => {
        console.error('Error loading skills:', err);
        this.isLoadingSkills = false;
      }
    });
  }
  
  onFileSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      const file = fileInput.files[0];
      this.editFreelancerForm.patchValue({ image: file });
    }
  }
  
  onSubmitEditForm(): void {
    if (this.editFreelancerForm.valid) {
      this.isUpdating = true;
      const formData = new FormData();
      const formValue = this.editFreelancerForm.getRawValue();
      
      formData.append('FreelancerProfile.FirstName', formValue.firstName);
      formData.append('FreelancerProfile.LastName', formValue.lastName);
      formData.append('FreelancerProfile.About', formValue.about);
      formData.append('FreelancerProfile.ResetImage', String(formValue.resetImage));
      formValue.skillIds.forEach((id, index) => {
        formData.append(`FreelancerProfile.SkillIds[${index}]`, id);
      });
      if (formValue.image) {
        formData.append('ImageFile', formValue.image);
      }
      
      this.uploadProgress = 0;
      const interval = setInterval(() => {
        this.uploadProgress += 20;
        if (this.uploadProgress >= 100) clearInterval(interval);
      }, 200);
      
      this.profileService.updateFreelancerProfile(formData).subscribe({
        next: () => {
          this.isUpdating = false;
          this.uploadProgress = 0;
          this.successMessage = 'Profile updated successfully!';
          this.loadUserData();
          this.isEditing = false;
          setTimeout(() => this.successMessage = null, 5000);
        },
        error: (err) => {
          console.error('Error updating profile:', err);
          this.isUpdating = false;
          this.uploadProgress = 0;
        }
      });
    }
  }
  
  onClickEdit(): void {
    this.isEditing = !this.isEditing;
    this.uploadProgress = 0;
    if (this.isEditing) {
      this.editFreelancerForm.patchValue({
        firstName: this.userData.firstName,
        lastName: this.userData.lastName,
        about: this.userData.about,
        skillIds: this.userData.skills.map(skill => skill.id)
      });
    }
  }
  
  onCancelEdit(): void {
    this.isEditing = false;
    this.uploadProgress = 0;
    this.editFreelancerForm.reset({
      firstName: this.userData.firstName,
      lastName: this.userData.lastName,
      about: this.userData.about,
      skillIds: this.userData.skills.map(skill => skill.id),
      resetImage: false,
      image: null
    });
  }
  
  onImageError(event: Event): void {
    (event.target as HTMLImageElement).src = '/assets/images/avatar-placeholder.png';
  }
  
  onClickChangePassword(): void {
    this.isChangingPassword = !this.isChangingPassword;
    if (!this.isChangingPassword) {
      this.changePasswordForm.reset();
      this.passwordChangeMessage = null;
      this.passwordChangeError = null;
      this.currentPasswordVisible = false;
      this.newPasswordVisible = false;
      this.confirmPasswordVisible = false;
    }
  }
  
  onSubmitChangePassword(): void {
    if (this.changePasswordForm.valid) {
      this.isChangingPasswordInProgress = true;
      this.passwordChangeError = null;
      const formValue = this.changePasswordForm.getRawValue();
      
      const request = {
        email: this.userData.email!,
        currentPassword: formValue.currentPassword,
        newPassword: formValue.newPassword
      };
      
      this.profileService.changePassword(request).subscribe({
        next: () => {
          this.isChangingPasswordInProgress = false;
          this.passwordChangeMessage = 'Password changed successfully!';
          this.isChangingPassword = false;
          this.changePasswordForm.reset();
          setTimeout(() => this.passwordChangeMessage = null, 5000);
        },
        error: (err) => {
          this.isChangingPasswordInProgress = false;
          this.passwordChangeError = err.error?.message || 'Failed to change password. Please try again.';
          console.error('Error changing password:', err);
        }
      });
    }
  }
  
  private passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmNewPassword')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
  }
}