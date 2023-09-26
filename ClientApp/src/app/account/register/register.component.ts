import { Component, ElementRef, Inject, OnInit, Renderer2, ViewChild } from '@angular/core';
import { AccountService } from '../account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from 'src/app/shared/shared.service';
import { Router } from '@angular/router';
import { shareReplay, take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';
import { CredentialResponse } from 'google-one-tap';
import jwt_decode from 'jwt-decode'
import { DOCUMENT } from '@angular/common';
declare const FB: any;

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  @ViewChild('googleButton', {static: true}) googleButton: ElementRef = new ElementRef({});

  registerForm : FormGroup = new FormGroup({});
  submitted = false;
  errormessages : string[]=[];
constructor(private accountService : AccountService,
  private formBuilder : FormBuilder, private sharedService : SharedService,
  private router : Router,private _render2:Renderer2,@Inject(DOCUMENT) private document : Document){
    this.accountService.user$.pipe(take(1)).subscribe({
      next:(user : User |null)=>{
        if(user){
          this.router.navigateByUrl("/")
        }
      }
    })
  }

  ngOnInit(): void {
    
      this.InitializeForm();
      this.initiazeGoogleButton();
  }

  ngAfterViewInit(){
    const script1 = this._render2.createComment('script');
    script1.src='https://accounts.google.com/gsi/client';
    script1.async = 'true';
    script1.defer = 'true';
    this._render2.appendChild(this.document.body,script1);
  }
  InitializeForm(){
      this.registerForm = this.formBuilder.group({
        FirstName: ['',[Validators.required,Validators.minLength(3),Validators.maxLength(15)]],
        LastName: ['',[Validators.required,Validators.minLength(3),Validators.maxLength(15)]],
        Email: ['',[Validators.required,Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
        Password: ['',[Validators.required,Validators.minLength(3),Validators.maxLength(15)]],
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

  registerWithFacebook(){
    FB.login(async(fbResult : any)=>{
      if(fbResult.authResponse){
        console.log(fbResult);
        const accessToken = fbResult.authResponse.accessToken;
        const UserId = fbResult.authResponse.userID;
        this.router.navigateByUrl(`/account/register/third-party/facebook?access_token=${accessToken}&userId=${UserId}`);
        
      }else{
        this.sharedService.showNotification(false,"Failed","Unable to register with your Facebook")
      }
    })
  }
  private initiazeGoogleButton() {
    (window as any).onGoogleLibraryLoad = () => {
      // @ts-ignore
      google.accounts.id.initialize({
        client_id: '479105200189-o76490m7bi9i1ojk8r93fuqpncg537u1.apps.googleusercontent.com',
        callback: this.googleCallBack.bind(this),
        auto_select: false,
        cancel_on_tap_outside: true
      });
      // @ts-ignore
      google.accounts.id.renderButton(
        this.googleButton.nativeElement,
        {size: 'medium', shape: 'rectangular', text: 'signup_with', logo_alignment: 'center'}
      );
    };
  }

  private async googleCallBack(response: CredentialResponse) {
      console.log(response);
      const decodedToken = jwt_decode(response.credential);
      this.router.navigateByUrl(`/account/register/third-party/google?access_token=${response.credential}&userId=${decodedToken}`);

  }

}
