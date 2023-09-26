import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';
import { RegisterWithExternal } from 'src/app/shared/models/account/RegiseterWithExternal';

@Component({
  selector: 'app-register-with-third-party',
  templateUrl: './register-with-third-party.component.html',
  styleUrls: ['./register-with-third-party.component.css']
})
export class RegisterWithThirdPartyComponent implements OnInit{
  registerForm : FormGroup = new FormGroup({});
  submitted =false;
  provider : string | null = null;
  access_Token : string | null = null;
  userId : string | null = null;
  errorMessages : string[] =[];

  constructor(private accountService:AccountService,private router:Router,private activatedRouter:ActivatedRoute,
    private formBuilder:FormBuilder){}
  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
      next:(user : User | null)=>{
        if(user){
          this.router.navigateByUrl("/")
        }else{
          this.activatedRouter.queryParamMap.subscribe({
            next:(params : any)=>{
              
              this.provider = this.activatedRouter.snapshot.paramMap.get('provider');
              this.access_Token = params.get('access_token');
              this.userId = params.get('userId');
              

             if(this.provider && this.access_Token && this.userId && 
              (this.provider =='facebook' || this.provider == 'google')){
                this.initializeForm();
              }else{
                this.router.navigateByUrl('/account/register');
              }
            }
         
          })
        }
      }
    })
  }

  initializeForm(){
    this.registerForm = this.formBuilder.group({
      firstName : ['',[Validators.required,Validators.maxLength(15),Validators.minLength(3)]],
      lastName : ['',[Validators.required,Validators.maxLength(15),Validators.minLength(3)]]
    })
  }
  Register(){
    this.submitted = true;
    this.errorMessages = [];
    if(this.registerForm.valid &&this.userId && this.access_Token && this.provider){
      const firstName = this.registerForm.get('firstName')?.value;
      const lastName = this.registerForm.get('lastName')?.value;

      const model =new RegisterWithExternal(firstName,lastName,this.userId,this.access_Token,this.provider);

      this.accountService.registerWithThirdParty(model).subscribe({
        next:(response : any)=>{
            this.router.navigateByUrl("/");
        },error: error =>{
          if(error.error.errors){
            this.errorMessages = error.error.errors;
          }else{
            this.errorMessages.push(error.error);
          }
        }
      })
    }
  }
}
