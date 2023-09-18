import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/user';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit{

  loginForm : FormGroup = new FormGroup({});
  Submitted = false;
  errorMessages : string[] =[];
  returnUrl : string =''

  profile :string | null =''
  constructor(private FormBuilder : FormBuilder,private AccountServices : AccountService,
    private router : Router,private activatedRoute : ActivatedRoute ){
      this.AccountServices.user$.pipe(take(1)).subscribe({
        next:(user : User | null)=>{
          if(user){
            this.router.navigateByUrl('/')
            console.log()
          }else{
            this.activatedRoute.queryParamMap.subscribe({
              next:(params :any)=>{
                if(params){
                  this.returnUrl = params.get('returnUrl')
                }
              }
            })
          }
        }
      })
    }
  ngOnInit(): void {
      this.initializeForm();
  }

  initializeForm(){
    this.loginForm = this.FormBuilder.group({
        'username' : ['Mos21@gmail.com',[Validators.required,Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
        'password' : ['Mos@21',[Validators.required]]
      }
    )
  }

  Login(){
    this.Submitted = true;
    this.errorMessages = [];
    if(this.loginForm.valid){
      this.AccountServices.login(this.loginForm.value).subscribe({
        next:(response)=>{
          if(this.returnUrl){
            this.router.navigateByUrl(this.returnUrl)

          }else{
            this.router.navigateByUrl('/');
          }
            
        },
        error:(error)=>{
          console.log(error)
          if(error.error.errors){
            this.errorMessages = error.error.errors;
          }else{
            this.errorMessages.push(error.error)
          }
        }
      })
    }
    
  }
}