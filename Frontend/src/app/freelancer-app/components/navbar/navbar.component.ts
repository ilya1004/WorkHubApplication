import { Component } from '@angular/core';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzSpaceCompactComponent} from 'ng-zorro-antd/space';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzMenuDirective, NzMenuItemComponent} from 'ng-zorro-antd/menu';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-navbar',
  imports: [
    NzFlexDirective,
    NzSpaceCompactComponent,
    NzButtonComponent,
    NzMenuDirective,
    NzMenuItemComponent,
    NzIconDirective,
    RouterLink
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

}
