import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {NzIconDirective} from "ng-zorro-antd/icon";
import {NavbarComponent} from "../navbar/navbar.component";

@Component({
  selector: 'app-employer-layout',
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
