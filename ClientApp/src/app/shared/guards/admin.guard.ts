import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { SharedService } from '../shared.service';
import { User } from '../models/account/user';
import jwtDecode from 'jwt-decode';

@Injectable({
  providedIn:'root'
})
export class adminGuard {

  constructor(private accountServices : AccountService,private sharedServices : SharedService,private router:Router){

  }
  canActivate():Observable<boolean> {
    return this.accountServices.user$.pipe(
      map((user : User | null)=>{
        if(user){
          const decodedToken : any = jwtDecode(user.jwt);
          if(decodedToken.role.includes('Admin')){
            return true;
          }
        }
        this.sharedServices.showNotification(false,'Admin area','Leave now Mother fucker !');
        this.router.navigateByUrl("/");
        return false
      })
    )
  }
  
};
