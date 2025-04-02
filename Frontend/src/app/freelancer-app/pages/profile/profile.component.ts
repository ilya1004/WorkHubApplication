import { Component } from '@angular/core';
import {ProfileService} from '../../services/profile.service';
import {FreelancerUser} from '../../interfaces/profile.interface';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzImageViewComponent} from 'ng-zorro-antd/experimental/image';
import {NgOptimizedImage} from '@angular/common';
import {NzCardComponent} from 'ng-zorro-antd/card';

@Component({
  selector: 'app-profile',
  imports: [
    NzFlexDirective,
    NzImageViewComponent,
    NgOptimizedImage,
    NzCardComponent
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {

  userData: FreelancerUser = {
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

  constructor(
    private profileService: ProfileService,
  ) {
    this.profileService.getUserData()
      .subscribe(value => {
        this.userData = value;
    });
  }


}
