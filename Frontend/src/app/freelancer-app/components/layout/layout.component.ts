import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {NavbarComponent} from '../navbar/navbar.component';
import {NzIconDirective} from "ng-zorro-antd/icon";

@Component({
  selector: 'app-freelancer-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    NavbarComponent,
    NzIconDirective
  ],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {

}
