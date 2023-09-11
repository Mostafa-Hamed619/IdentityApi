import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../shared/models/Register';
import { environment } from 'src/environments/environment.development';
import { Login } from '../shared/models/Login';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http : HttpClient) { }

  register(model:Register){
    return this.http.post(`${environment.appUrl}/api/Account/Register`,model);
  }

  login(model : Login){
    return this.http.post(`${environment.appUrl}/api/Account/login`,model);
  }
}
