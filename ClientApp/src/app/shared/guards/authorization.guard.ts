import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { SharedService } from '../shared.service';
import { User } from '../models/account/user';

@Injectable({
  providedIn : 'root'
})
export class authorizationGuard{
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean>{
    return this.accountServices.user$.pipe(
      map((user :User|null)=>{
        if(user){
          return true;
        }else{
          this.sharedServices.showNotification(false,'Restricted Area','Leave immediately!');
          this.router.navigate(['account/login'],{queryParams:{returnUrl :state.url}});
          return false;
        }
      })
    );
  }

  constructor(private accountServices : AccountService,private sharedServices : SharedService,private router : Router){}
};
