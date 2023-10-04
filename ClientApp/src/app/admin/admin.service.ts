import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { MemberDto } from '../shared/models/Admin/MemberDto';
import { MemberAddEdit } from '../shared/models/Admin/MemberAddEdit';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private http : HttpClient) { }

  getMembers(){
    return this.http.get<MemberDto[]>(`${environment.appUrl}/Api/Admin/get-members`);
  }

  getMember(id : string){
    return this.http.get<MemberAddEdit>(`${environment.appUrl}/Api/Admin/get-member/${id}`);
  }

  getApplicationRoles(){
    return this.http.get<string[]>(`${environment.appUrl}/Api/Admin/get-roles`);
  }

  addEditMember(model : MemberAddEdit){
    return this.http.post(`${environment.appUrl}/Api/Admin/add-edit-member`,model);
  }

  lockMember(id : string){
    return this.http.put(`${environment.appUrl}/Api/Admin/lock-member/${id}`,{})
  }

  unLockMember(id : string){
    return this.http.put(`${environment.appUrl}/Api/Admin/unlock-member/${id}`,{})
  }

  deleteMember(id : string){
    return this.http.delete(`${environment.appUrl}/Api/Admin/delete-member/${id}`)
  }
}
