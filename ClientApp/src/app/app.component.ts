import { Component, OnInit } from '@angular/core';
import { AccountService } from './account/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{

  constructor(private AccountServices : AccountService){}
  ngOnInit(): void {
    this.refreshUser();
  }
  refreshUser(){
    const jwt = this.AccountServices.getJWT();
    if(jwt){
      this.AccountServices.refreshjwt(jwt).subscribe({
        next:_=>{},
        error:_=>{
          this.AccountServices.logout()
        }
      })
    }else{
      this.AccountServices.refreshjwt(null).subscribe();
    }
  }
  title = 'ClientApp';
}
