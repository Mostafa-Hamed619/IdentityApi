import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class PlayServicesService {

  constructor(private http : HttpClient) { }

  getplayers(){
    return this.http.get(`${environment.appUrl}/Api/Play/get-players`);
  }
}
