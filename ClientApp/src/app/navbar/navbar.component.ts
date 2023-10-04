import { Component, TestabilityRegistry } from '@angular/core';
import { AccountService } from '../account/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

  collapse = true;
  constructor(public accountServices : AccountService){}

  Logout(){
    this.accountServices.logout()
  }

  toggleCollapsed(){
    this.collapse = !this.collapse;
  }
}
