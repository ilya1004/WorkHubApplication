import { Component } from '@angular/core';
import {ProfileService} from '../../services/profile.service';
import {FreelancerProfile} from '../../interfaces/profile.interface';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {

  userData: FreelancerProfile;

  constructor(
    private profileService: ProfileService,
  ) {
    this.profileService.getUserData().subscribe(value => {
        this.userData = value;
    });
  }


}
