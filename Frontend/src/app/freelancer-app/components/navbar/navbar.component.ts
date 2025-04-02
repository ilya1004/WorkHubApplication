import { Component } from '@angular/core';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzSpaceCompactComponent} from 'ng-zorro-antd/space';
import {NzButtonComponent} from 'ng-zorro-antd/button';

@Component({
  selector: 'app-navbar',
  imports: [
    NzFlexDirective,
    NzSpaceCompactComponent,
    NzButtonComponent
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

}
