import { Component } from '@angular/core';
import {ProfileService} from '../../services/profile.service';
import {FreelancerUser} from '../../interfaces/profile/freelancer-user.interface';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NgForOf, NgIf, NgOptimizedImage} from '@angular/common';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzTagComponent} from 'ng-zorro-antd/tag';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {EditFreelancerForm} from '../../interfaces/profile/edit-form.interface';
import {NzOptionComponent, NzSelectComponent} from 'ng-zorro-antd/select';
import {FreelancerSkill} from '../../interfaces/profile/skill.interface';
import {skip} from 'rxjs';


@Component({
  selector: 'app-profile',
  imports: [
    NzFlexDirective,
    NgOptimizedImage,
    NzCardComponent,
    NzTagComponent,
    NzButtonComponent,
    NzIconDirective,
    FormsModule,
    NgIf,
    NzInputDirective,
    NzSpaceComponent,
    ReactiveFormsModule,
    NzSelectComponent,
    NzOptionComponent,
    NgForOf,
    NzSpaceItemDirective
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {

  availableSkills: FreelancerSkill[] = [];
  isEditing: boolean = false;

  userData: FreelancerUser = {
    id: '',
    userName: '',
    firstName: '',
    lastName: '',
    about: '',
    email: null,
    registeredAt: '',
    stripeAccountId: null,
    skills: [],
    imageUrl: null,
    roleName: ''
  };

  editFreelancerForm = new FormGroup<EditFreelancerForm>({
    firstName: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ]
    }),
    lastName: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ]
    }),
    about: new FormControl(this.userData.about, {
      nonNullable: true,
      validators: [
        Validators.maxLength(1000)
      ]
    }),
    skillIds: new FormControl<string[]>([], {
      nonNullable: true
    }),
    resetImage: new FormControl(false, {
      nonNullable: true
    }),
    image: new FormControl<File | null>(null),
  });

  constructor(
    private profileService: ProfileService,
  ) {
    this.profileService.getUserData()
      .subscribe(value => {
        this.userData = value;
    });

    this.profileService.getAvailableSkill()
      .subscribe(value => {
        this.availableSkills = value.items;
      })
  }

  onFileSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.editFreelancerForm.patchValue({ image: fileInput.files[0] });
    }
  }

  onSubmitEditForm() {
    if (this.editFreelancerForm.valid) {
      const formData = new FormData();
      const formValue = this.editFreelancerForm.getRawValue();

      formData.append("FreelancerProfile.FirstName", formValue.firstName);
      formData.append("FreelancerProfile.LastName", formValue.lastName);
      formData.append("FreelancerProfile.About", formValue.about);
      formData.append("FreelancerProfile.ResetImage", String(formValue.resetImage));

      formValue.skillIds.forEach((id, index) => {
        formData.append(`FreelancerProfile.SkillIds[${index}]`, id);
      });

      if (formValue.image) {
        formData.append("ImageFile", formValue.image);
      }

      this.profileService.updateFreelancerProfile(formData).subscribe({
        next: () => {
          console.log("Profile updated successfully");

          // 🔄 Обновляем данные пользователя после сохранения
          this.profileService.getUserData().subscribe(updated => {
            this.userData = updated;

            // Можно сразу обновить форму (опционально)
            this.editFreelancerForm.patchValue({
              firstName: updated.firstName,
              lastName: updated.lastName,
              about: updated.about,
              skillIds: updated.skills.map(skill => skill.id),
              resetImage: false,
              image: null
            });

            // Закрываем режим редактирования
            this.isEditing = false;
          });
        },
        error: (err) => {
          console.error("Error updating profile", err);
        },
      });
    }
  }

  onClickEdit() {
    this.isEditing = !this.isEditing;

    if (this.isEditing) {
      this.editFreelancerForm.patchValue({
        firstName: this.userData.firstName,
        lastName: this.userData.lastName,
        about: this.userData.about,
        skillIds: this.userData.skills.map(skill => skill.id)
      });
    }
  }

  protected readonly skip = skip;
}
