import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { SharedService } from 'src/app/shared/shared.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.css']
})
export class SendEmailComponent implements OnInit{

  emailConfirm : FormGroup = new FormGroup({});
  submitted = false;
  mode : string | undefined;
  errorMessages : string[] =[]

  constructor(private accountService : AccountService,
    private sharedServices : SharedService,
    private activatedRoute : ActivatedRoute,
    private router : Router,
    private formBuilder : FormBuilder){}

  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
      next:(user : User | null)=>{
        if(user){
          this.router.navigateByUrl('/');
        }else{
          const mode = this.activatedRoute.snapshot.paramMap.get('mode');
          if(mode){
            this.mode = mode;
            console.log(mode);
            this.InitializeForm();
          }
        }
      }
    })
  }

  InitializeForm(){
    this.emailConfirm = this.formBuilder.group({
      'email' : ['',[Validators.required,Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]]
    })
  }

  sendEmail(){
    this.submitted = true;
    this.errorMessages = [];

    if(this.emailConfirm.valid && this.mode){
      if(this.mode.includes('resend-email-confirmation-link')){
        this.accountService.resendEmailConfirmation(this.emailConfirm.get('email')?.value).subscribe({
          next:(response : any)=>{
            this.sharedServices.showNotification(true,response.value.title,response.value.message);
            this.router.navigateByUrl('/account/login');
          }
        })
      }else if(this.mode.includes('forgot-password-or-username')){
        this.accountService.forgotPasswordOrUsername(this.emailConfirm.get('email')?.value).subscribe({
          next:(response : any)=>{
            this.sharedServices.showNotification(true,response.value.title,response.value.message);
            this.router.navigateByUrl("/account/login");
          },
          // error:(error : any)=>{
          //   if(error.error.errors){
          //     this.errorMessages = error.error.errors;
          //   }else{
          //     this.errorMessages.push(error.error);
          //   }
          // }
        })
      }
    }
  }


  Cancel(){
    this.router.navigateByUrl('/account/login');
  }
}
