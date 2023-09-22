import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ResetPasswordModel } from 'src/app/shared/models/account/ResetPasswordModel';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit{


  ResetForm : FormGroup = new FormGroup({});
  token:string |undefined ;
  email:string |undefined;
  submitted =false;
  errorMessages : string[] = [];
  result : boolean = true;

  constructor(private accountService : AccountService,private router : Router
    ,private activatedRouter : ActivatedRoute,private sharedService : SharedService,
    private formBuilder : FormBuilder){}
  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
        next:(user : User | null)=>{
          if(user){
            this.router.navigateByUrl("/");
          }else{

             this.activatedRouter.queryParamMap.subscribe({
              next:(params : any)=>{
                this.token = params.get('token');
                this.email = params.get('email');
                if(this.token && this.email){

                 this.InitializeForm();
                }
            }
           })
          }
        }
    })
  }

  InitializeForm(){
    this.ResetForm = this.formBuilder.group({
      email: [this.email],
      'newPassword' : ['',[Validators.required,Validators.minLength(6),Validators.maxLength(15)]]
    })
  }

  resetForm(){
    this.submitted = true;
    this.errorMessages = [];

    if(this.ResetForm.valid && this.email && this.token){
      const model :ResetPasswordModel = {
        token : this.token,
        email : this.email,
        newPassword : this.ResetForm.get('newPassword')?.value
      };
      console.log(this.ResetForm.get('newPassword')?.value)
      this.accountService.resetPasswordFun(model).subscribe({
        next:(response : any)=>{
          this.sharedService.showNotification(true,response.value.title,response.value.message);
          this.router.navigateByUrl("/account/login");
        }
      })
    }
  }
}
