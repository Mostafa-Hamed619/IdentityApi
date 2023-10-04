import { Component, OnInit, TemplateRef } from '@angular/core';
import { AdminService } from './admin.service';
import { SharedService } from '../shared/shared.service';
import { MemberDto } from '../shared/models/Admin/MemberDto';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit{

  members: MemberDto[] = [];
  modalRef?: BsModalRef;
  MemberToDelete : MemberDto | undefined;
  constructor(private AdminServices : AdminService,private sharedServices : SharedService,
    private modalService: BsModalService){

  }
  ngOnInit(): void {
    this.AdminServices.getMembers().subscribe({
      next:response =>{
        this.members = response
        console.log(response);
      } 
    })
  }
  LockMember(id : string){
    this.AdminServices.lockMember(id).subscribe({
      next:_=>{
        this.handleLockUnlockFilterAndMessage(id,true);
      }
    })
  }

  unLockMember(id : string){
    this.AdminServices.unLockMember(id).subscribe({
      next:_=>{
        this.handleLockUnlockFilterAndMessage(id,false);
      }
    })
  }

  deleteMember(id : string,template :TemplateRef<any>){
    let member = this.findMember(id);
    if(member){
      this.MemberToDelete = member;
      this.modalRef = this.modalService.show(template,{class:'modal-sm'})
    }
  }

  confirm(){
    if(this.MemberToDelete){
      this.AdminServices.deleteMember(this.MemberToDelete.id).subscribe({
        next:_=>{
          this.sharedServices.showNotification(true,"Delete Member",`Member ${this.MemberToDelete?.userName} has been deleted`)
          this.members = this.members.filter(x=>x.id !== this.MemberToDelete?.id);
          this.MemberToDelete = undefined;
          this.modalRef?.hide();
        }
      })
    }
  }

  decline(){
    this.MemberToDelete = undefined;
    this.modalRef?.hide();
  }
  private handleLockUnlockFilterAndMessage(id : string,locking: boolean){
    let member = this.findMember(id);

    if(member){
      member.isLocked = !member.isLocked;
      if(locking){
        this.sharedServices.showNotification(true,"Locked",`${member.userName} member has been locked`);
      }else{
        this.sharedServices.showNotification(true,"UnLocked",`${member.userName} member has been unlocked`);
      }
    }
  }

  private findMember(id : string):MemberDto|undefined{
    let member = this.members.find(x=>x.id === id);
    if(member){
      return member;
    }
    return undefined;
  }
}
