import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../shared/models/account/Register';
import { environment } from 'src/environments/environment.development';
import { Login } from '../shared/models/account/Login';
import { User } from '../shared/models/account/user';
import { Router, UrlSegment } from '@angular/router';
import { ReplaySubject, map, observable, of } from 'rxjs';
import { confirmEmail } from '../shared/models/account/confirmEmail';
import { ResetPasswordModel } from '../shared/models/account/ResetPasswordModel';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private userSource = new ReplaySubject<User | null>(1)
  user$ = this.userSource.asObservable();
  constructor(private http : HttpClient,private router : Router) { }

  register(model:Register){
    return this.http.post(`${environment.appUrl}/api/Account/Register`,model);
  }

  login(model : Login){
    return this.http.post<User>(`${environment.appUrl}/api/Account/login`,model).pipe(
      map((user : User)=>{
        if(user){
          this.setUser(user)
          //return user;
        }
        //return null;
      })
    );
  }

  logout(){
    localStorage.removeItem(environment.userKey);
    this.userSource.next(null);
    this.router.navigateByUrl('/')
  }

  confirmEmail(model : confirmEmail){
    return this.http.put(`${environment.appUrl}/Api/Account/confirm-email`,model);
  }

  resendEmailConfirmation(email :string){
    return this.http.post(`${environment.appUrl}/Api/Account/resend-email-confirmation-link/${email}`,{});
  }
  forgotPasswordOrUsername(email : string){
    return this.http.post(`${environment.appUrl}/Api/Account/forgot-username-or-password/${email}`,{});
  }

  resetPasswordFun(model :ResetPasswordModel){
    return this.http.put(`${environment.appUrl}/Api/Account/reset-password`,model);
  }
  refreshjwt(jwt : string |null){
    if(jwt === null){
      this.userSource.next(null);
      return of(undefined)
    }
    let headers = new HttpHeaders();
    headers = headers.set('Authorization','Bearer '+jwt);

    return this.http.get<User>(`${environment.appUrl}/api/account/refresh-user-token`,{headers}).pipe(
      map((user:User)=>{
        this.setUser(user)
      })
    )
  }

  getJWT(){
    const key = localStorage.getItem(environment.userKey);
    if(key){
      const user : User =JSON.parse(key);
      return user.jwt;
    }
    return null
  }

  private setUser(user : User){
    localStorage.setItem(environment.userKey,JSON.stringify(user));
    this.userSource.next(user);
    
    // this.user$.subscribe({
    //   next:(response)=>{
    //     console.log(response)
    //   }
    //})
  }
  
}
