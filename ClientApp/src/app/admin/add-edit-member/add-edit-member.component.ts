import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AdminService } from '../admin.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';
import { MemberAddEdit } from 'src/app/shared/models/Admin/MemberAddEdit';

@Component({
  selector: 'app-add-edit-member',
  templateUrl: './add-edit-member.component.html',
  styleUrls: ['./add-edit-member.component.css']
})
export class AddEditMemberComponent implements OnInit{
  memberForm : FormGroup = new FormGroup({});
  formInitialized = false;
  addNew = true;
  submitted = false;
  errorMessages : string[] =[];
  applicationRoles : string[] =[];
  existingMemberRoles : string[] =[];

  constructor(private adminServices : AdminService,private router : Router,
    private formBuilder : FormBuilder,private activatedRoute : ActivatedRoute,private sharedServices : SharedService){}
  
  
    ngOnInit(): void {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if(id){
      this.addNew = false;
      this.getMember(id);
    }
    else{
      this.initializeForm(undefined);  
    }

    this.getRole();
  }

  getMember(id :string){
    if(id){
      this.adminServices.getMember(id).subscribe({
        next:(member)=>{
          this.initializeForm(member)
        }
      })
    }
  }

  getRole(){
    this.adminServices.getApplicationRoles().subscribe({
      next:roles => this.applicationRoles = roles
    });
  }

  roleOnChange(selectedRoles : string){
    
    let roles = this.memberForm.get('roles')?.value.split(',');

    console.log(roles);
    const Index = roles.indexOf(selectedRoles)

    console.log(Index);
    Index !== -1 ? roles.splice(Index, 1) : roles.push(selectedRoles);

    if(roles[0] === ""){
      roles.splice(0,1)
    }

    this.memberForm.controls['roles'].setValue(roles.join(','));

    console.log(this.memberForm.get('roles')?.value)
  }
  initializeForm(member : MemberAddEdit | undefined){
    if(member){
      this.memberForm = this.formBuilder.group({
        id : [member.id],
        userName : [member.userName,Validators.required],
        firstName : [member.firstName,Validators.required],
        lastName : [member.lastName,Validators.required],
        password: [''],
        roles : [member.roles,Validators.required]
      });

      this.existingMemberRoles = member.roles.split(',');
    }
    else{
      this.memberForm = this.formBuilder.group({
        id : [''],
        userName : ['',Validators.required],
        firstName : ['',Validators.required],
        lastName : ['',Validators.required],
        password : ['',[Validators.required,Validators.minLength(3),Validators.maxLength(15)]],
        roles : ['',Validators.required]
    })
  }
  this.formInitialized = true;
  
}

passwordOnChange(){
  if(this.addNew == false){
    if(this.memberForm.get('password')?.value){
      this.memberForm.controls['password'].setValidators([Validators.required,Validators.minLength(5),Validators.max(15)]);
    }else{
      this.memberForm.get('password')?.clearValidators();
    }
    
    this.memberForm.get('password')?.updateValueAndValidity();
  }
  
}

submit(){
  this.submitted = true;
  this.errorMessages = [];
  if(this.memberForm.valid){
    this.adminServices.addEditMember(this.memberForm.value).subscribe({
      next:(response : any)=>{
        console.log('done');
        this.sharedServices.showNotification(true, response.value.title,response.value.message);
        this.router.navigateByUrl("/admin");
      },
      error:(error)=>{
        console.log(error)
        if(error.error.errors){
          this.errorMessages = error.error.errors
        }else{
          this.errorMessages.push(error);
        }
      }
    })
  }
}
}
