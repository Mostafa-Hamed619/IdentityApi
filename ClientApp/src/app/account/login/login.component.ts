import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit{

  loginForm : FormGroup = new FormGroup({});
  Submitted = false;
  errorMessages : string[] =[];
  constructor(private FormBuilder : FormBuilder,private AccountServices : AccountService,
    private router : Router ){}
  ngOnInit(): void {
      this.initializeForm();
  }

  initializeForm(){
    this.loginForm = this.FormBuilder.group({
        'username' : ['',[Validators.required,Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
        'password' : ['',[Validators.required]]
      }
    )
  }

  Login(){
    this.Submitted = true;
    this.errorMessages = [];
    if(this.loginForm.valid){
      this.AccountServices.login(this.loginForm.value).subscribe({
        next:(response)=>{
          console.log(response);
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
