import { Component } from '@angular/core';
import { AccountService } from '../account/account.service';
import { User } from '../shared/models/user';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
constructor(public accountService : AccountService){}

ShowInfo(){
  this.accountService.user$.subscribe({
    next:(response)=>{
      console.log(response)
    }
  })
}
}
