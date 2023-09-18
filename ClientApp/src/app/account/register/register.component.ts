import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from 'src/app/shared/shared.service';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{

  registerForm : FormGroup = new FormGroup({});
  submitted = false;
  errormessages : string[]=[];
constructor(private accountService : AccountService,
  private formBuilder : FormBuilder, private sharedService : SharedService,
  private router : Router){
    this.accountService.user$.pipe(take(1)).subscribe({
      next:(user : User |null)=>{
        if(user){
          this.router.navigateByUrl("/")
        }
      }
    })
  }

  ngOnInit(): void {
      this.InitializeForm()
  }

  InitializeForm(){
      this.registerForm = this.formBuilder.group({
        FirstName: ['',[Validators.required,Validators.minLength(3),Validators.maxLength(15)]],
        LastName: ['',[Validators.required,Validators.minLength(3),Validators.maxLength(15)]],
        Email: ['',[Validators.required,Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
        Password: ['',[Validators.required,Validators.minLength(6),Validators.maxLength(15)]],
      })
  }

  register(){
    this.submitted = true;
    this.errormessages =[];
    if(this.registerForm.valid){
      this.accountService.register(this.registerForm.value).subscribe({
        next:(response : any)=>{
          this.sharedService.showNotification(true,response.value.title,response.value.message);
          this.router.navigateByUrl('/account/login')
          console.log(response)
        },
        error:(error)=>{
          console.log(error)
          if(error.error.errors){
            this.errormessages = error.error.errors;
          }else{
            this.errormessages.push(error.error);
          }
        }
      })
    }
  }
}
